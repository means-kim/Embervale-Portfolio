using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Security.Cryptography;
public static class PackTallIceV04 {
 public static void Run(string root) {
  string dir=Path.Combine(root,"Docs/Art/AirBossAttack/IceCylinder_Tall_v04");
  Directory.CreateDirectory(Path.Combine(dir,"Frames"));
  int[] tops={736,635,388,165,242,122,122,122,126,126,124,124,90,90,90,88,112,112,192,312,435,650,503,552};
  int[] bases={819,818,791,791,812,812,812,812,794,794,792,792,792,791,790,790,778,778,756,755,1020,1020,632,632};
  float[] widths={156,156,156,156,152,152,152,152,160,160,160,160,168,168,168,168,152,152,152,152,152,152,152,152};
  float[] heights={94,156,312,468,640,780,780,780,780,780,780,780,780,780,780,780,780,780,600,470,312,190,90,55};
  var lines=new List<string>();lines.Add("frame,source_width,source_height,scale_x,scale_y,body_or_effect_height,alpha32_bounds,edge_alpha32_pixels,sha256");
  using(var sheet=new Bitmap(3072,4096,PixelFormat.Format32bppArgb))
  using(var sg=Graphics.FromImage(sheet)) {
   sg.Clear(Color.Transparent);sg.CompositingMode=CompositingMode.SourceCopy;
   using(var contact=new Bitmap(1536,2048,PixelFormat.Format32bppArgb))
   using(var cg=Graphics.FromImage(contact)) {
    cg.Clear(Color.FromArgb(255,26,34,47));cg.InterpolationMode=InterpolationMode.HighQualityBicubic;
    for(int n=0;n<24;n++) {
     int group=n/4+1,q=n%4, ox=(q%2==0)?0:444,oy=(q<2)?0:887, sh=887,sw=(q%2==0)?444:443;
     if(group==6) {oy=(q<2)?0:1100;sh=(q<2)?1100:674;}
     using(var src=new Bitmap(Path.Combine(dir,"GenerationSources/Group_"+group+".png")))
     using(var cell=new Bitmap(sw,sh,PixelFormat.Format32bppArgb))
     using(var cellg=Graphics.FromImage(cell)) {
      cellg.CompositingMode=CompositingMode.SourceCopy;
      cellg.DrawImage(src,new Rectangle(0,0,sw,sh),new Rectangle(ox,oy,sw,sh),GraphicsUnit.Pixel);
      float sx=160f/widths[n],sy=heights[n]/(bases[n]-tops[n]);
      if(n>=20) sx=sy; // uniform shrinking of disappearing fragments, retaining their shape
      float dx=256f-222f*sx,dy=888f-bases[n]*sy;
      using(var frame=new Bitmap(512,1024,PixelFormat.Format32bppArgb))
      using(var fg=Graphics.FromImage(frame))
      using(var attrs=new ImageAttributes()) {
       fg.Clear(Color.Transparent);fg.CompositingMode=CompositingMode.SourceCopy;
       fg.InterpolationMode=InterpolationMode.HighQualityBicubic;fg.PixelOffsetMode=PixelOffsetMode.HighQuality;
       attrs.SetWrapMode(WrapMode.TileFlipXY);
       PointF[] dst={new PointF(dx,dy),new PointF(dx+sw*sx,dy),new PointF(dx,dy+sh*sy)};
       fg.DrawImage(cell,dst,new RectangleF(0,0,sw,sh),GraphicsUnit.Pixel,attrs);
       int x0=512,y0=1024,x1=-1,y1=-1,edge=0;
       for(int y=0;y<1024;y++)for(int x=0;x<512;x++){if(frame.GetPixel(x,y).A>=32){x0=Math.Min(x0,x);y0=Math.Min(y0,y);x1=Math.Max(x1,x);y1=Math.Max(y1,y);if(x==0||x==511||y==0||y==1023)edge++;}}
       string path=Path.Combine(dir,"Frames/Frame_"+(n+1).ToString("D2")+".png");
       frame.Save(path,ImageFormat.Png);
       string hash;using(var sha=SHA256.Create())hash=BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-","");
       lines.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture,"{0},{1},{2},{3:F5},{4:F5},{5},\"{6},{7},{8},{9}\",{10},{11}",n+1,sw,sh,sx,sy,heights[n],x0,y0,x1,y1,edge,hash));
       sg.DrawImageUnscaled(frame,(n%6)*512,(n/6)*1024);
       cg.DrawImage(frame,new Rectangle((n%6)*256,(n/6)*512,256,512));
       using(var brush=new SolidBrush(Color.White))using(var font=new Font("Arial",13)) cg.DrawString((n+1).ToString("D2"),font,brush,(n%6)*256+8,(n/6)*512+6);
      }
     }
    }
    contact.Save(Path.Combine(dir,"ContactSheet_v04.png"),ImageFormat.Png);
   }
   sheet.Save(Path.Combine(root,"Assets/04.Sprites/AirBossAttack/Attack_IceCylinder_Tall_AirBoss_24F_v04.png"),ImageFormat.Png);
  }
  File.WriteAllLines(Path.Combine(dir,"FrameMetrics.csv"),lines);
 }
}
