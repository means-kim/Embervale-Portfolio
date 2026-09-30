$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing.Common
Add-Type -ReferencedAssemblies @([System.Drawing.Bitmap].Assembly.Location, [System.Drawing.Point].Assembly.Location, "$PSHOME/System.Private.Windows.GdiPlus.dll", "$PSHOME/System.Private.Windows.Core.dll", 'System.Runtime', 'System.Drawing') -TypeDefinition @'
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
public static class IceV3Atlas {
 public static void Run(string root) {
  using(var raw=new Bitmap(Path.Combine(root,"ArtSource/Boss_IceThorn_v3/Generated_4x4.png")))
  using(var sheet=new Bitmap(4096,1024,PixelFormat.Format32bppArgb)) {
   if(raw.Width!=1254 || raw.Height!=1254) throw new Exception("Reinspect atlas before changing its registration values.");
   int[] rows={0,350,662,949,1254}; int[] baseline={316,630,938,1231};
   int[][] columns={new[]{0,314,627,940,1254},new[]{0,314,627,940,1254},new[]{0,314,630,947,1254},new[]{0,314,627,940,1254}};
   const float scale=1.39f;
   var report=new StringBuilder("Source 1254x1254 RGBA, work atlas 4x4. Final 4096x1024 RGBA, 8x2.\nUniform scale 1.39 for all 16 poses. Same horizontal root and common ground baseline per source row. No per-frame bbox recenter or resize; no Idle cells in attack atlas.\n");
   using(var g=Graphics.FromImage(sheet)) {
    g.CompositingMode=CompositingMode.SourceCopy;g.InterpolationMode=InterpolationMode.HighQualityBicubic;g.PixelOffsetMode=PixelOffsetMode.HighQuality;
    for(int i=0;i<16;i++) {
     int r=i/4,c=i%4,x0=columns[r][c],x1=columns[r][c+1];
     using(var part=raw.Clone(new Rectangle(x0,rows[r],x1-x0,rows[r+1]-rows[r]),PixelFormat.Format32bppArgb)) {
      for(int y=0;y<part.Height;y++)for(int x=0;x<part.Width;x++)if(part.GetPixel(x,y).A<8)part.SetPixel(x,y,Color.Transparent);
      float dx=256-(raw.Width/8f)*scale+(x0-(float)Math.Round(c*raw.Width/4.0))*scale,dy=511-(baseline[r]-rows[r])*scale;
      var dest=new RectangleF((i%8)*512+dx,(i/8)*512+dy,part.Width*scale,part.Height*scale);
      g.SetClip(new Rectangle((i%8)*512,(i/8)*512,512,512));g.DrawImage(part,dest,new RectangleF(0,0,part.Width,part.Height),GraphicsUnit.Pixel);g.ResetClip();
      report.AppendLine("Frame "+(i+1)+": row "+r+", common local registration ("+dx+","+dy+"), source ("+x0+","+rows[r]+","+part.Width+","+part.Height+")");
     }
    }
   }
   sheet.Save(Path.Combine(root,"Assets/04.Sprites/Boss/Boss_IceThorn_v3.png"),ImageFormat.Png);
   for(int i=0;i<16;i++)using(var frame=sheet.Clone(new Rectangle((i%8)*512,(i/8)*512,512,512),PixelFormat.Format32bppArgb)) {
    frame.Save(Path.Combine(root,"Docs/Boss/IceThorn_v3/Frame_"+(i+1).ToString("00")+".png"),ImageFormat.Png);
   }
   File.WriteAllText(Path.Combine(root,"Docs/Boss/IceThorn_v3/Atlas_Registration.txt"),report.ToString());
  }
 }
}
'@
[IceV3Atlas]::Run((Resolve-Path "$PSScriptRoot/../..").Path)
