param([switch]$InspectOnly)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$airBossRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$airBossMasterPath = Join-Path $airBossRoot 'ArtSource/AirBoss/idle/AirBoss_Idle_Master_image2.png'
$airBossRawRoot = Join-Path $airBossRoot 'ArtSource/AirBoss/idle/source/Idle_Float_v01'
$airBossFrameRoot = Join-Path $airBossRoot 'ArtSource/AirBoss/idle/frames/v01'
$airBossDrawingReferences = @([System.Drawing.Bitmap].Assembly.Location,[System.Drawing.Color].Assembly.Location) + @(Get-ChildItem -LiteralPath $PSHOME -Filter 'System.Private.Windows.*.dll' | Select-Object -ExpandProperty FullName)
Add-Type -ReferencedAssemblies $airBossDrawingReferences -TypeDefinition @'
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
public static class AirBossPixels {
 public static byte[] Read(Bitmap b) {
  var d=b.LockBits(new Rectangle(0,0,b.Width,b.Height),ImageLockMode.ReadOnly,PixelFormat.Format32bppArgb);
  var p=new byte[b.Width*b.Height*4];
  for(int y=0;y<b.Height;y++) Marshal.Copy(IntPtr.Add(d.Scan0,y*d.Stride),p,y*b.Width*4,b.Width*4);
  b.UnlockBits(d); return p;
 }
 public static int[] Bounds(byte[] p,int w,int h,int threshold) {
  int minX=w,minY=h,maxX=-1,maxY=-1,count=0,outside=0;
  for(int y=0;y<h;y++)for(int x=0;x<w;x++)if(p[(y*w+x)*4+3]>=threshold){
   minX=Math.Min(minX,x);minY=Math.Min(minY,y);maxX=Math.Max(maxX,x);maxY=Math.Max(maxY,y);count++;
   if(x<64||x>=w-64||y<64||y>=h-64)outside++;
  }
  return new[]{minX,minY,maxX,maxY,count,outside};
 }
 public static double[] MatchMask(byte[] m,byte[] f) {
  double best=double.MaxValue; int bx=0,by=0;
  for(int dy=-24;dy<=24;dy++)for(int dx=-24;dx<=24;dx++){
   double sum=0;
   for(int y=218;y<318;y+=2)for(int x=497;x<550;x+=2){
    int mi=(y*1024+x)*4, fi=((y+dy)*1024+x+dx)*4;
    for(int c=0;c<3;c++){double delta=(double)m[mi+c]*m[mi+3]/255.0-(double)f[fi+c]*f[fi+3]/255.0;sum+=delta*delta;}
   }
   if(sum<best){best=sum;bx=dx;by=dy;}
  }
  return new double[]{bx,by,Math.Sqrt(best/(50*27*3))};
 }
 public static int CleanDetachedFaint(byte[] p,int w,int h) {
  int n=w*h;var distance=new int[n];
  for(int i=0;i<n;i++)distance[i]=p[i*4+3]>=8?0:4096;
  for(int y=0;y<h;y++)for(int x=0;x<w;x++){
   int i=y*w+x,d=distance[i];
   if(x>0)d=Math.Min(d,distance[i-1]+1);
   if(y>0){d=Math.Min(d,distance[i-w]+1);if(x>0)d=Math.Min(d,distance[i-w-1]+1);if(x<w-1)d=Math.Min(d,distance[i-w+1]+1);}
   distance[i]=d;
  }
  for(int y=h-1;y>=0;y--)for(int x=w-1;x>=0;x--){
   int i=y*w+x,d=distance[i];
   if(x<w-1)d=Math.Min(d,distance[i+1]+1);
   if(y<h-1){d=Math.Min(d,distance[i+w]+1);if(x>0)d=Math.Min(d,distance[i+w-1]+1);if(x<w-1)d=Math.Min(d,distance[i+w+1]+1);}
   distance[i]=d;
  }
  var seen=new bool[n];var queue=new int[n];int removed=0;
  for(int start=0;start<n;start++){
   if(seen[start]||p[start*4+3]==0)continue;
   int head=0,tail=1,maxA=0,minD=4096;queue[0]=start;seen[start]=true;
   while(head<tail){
    int i=queue[head++],x=i%w,y=i/w;maxA=Math.Max(maxA,p[i*4+3]);minD=Math.Min(minD,distance[i]);
    for(int oy=-1;oy<=1;oy++)for(int ox=-1;ox<=1;ox++){
     int xx=x+ox,yy=y+oy;if(xx<0||xx>=w||yy<0||yy>=h)continue;int j=yy*w+xx;
     if(!seen[j]&&p[j*4+3]>0){seen[j]=true;queue[tail++]=j;}
    }
   }
   if(maxA<8&&minD>16)for(int k=0;k<tail;k++){int i=queue[k]*4;p[i]=p[i+1]=p[i+2]=p[i+3]=0;removed++;}
  }
  return removed;
 }
 public static byte[] Translate(byte[] p,int w,int h,int dx,int dy) {
  var dest=new byte[p.Length];
  for(int y=0;y<h;y++)for(int x=0;x<w;x++){
   int i=(y*w+x)*4,xx=x+dx,yy=y+dy;
   if(xx<0||xx>=w||yy<0||yy>=h){if(p[i+3]>0)throw new Exception("Nontransparent pixel would be clipped.");continue;}
   int j=(yy*w+xx)*4;Buffer.BlockCopy(p,i,dest,j,4);
  }
  return dest;
 }
 public static void Save(byte[] p,int w,int h,string path) {
  using(var b=new Bitmap(w,h,PixelFormat.Format32bppArgb)){
   var d=b.LockBits(new Rectangle(0,0,w,h),ImageLockMode.WriteOnly,PixelFormat.Format32bppArgb);
   for(int y=0;y<h;y++)Marshal.Copy(p,y*w*4,IntPtr.Add(d.Scan0,y*d.Stride),w*4);
   b.UnlockBits(d);b.Save(path,ImageFormat.Png);
  }
 }
 public static bool Equal(byte[] a,byte[] b){if(a.Length!=b.Length)return false;for(int i=0;i<a.Length;i++)if(a[i]!=b[i])return false;return true;}
 public static void Pack(byte[] cell,byte[] sheet,int id){int sx=(id%4)*1024,sy=(id/4)*1024;for(int y=0;y<1024;y++)Buffer.BlockCopy(cell,y*4096,sheet,((sy+y)*4096+sx)*4,4096);}
 public static bool CellEquals(byte[] cell,byte[] sheet,int id){int sx=(id%4)*1024,sy=(id/4)*1024;for(int y=0;y<1024;y++)for(int x=0;x<4096;x++)if(cell[y*4096+x]!=sheet[((sy+y)*4096+sx)*4+x])return false;return true;}
}
'@
$airBossMaster = [System.Drawing.Bitmap]::new($airBossMasterPath)
$airBossMasterPixels = [AirBossPixels]::Read($airBossMaster)
$airBossReport = @()
$airBossPreview = [System.Drawing.Bitmap]::new(2048,1088)
$airBossPreviewGraphics = [System.Drawing.Graphics]::FromImage($airBossPreview)
$airBossPreviewGraphics.Clear([System.Drawing.Color]::FromArgb(255,35,42,54))
$airBossPreviewGraphics.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$airBossFont=[System.Drawing.Font]::new('Segoe UI',14)
foreach($airBossIndex in 0..7){
 $airBossId=$airBossIndex.ToString('00')
 $airBossNormalizedPath=Join-Path $airBossRawRoot ('frame'+$airBossId+'_canvas1024.png')
 if($airBossIndex -eq 0){$airBossCell=[System.Drawing.Bitmap]::new($airBossMasterPath);$airBossInputWidth=1024;$airBossInputHeight=1024}
 else{
  $airBossInput=[System.Drawing.Bitmap]::new((Join-Path $airBossRawRoot ('frame'+$airBossId+'_generated.png')))
  $airBossInputWidth=$airBossInput.Width;$airBossInputHeight=$airBossInput.Height
  if($airBossInputWidth -ne $airBossInputHeight){throw 'Unexpected nonsquare generated canvas.'}
  $airBossCell=[System.Drawing.Bitmap]::new(1024,1024,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
  $airBossG=[System.Drawing.Graphics]::FromImage($airBossCell)
  $airBossG.Clear([System.Drawing.Color]::Transparent)
  $airBossG.CompositingMode=[System.Drawing.Drawing2D.CompositingMode]::SourceCopy
  $airBossG.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
  $airBossG.PixelOffsetMode=[System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
  $airBossG.DrawImage($airBossInput,[System.Drawing.Rectangle]::new(0,0,1024,1024),0,0,$airBossInputWidth,$airBossInputHeight,[System.Drawing.GraphicsUnit]::Pixel)
  $airBossG.Dispose();$airBossInput.Dispose()
  $airBossCell.Save($airBossNormalizedPath,[System.Drawing.Imaging.ImageFormat]::Png)
 }
 $airBossPixels=[AirBossPixels]::Read($airBossCell)
 $airBossBounds=[AirBossPixels]::Bounds($airBossPixels,1024,1024,1)
 $airBossVisible=[AirBossPixels]::Bounds($airBossPixels,1024,1024,8)
 $airBossMatch=[AirBossPixels]::MatchMask($airBossMasterPixels,$airBossPixels)
 $airBossReport += [pscustomobject]@{frame=$airBossId;rawSize=@($airBossInputWidth,$airBossInputHeight);alphaBounds=$airBossBounds;visibleBounds=$airBossVisible;maskOffsetInGenerated=@($airBossMatch[0],$airBossMatch[1]);maskRGB_RMSE=$airBossMatch[2]}
 $airBossColumn=$airBossIndex%4;$airBossRow=[Math]::Floor($airBossIndex/4)
 $airBossPreviewGraphics.DrawImage($airBossCell,[System.Drawing.Rectangle]::new($airBossColumn*512,$airBossRow*544+32,512,512))
 $airBossPreviewGraphics.DrawString($airBossId,$airBossFont,[System.Drawing.Brushes]::White,[single]($airBossColumn*512+16),[single]($airBossRow*544+7))
 $airBossCell.Dispose()
}
$airBossPreview.Save((Join-Path $PSScriptRoot 'AirBoss_Idle_Float_v01_unregistered_review.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$airBossPreviewGraphics.Dispose();$airBossPreview.Dispose();$airBossFont.Dispose();$airBossMaster.Dispose()
$airBossReport|ConvertTo-Json -Depth 6|Set-Content -LiteralPath (Join-Path $PSScriptRoot 'AirBoss_Idle_Float_v01_generation_metrics.json') -Encoding utf8
$airBossReport|ConvertTo-Json -Depth 6

