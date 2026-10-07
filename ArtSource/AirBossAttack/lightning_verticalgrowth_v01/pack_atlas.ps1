param([Parameter(Mandatory=$true)][string[]]$Sources)
$ErrorActionPreference = 'Stop'
if ($Sources.Count -ne 4) { throw 'Exactly four 4x2 source sheets are required.' }
Add-Type -AssemblyName System.Drawing
Add-Type -ReferencedAssemblies System.Drawing.Common,System.Drawing.Primitives,System.Private.Windows.GdiPlus,System.Private.Windows.Core,System.Runtime.InteropServices,System.Console,System.Collections -TypeDefinition @'
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
public static class LightningAtlasPack {
  public static void Run(string[] paths, string output, string metricsPath) {
    var sheets = new Bitmap[4];
    int cellW = 0, cellH = 0;
    try {
      for (int p=0;p<4;p++) {
        sheets[p] = new Bitmap(paths[p]);
        cellW = Math.Max(cellW,(int)Math.Ceiling(sheets[p].Width/4.0));
        cellH = Math.Max(cellH,(int)Math.Ceiling(sheets[p].Height/2.0));
      }
      // Extra transparent padding permits registration without discarding any source pixel.
      cellW = ((Math.Max(cellW,(cellH+1)/2)+64+31)/32)*32;
      cellH = cellW*2;
      using (var atlas = new Bitmap(cellW*8,cellH*4,PixelFormat.Format32bppArgb)) {
        {
          var dest=atlas.LockBits(new Rectangle(0,0,atlas.Width,atlas.Height),ImageLockMode.WriteOnly,PixelFormat.Format32bppArgb);
          var packed=new byte[dest.Stride*atlas.Height];
          int[] order = {0,1,2,8,3,9,4,10,5,16,11,12,6,17,13,7,18,25,14,19,24,20,15,21,22,23,26,27,28,29,30,31};
          var mapping=new System.Collections.Generic.List<string>();
          mapping.Add("frame,source_frame,source_part,source_cell,source_rect,offset_x,offset_y");
          for (int f=0;f<32;f++) {
            int sf=order[f], p=sf/8, local=sf%8, col=local%4, row=local/4;
            var s=sheets[p];
            int x0=(int)Math.Round(col*s.Width/4.0), x1=(int)Math.Round((col+1)*s.Width/4.0);
            int y0=(int)Math.Round(row*s.Height/2.0), y1=(int)Math.Round((row+1)*s.Height/2.0);
            int w=x1-x0,h=y1-y0;
            int ymin=h,ymax=-1;
            for(int yy=0;yy<h;yy++)for(int xx=0;xx<w;xx++) {
              if(s.GetPixel(x0+xx,y0+yy).A>=16){ymin=Math.Min(ymin,yy);ymax=Math.Max(ymax,yy);}
            }
            // Register the luminous endpoints, rather than the asymmetrical branch bounding box.
            double endpointX=0,weight=0;
            if(ymax>=ymin)for(int end=0;end<2;end++)for(int d=5;d<15;d++) {
              int yy=end==0?ymin+d:ymax-d;
              for(int xx=0;xx<w;xx++) {
                var px=s.GetPixel(x0+xx,y0+yy); double q=px.A*px.A;
                endpointX+=xx*q;weight+=q;
              }
            }
            double cx=weight>0?endpointX/weight:(w-1)/2.0;
            double cy=ymax>=ymin?(ymin+ymax)/2.0:(h-1)/2.0;
            int ox=(int)Math.Round(cellW/2.0-cx),oy=(int)Math.Round(cellH/2.0-cy);
            if(ox<0||oy<0||ox+w>cellW||oy+h>cellH)throw new Exception("Insufficient padding for registered source cell "+sf);
            var src=s.LockBits(new Rectangle(x0,y0,w,h),ImageLockMode.ReadOnly,PixelFormat.Format32bppArgb);
            var rowPixels=new byte[w*4];
            for(int y=0;y<h;y++) {
              Marshal.Copy(IntPtr.Add(src.Scan0,y*src.Stride),rowPixels,0,rowPixels.Length);
              Array.Copy(rowPixels,0,packed,((f/8)*cellH+oy+y)*dest.Stride+((f%8)*cellW+ox)*4,rowPixels.Length);
            }
            s.UnlockBits(src);
            mapping.Add(string.Join(",",f.ToString("D2"),sf.ToString("D2"),p+1,local,x0+"/"+y0+"/"+w+"/"+h,ox,oy));
          }
          Marshal.Copy(packed,0,dest.Scan0,packed.Length);
          atlas.UnlockBits(dest);
          File.WriteAllLines(Path.Combine(Path.GetDirectoryName(output),"frame_mapping.csv"),mapping);
        }
        atlas.Save(output,ImageFormat.Png);
        var data=atlas.LockBits(new Rectangle(0,0,atlas.Width,atlas.Height),ImageLockMode.ReadOnly,PixelFormat.Format32bppArgb);
        var bytes=new byte[data.Stride*atlas.Height];
        Marshal.Copy(data.Scan0,bytes,0,bytes.Length);
        var lines=new System.Collections.Generic.List<string>();
        lines.Add("frame,cell_width,cell_height,alpha_nonzero,alpha_partial,edge_alpha_max,visible_xmin,visible_ymin,visible_xmax,visible_ymax,core_pixels,alpha_sum");
        for(int f=0;f<32;f++) {
          int count=0,partial=0,edge=0,xmin=cellW,ymin=cellH,xmax=-1,ymax=-1,core=0; long asum=0;
          for(int y=0;y<cellH;y++) for(int x=0;x<cellW;x++) {
            int i=((f/8)*cellH+y)*data.Stride+((f%8)*cellW+x)*4;
            int a=bytes[i+3]; asum+=a;
            if(a>0)count++;
            if(a>0&&a<255)partial++;
            if(x<4||x>=cellW-4||y<4||y>=cellH-4)edge=Math.Max(edge,a);
            if(a>=16){xmin=Math.Min(xmin,x);ymin=Math.Min(ymin,y);xmax=Math.Max(xmax,x);ymax=Math.Max(ymax,y);}
            if(a>=128 && bytes[i+2]>=225 && bytes[i+1]>=225 && bytes[i]>=235)core++;
          }
          lines.Add(string.Join(",",f.ToString("D2"),cellW,cellH,count,partial,edge,xmin,ymin,xmax,ymax,core,asum));
        }
        atlas.UnlockBits(data);
        File.WriteAllLines(metricsPath,lines);
        Console.WriteLine("Packed 32 cells without resampling or repainting. Sheet: "+atlas.Width+"x"+atlas.Height+". Cell: "+cellW+"x"+cellH+".");
      }
    } finally { foreach(var s in sheets) if(s!=null)s.Dispose(); }
  }
}
'@
$outputFile = Join-Path $PSScriptRoot 'Attack_Lightning_VerticalGrowth_AirBoss_32F_v01_Final.png'
if (Test-Path -LiteralPath $outputFile) { throw "Existing atlas protected: $outputFile" }
[LightningAtlasPack]::Run($Sources, $outputFile, (Join-Path $PSScriptRoot 'atlas_metrics.csv'))
