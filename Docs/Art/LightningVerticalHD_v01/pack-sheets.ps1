$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$source = @'
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

public static class LightningSheetPacker {
  static Rectangle Bounds(Bitmap b, byte threshold) {
    int minX=b.Width,minY=b.Height,maxX=-1,maxY=-1;
    var d=b.LockBits(new Rectangle(0,0,b.Width,b.Height),ImageLockMode.ReadOnly,PixelFormat.Format32bppArgb);
    byte[] p=new byte[d.Stride*b.Height]; Marshal.Copy(d.Scan0,p,0,p.Length); b.UnlockBits(d);
    for(int y=0;y<b.Height;y++) for(int x=0;x<b.Width;x++) if(p[y*d.Stride+x*4+3]>=threshold) {
      minX=Math.Min(minX,x);minY=Math.Min(minY,y);maxX=Math.Max(maxX,x);maxY=Math.Max(maxY,y);
    }
    if(maxX<0) throw new Exception("Empty frame");
    return new Rectangle(minX,minY,maxX-minX+1,maxY-minY+1);
  }
  static void ClearHiddenRgb(Bitmap b) {
    var d=b.LockBits(new Rectangle(0,0,b.Width,b.Height),ImageLockMode.ReadWrite,PixelFormat.Format32bppArgb);
    byte[] p=new byte[d.Stride*b.Height];Marshal.Copy(d.Scan0,p,0,p.Length);
    for(int i=0;i<p.Length;i+=4) if(p[i+3]==0) {p[i]=0;p[i+1]=0;p[i+2]=0;}
    Marshal.Copy(p,0,d.Scan0,p.Length);b.UnlockBits(d);
  }
  public static void Run(string root) {
    string doc=Path.Combine(root,"Docs/Art/LightningVerticalHD_v01");
    string assets=Path.Combine(root,"Assets/04.Sprites/AirBossAttack");
    string prefix="Attack_Lightning_VerticalHD_AirBoss";
    foreach(int n in new[]{1,2}) if(File.Exists(Path.Combine(assets,prefix+"_Part"+n+"_16F_v01.png"))) throw new Exception("Output already exists; refusing overwrite");
    var report=new StringBuilder("frame,originalWidthPx,originalHeightPx,generatedWidthPx,generatedHeightPx,newWidthPx,newHeightPx,oldWorldHeight,newWorldHeight,oldWorldWidth,newWorldWidth,edgeMaxAlpha\n");
    using(var original=new Bitmap(Path.Combine(assets,"Attack_Lightning_VerticalGrowth_AirBoss_32F_v01.png"))) {
      Bitmap[] frames=new Bitmap[32];
      for(int group=0;group<8;group++) using(var generated=new Bitmap(Path.Combine(doc,"Generated/group"+(group+1).ToString("00")+".png"))) {
        if(generated.Width!=768 || generated.Height!=2048) throw new Exception("Unexpected native generation size");
        for(int c=0;c<4;c++) {
          int i=group*4+c;
          using(var old=original.Clone(new Rectangle(i%8*384,i/8*768,384,768),PixelFormat.Format32bppArgb))
          using(var src=generated.Clone(new Rectangle(c*192,0,192,2048),PixelFormat.Format32bppArgb)) {
            Rectangle ob=Bounds(old,32),nb=Bounds(src,32);
            // Match the visible alpha>=32 footprint of the original at root scale (10,100).
            // New PPU=2048/3, Visual scale=(10,1,1): combined XY scale is uniform100.
            double k=2048.0/768.0;
            float tw=(float)(ob.Width*k/10.0),th=(float)(ob.Height*k);
            float tx=(float)(128+(ob.X-192)*k/10.0),ty=(float)(ob.Y*k);
            float sx=tw/nb.Width,sy=th/nb.Height;
            var f=new Bitmap(256,2048,PixelFormat.Format32bppArgb);
            using(var g=Graphics.FromImage(f)) {
              g.CompositingMode=CompositingMode.SourceCopy;g.InterpolationMode=InterpolationMode.HighQualityBicubic;g.PixelOffsetMode=PixelOffsetMode.HighQuality;
              using(var attrs=new ImageAttributes()) {
                attrs.SetWrapMode(WrapMode.TileFlipXY);
                // Only resample/align the generated art; preserve its alpha and filaments.
                PointF[] dest={new PointF(tx-nb.X*sx,ty-nb.Y*sy),new PointF(tx+(src.Width-nb.X)*sx,ty-nb.Y*sy),new PointF(tx-nb.X*sx,ty+(src.Height-nb.Y)*sy)};
                g.DrawImage(src,dest,new RectangleF(0,0,src.Width,src.Height),GraphicsUnit.Pixel,attrs);
              }
            }
            ClearHiddenRgb(f);
            Rectangle fb=Bounds(f,32); int edge=0;
            for(int y=0;y<f.Height;y++) edge=Math.Max(edge,Math.Max(f.GetPixel(0,y).A,f.GetPixel(f.Width-1,y).A));
            for(int x=0;x<f.Width;x++) edge=Math.Max(edge,Math.Max(f.GetPixel(x,0).A,f.GetPixel(x,f.Height-1).A));
            if(edge!=0) throw new Exception("Nontransparent frame edge: "+i+" alpha="+edge);
            report.AppendLine(string.Join(",",new object[]{i,ob.Width,ob.Height,nb.Width,nb.Height,fb.Width,fb.Height,ob.Height/256.0*100,fb.Height/(2048.0/3)*100,ob.Width/256.0*10,fb.Width/(2048.0/3)*100,edge}));
            frames[i]=f;
          }
        }
      }
      for(int part=0;part<2;part++) using(var sheet=new Bitmap(4096,2048,PixelFormat.Format32bppArgb)) {
        using(var g=Graphics.FromImage(sheet)) {g.CompositingMode=CompositingMode.SourceCopy;for(int c=0;c<16;c++)g.DrawImageUnscaled(frames[part*16+c],c*256,0);}
        sheet.Save(Path.Combine(assets,prefix+"_Part"+(part+1)+"_16F_v01.png"),ImageFormat.Png);
      }
      int[] samples={0,7,15,16,23,28,31};
      using(var preview=new Bitmap(1400,1050,PixelFormat.Format32bppArgb)) using(var g=Graphics.FromImage(preview)) using(var font=new Font("Segoe UI",14)) {
        g.Clear(Color.FromArgb(255,18,23,38));g.InterpolationMode=InterpolationMode.HighQualityBicubic;
        g.DrawString("Same visible size | OLD / NEW | 32-frame vertical lightning",font,Brushes.White,20,12);
        for(int s=0;s<samples.Length;s++) {
          int i=samples[s];float x=s*200;
          g.DrawString("Frame "+i.ToString("00"),font,Brushes.White,x+45,48);
          g.DrawString("OLD     NEW",font,Brushes.LightSteelBlue,x+35,76);
          g.DrawImage(original,new RectangleF(x+38,110,45,900),new RectangleF(i%8*384,i/8*768,384,768),GraphicsUnit.Pixel);
          g.DrawImage(frames[i],new RectangleF(x+83,110,112.5f,900),new RectangleF(0,0,256,2048),GraphicsUnit.Pixel);
        }
        preview.Save(Path.Combine(doc,"comparison.png"),ImageFormat.Png);
      }
      foreach(var f in frames) f.Dispose();
    }
    File.WriteAllText(Path.Combine(doc,"pixel-validation.csv"),report.ToString());
  }
}
'@
Add-Type -TypeDefinition $source -ReferencedAssemblies @([System.Drawing.Bitmap].Assembly.Location, [System.Drawing.Rectangle].Assembly.Location)
[LightningSheetPacker]::Run((Resolve-Path "$PSScriptRoot/../../..").Path)
