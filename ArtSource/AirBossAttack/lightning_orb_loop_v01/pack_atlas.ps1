param()
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
Add-Type -ReferencedAssemblies System.Drawing.Common,System.Drawing.Primitives,System.Private.Windows.GdiPlus,System.Private.Windows.Core,System.Runtime.InteropServices,System.Console,System.Collections -TypeDefinition @'
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
public static class OrbAtlasPack {
 public static void Run(string source,string output,string metrics,string contact) {
  int[] xs={0,333,638,944,1254},ys={0,367,676,969,1254};
  int[] cx={178,486,790,1098},cy={218,517,835,1103};
  const int cell=512;
  using(var src=new Bitmap(source))
  using(var atlas=new Bitmap(cell*4,cell*4,PixelFormat.Format32bppArgb)) {
   if(src.Width!=1254||src.Height!=1254)throw new Exception("Unexpected source dimensions");
   var lines=new List<string>{"frame,source_rect,nucleus_x,nucleus_y,offset_x,offset_y,source_edge_max_alpha,final_edge_max_alpha"};
   var data=atlas.LockBits(new Rectangle(0,0,atlas.Width,atlas.Height),ImageLockMode.WriteOnly,PixelFormat.Format32bppArgb);
   var bytes=new byte[data.Stride*atlas.Height];
   for(int f=0;f<16;f++){
    int col=f%4,row=f/4,x0=xs[col],y0=ys[row],w=xs[col+1]-x0,h=ys[row+1]-y0;
    double sum=0,sx=0,sy=0;
    for(int y=cy[row]-16;y<=cy[row]+16;y++)for(int x=cx[col]-16;x<=cx[col]+16;x++){
     var p=src.GetPixel(x,y);
     double q=Math.Max(0,Math.Min(p.R,Math.Min(p.G,p.B))-210)*p.A;
     sx+=x*q;sy+=y*q;sum+=q;
    }
    if(sum<=0)throw new Exception("Cannot locate nucleus "+f);
    int nx=(int)Math.Round(sx/sum),ny=(int)Math.Round(sy/sum);
    int ox=cell/2-(nx-x0),oy=cell/2-(ny-y0);
    if(ox<4||oy<4||ox+w>cell-4||oy+h>cell-4)throw new Exception("Insufficient padding "+f);
    int edge=0;
    for(int y=y0;y<y0+h;y++){edge=Math.Max(edge,src.GetPixel(x0,y).A);edge=Math.Max(edge,src.GetPixel(x0+w-1,y).A);}
    for(int x=x0;x<x0+w;x++){edge=Math.Max(edge,src.GetPixel(x,y0).A);edge=Math.Max(edge,src.GetPixel(x,y0+h-1).A);}
    var rectData=src.LockBits(new Rectangle(x0,y0,w,h),ImageLockMode.ReadOnly,PixelFormat.Format32bppArgb);
    var rowBytes=new byte[w*4];
    for(int y=0;y<h;y++){
     Marshal.Copy(IntPtr.Add(rectData.Scan0,y*rectData.Stride),rowBytes,0,rowBytes.Length);
     Array.Copy(rowBytes,0,bytes,((row*cell+oy+y)*data.Stride+(col*cell+ox)*4),rowBytes.Length);
    }
    src.UnlockBits(rectData);
    lines.Add(string.Join(",",f.ToString("D2"),x0+"/"+y0+"/"+w+"/"+h,nx,ny,ox,oy,edge,0));
   }
   Marshal.Copy(bytes,0,data.Scan0,bytes.Length);atlas.UnlockBits(data);
   atlas.Save(output,ImageFormat.Png);File.WriteAllLines(metrics,lines);
   using(var preview=new Bitmap(atlas.Width,atlas.Height,PixelFormat.Format24bppRgb))
   using(var g=Graphics.FromImage(preview)){
    g.Clear(Color.FromArgb(24,32,48));g.DrawImageUnscaled(atlas,0,0);preview.Save(contact,ImageFormat.Png);
   }
   Console.WriteLine("2048x2048 RGBA, 16 centered 512x512 cells; source pixels copied without scaling, painting, alpha removal or rotation.");
  }
 }
}
'@
[OrbAtlasPack]::Run(
 (Join-Path $PSScriptRoot 'generated_16f_refined.png'),
 (Join-Path $PSScriptRoot 'Attack_LightningOrb_FlightLoop_AirBoss_16F_v01.png'),
 (Join-Path $PSScriptRoot 'frame_registration.csv'),
 (Join-Path $PSScriptRoot 'contact_sheet_dark.png'))

