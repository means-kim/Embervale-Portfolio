$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing.Common
Add-Type -ReferencedAssemblies @([System.Drawing.Bitmap].Assembly.Location, [System.Drawing.Point].Assembly.Location, "$PSHOME/System.Private.Windows.GdiPlus.dll", "$PSHOME/System.Private.Windows.Core.dll", 'System.Runtime', 'System.Drawing') -TypeDefinition @'
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
public static class IceSheetPacker {
 public static void Run(string root) {
  using(var raw=new Bitmap(Path.Combine(root,"ArtSource/Boss_IceThorn_v01/Generated.png")))
  using(var idle=new Bitmap(Path.Combine(root,"ArtSource/Boss_IceThorn_v01/Boss_Idle.png")))
  using(var sheet=new Bitmap(2048,1024,PixelFormat.Format32bppArgb)) {
   var report=new StringBuilder("Generated: "+raw.Width+"x"+raw.Height+"; final 2048x1024; scale 1.14, uniform across new poses.\n");
   using(var g=Graphics.FromImage(sheet)) {
    g.CompositingMode=CompositingMode.SourceCopy;
    g.InterpolationMode=InterpolationMode.HighQualityBicubic;
    g.PixelOffsetMode=PixelOffsetMode.HighQuality;
    for(int i=0;i<8;i++) {
     int destX=(i%4)*512, destY=(i/4)*512;
     if(i==0 || i==7) {g.DrawImage(idle,new Rectangle(destX,destY,512,512),new Rectangle(0,0,512,512),GraphicsUnit.Pixel);continue;}
     int x0=(int)Math.Round((i%4)*raw.Width/4.0),x1=(int)Math.Round((i%4+1)*raw.Width/4.0);
     int y0=i<4?0:470,y1=i<4?467:raw.Height;
     using(var cell=raw.Clone(new Rectangle(x0,y0,x1-x0,y1-y0),PixelFormat.Format32bppArgb)) {
      int bottom=0;
      for(int y=0;y<cell.Height;y++) for(int x=0;x<cell.Width;x++) {
       var c=cell.GetPixel(x,y);
       if(c.A<8) cell.SetPixel(x,y,Color.Transparent);
       else if(c.A>=48) bottom=Math.Max(bottom,y);
      }
      float scale=1.14f; int w=(int)Math.Round(cell.Width*scale), h=(int)Math.Round((bottom+1)*scale);
      int dx=(512-w)/2,dy=512-h;
      g.SetClip(new Rectangle(destX,destY,512,512));
      g.DrawImage(cell,new Rectangle(destX+dx,destY+dy,w,h),new Rectangle(0,0,cell.Width,bottom+1),GraphicsUnit.Pixel);
      g.ResetClip();
      report.AppendLine("Frame "+i+": source "+x0+","+y0+", size "+cell.Width+"x"+(bottom+1)+"; destination "+dx+","+dy+","+w+","+h);
     }
    }
   }
   // Copy endpoints exactly, without resampling or alpha rounding.
   for(int y=0;y<512;y++) for(int x=0;x<512;x++) {
    var pixel=idle.GetPixel(x,y);sheet.SetPixel(x,y,pixel);sheet.SetPixel(1536+x,512+y,pixel);
   }
   report.AppendLine("Frames 0 and 7: exact RGBA copy of Idle_0.");
   sheet.Save(Path.Combine(root,"Assets/04.Sprites/Boss/Boss_IceThorn_v01.png"),ImageFormat.Png);
   for(int i=0;i<8;i++) using(var frame=sheet.Clone(new Rectangle((i%4)*512,(i/4)*512,512,512),PixelFormat.Format32bppArgb)) {
    frame.Save(Path.Combine(root,"Docs/Boss/IceThorn_v01/Frame_"+i+".png"),ImageFormat.Png);
   }
   File.WriteAllText(Path.Combine(root,"Docs/Boss/IceThorn_v01/Alignment.txt"),report.ToString());
  }
 }
}
'@
[IceSheetPacker]::Run((Resolve-Path "$PSScriptRoot/../..").Path)
