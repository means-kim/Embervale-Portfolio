$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing.Common
Add-Type -ReferencedAssemblies @([System.Drawing.Bitmap].Assembly.Location,[System.Drawing.Point].Assembly.Location,"$PSHOME/System.Private.Windows.GdiPlus.dll","$PSHOME/System.Private.Windows.Core.dll",'System.Runtime','System.Drawing') -TypeDefinition @'
using System;using System.Drawing;using System.IO;using System.Text;
public static class RecoveryQA {
 static string Bounds(Bitmap b,int cx,int cy){int l=512,t=512,r=-1,d=-1;for(int y=0;y<512;y++)for(int x=0;x<512;x++)if(b.GetPixel(cx+x,cy+y).A>40){l=Math.Min(l,x);t=Math.Min(t,y);r=Math.Max(r,x);d=Math.Max(d,y);}return "bbox=("+l+","+t+")-("+r+","+d+") width="+(r-l+1)+" height="+(d-t+1);}
 public static string Run(){var log=new StringBuilder();using(var cur=new Bitmap("Assets/04.Sprites/Boss/Boss_IceThorn_v3.png"))using(var old=new Bitmap("Docs/Boss/IceThorn_v3/RecoveryFix/Before/Boss_IceThorn_v3.png"))using(var idle=new Bitmap("Docs/Boss/IceThorn_v01/Frame_0.png")){
 log.AppendLine("Alpha threshold >40/255. Coordinates: cell top-left origin. Final PNG: "+cur.Width+"x"+cur.Height);
 log.AppendLine("Idle: "+Bounds(idle,0,0));
 for(int i=0;i<16;i++){int count=0,cx=i%8*512,cy=i/8*512;for(int y=0;y<512;y++)for(int x=0;x<512;x++)if(cur.GetPixel(cx+x,cy+y).ToArgb()!=old.GetPixel(cx+x,cy+y).ToArgb())count++;
 if(i<12 && count!=0)throw new Exception("Unexpected change in frame "+(i+1));
 log.AppendLine("Frame "+(i+1)+": changedPixels="+count+" "+Bounds(cur,cx,cy));if(i>=12)log.AppendLine("  Before: "+Bounds(old,cx,cy));
 }
 }File.WriteAllText("Docs/Boss/IceThorn_v3/RecoveryFix/Pixel_Verification.txt",log.ToString());return log.ToString();}
}
'@
[RecoveryQA]::Run()

