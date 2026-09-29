$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing.Common
Add-Type -ReferencedAssemblies @([System.Drawing.Bitmap].Assembly.Location,[System.Drawing.Point].Assembly.Location,"$PSHOME/System.Private.Windows.GdiPlus.dll","$PSHOME/System.Private.Windows.Core.dll",'System.Runtime','System.Drawing') -TypeDefinition @'
using System;using System.Drawing;using System.Drawing.Imaging;using System.Drawing.Drawing2D;using System.IO;using System.Text;
public static class IceRecoveryFix {
 public static void Run(string root) {
  string path=Path.Combine(root,"Assets/04.Sprites/Boss/Boss_IceThorn_v3.png");
  using var sourceStream=new MemoryStream(File.ReadAllBytes(path));
  using var original=new Bitmap(sourceStream);
  if(original.Width!=4096||original.Height!=1024)throw new Exception("Wrong target atlas size.");
  Bitmap atlas=original.Clone(new Rectangle(0,0,4096,1024),PixelFormat.Format32bppArgb);
  using(atlas)using(var raw=new Bitmap(Path.Combine(root,"ArtSource/Boss_IceThorn_v3/RecoveryFix/Generated_2x2.png"))) {
   if(raw.Width!=1254||raw.Height!=1254)throw new Exception("Re-inspect changed source before packing.");
   const float scale=.724f;int[] cols={0,642,1254},rows={0,627,1254},ground={587,1165};
   var log=new StringBuilder("Only atlas cells 13-16 replaced. Source 1254x1254 RGBA 2x2; common scale 0.724; shared horizontal roots and row ground baseline. No runtime scale, pivot, PPU or clip change.\n");
   using(var g=Graphics.FromImage(atlas)) {
    g.CompositingMode=CompositingMode.SourceCopy;g.InterpolationMode=InterpolationMode.HighQualityBicubic;g.PixelOffsetMode=PixelOffsetMode.HighQuality;
    for(int i=0;i<4;i++) {
     int r=i/2,c=i%2,index=i+12;
     using(var part=raw.Clone(new Rectangle(cols[c],rows[r],cols[c+1]-cols[c],rows[r+1]-rows[r]),PixelFormat.Format32bppArgb)) {
      for(int y=0;y<part.Height;y++)for(int x=0;x<part.Width;x++)if(part.GetPixel(x,y).A<8)part.SetPixel(x,y,Color.Transparent);
      float dx=256-raw.Width/4f*scale+(cols[c]-c*raw.Width/2f)*scale,dy=512-(ground[r]-rows[r])*scale;
      var cell=new Rectangle(index%8*512,index/8*512,512,512);g.SetClip(cell);g.FillRectangle(Brushes.Transparent,cell);
      g.DrawImage(part,new RectangleF(cell.X+dx,cell.Y+dy,part.Width*scale,part.Height*scale),new RectangleF(0,0,part.Width,part.Height),GraphicsUnit.Pixel);g.ResetClip();
      log.AppendLine("Frame "+(index+1)+" crop="+cols[c]+","+rows[r]+","+part.Width+","+part.Height+" registration="+dx+","+dy);
     }
    }
   }
   atlas.Save(path,ImageFormat.Png);
   for(int i=12;i<16;i++)using(var frame=atlas.Clone(new Rectangle(i%8*512,i/8*512,512,512),PixelFormat.Format32bppArgb)){
    frame.Save(Path.Combine(root,"Docs/Boss/IceThorn_v3/RecoveryFix/Frame_"+(i+1).ToString("00")+".png"),ImageFormat.Png);
    frame.Save(Path.Combine(root,"Docs/Boss/IceThorn_v3/LiftRevision/Frame_"+(i+1).ToString("00")+".png"),ImageFormat.Png);
   }
   File.WriteAllText(Path.Combine(root,"Docs/Boss/IceThorn_v3/RecoveryFix/Alignment.txt"),log.ToString());
  }
 }
}
'@
[IceRecoveryFix]::Run((Resolve-Path "$PSScriptRoot/../../..").Path)
