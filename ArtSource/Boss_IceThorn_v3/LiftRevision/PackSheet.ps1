$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing.Common
Add-Type -ReferencedAssemblies @([System.Drawing.Bitmap].Assembly.Location,[System.Drawing.Point].Assembly.Location,"$PSHOME/System.Private.Windows.GdiPlus.dll","$PSHOME/System.Private.Windows.Core.dll",'System.Runtime','System.Drawing') -TypeDefinition @'
using System;using System.Drawing;using System.Drawing.Imaging;using System.Drawing.Drawing2D;using System.IO;using System.Text;
public static class IceLiftAtlas {
 public static void Run(string root) {
  using(var raw=new Bitmap(Path.Combine(root,"ArtSource/Boss_IceThorn_v3/LiftRevision/Generated_4x4.png")))
  using(var atlas=new Bitmap(4096,1024,PixelFormat.Format32bppArgb)) {
   if(raw.Width!=1254||raw.Height!=1254)throw new Exception("Source geometry changed; re-inspect registration.");
   int[] rows={0,326,616,955,1254}, ground={300,612,936,1214};
   int[][] cols={new[]{0,323,636,947,1254},new[]{0,334,642,960,1254},new[]{0,329,638,951,1254},new[]{0,326,647,962,1254}};
   const float scale=1.375f;
   var log=new StringBuilder("Source 1254x1254 RGBA, 4x4. Final 4096x1024 RGBA, 8x2, 512 cells. All 16 poses scale 1.375. Fixed nominal column roots; common row ground registration. No per-frame bounding-box recenter, trim or scale.\n");
   using(var g=Graphics.FromImage(atlas)) {
    g.CompositingMode=CompositingMode.SourceCopy;g.InterpolationMode=InterpolationMode.HighQualityBicubic;g.PixelOffsetMode=PixelOffsetMode.HighQuality;
    for(int i=0;i<16;i++) {
     int r=i/4,c=i%4,x0=cols[r][c],x1=cols[r][c+1];
     using(var part=raw.Clone(new Rectangle(x0,rows[r],x1-x0,rows[r+1]-rows[r]),PixelFormat.Format32bppArgb)) {
      for(int y=0;y<part.Height;y++)for(int x=0;x<part.Width;x++)if(part.GetPixel(x,y).A<8)part.SetPixel(x,y,Color.Transparent);
      float dx=256-(raw.Width/8f)*scale+(x0-c*raw.Width/4f)*scale,dy=511-(ground[r]-rows[r])*scale;
      g.SetClip(new Rectangle(i%8*512,i/8*512,512,512));g.DrawImage(part,new RectangleF(i%8*512+dx,i/8*512+dy,part.Width*scale,part.Height*scale),new RectangleF(0,0,part.Width,part.Height),GraphicsUnit.Pixel);g.ResetClip();
      log.AppendLine("Frame "+(i+1)+": crop="+x0+","+rows[r]+","+part.Width+","+part.Height+"; registration="+dx+","+dy);
     }
    }
   }
   atlas.Save(Path.Combine(root,"Assets/04.Sprites/Boss/Boss_IceThorn_v3.png"),ImageFormat.Png);
   for(int i=0;i<16;i++)using(var frame=atlas.Clone(new Rectangle(i%8*512,i/8*512,512,512),PixelFormat.Format32bppArgb))frame.Save(Path.Combine(root,"Docs/Boss/IceThorn_v3/LiftRevision/Frame_"+(i+1).ToString("00")+".png"),ImageFormat.Png);
   File.WriteAllText(Path.Combine(root,"Docs/Boss/IceThorn_v3/LiftRevision/Alignment.txt"),log.ToString());
  }
 }
}
'@
[IceLiftAtlas]::Run((Resolve-Path "$PSScriptRoot/../../..").Path)
