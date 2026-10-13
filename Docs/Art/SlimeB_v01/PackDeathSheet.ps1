Add-Type -AssemblyName System.Drawing
$root=(Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$folder=Join-Path $root 'Assets/04.Sprites/NormalMonster/SlimeB_v01/Death'
New-Item -ItemType Directory -Force $folder | Out-Null
$src=[System.Drawing.Bitmap]::new((Join-Path $PSScriptRoot 'Death_generated.png'))
$sheet=[System.Drawing.Bitmap]::new(3072,512,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$sg=[System.Drawing.Graphics]::FromImage($sheet);$sg.CompositingMode=[System.Drawing.Drawing2D.CompositingMode]::SourceCopy
$poses=@(@{N=0;X=91;Y=160;W=617;H=404;TW=460;TH=250},@{N=1;X=818;Y=298;W=664;H=266;TW=510;TH=180},@{N=2;X=178;Y=759;W=445;H=154;TW=380;TH=105},@{N=3;X=1024;Y=803;W=283;H=114;TW=210;TH=85})
foreach($p in $poses){
 $frame=[System.Drawing.Bitmap]::new(768,512,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
 $g=[System.Drawing.Graphics]::FromImage($frame);$g.CompositingMode=[System.Drawing.Drawing2D.CompositingMode]::SourceCopy
 $g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic;$g.PixelOffsetMode=[System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
 $g.DrawImage($src,[System.Drawing.Rectangle]::new([int]((768-$p.TW)/2),448-$p.TH,$p.TW,$p.TH),$p.X,$p.Y,$p.W,$p.H,[System.Drawing.GraphicsUnit]::Pixel);$g.Dispose()
 $frame.Save((Join-Path $folder ('SlimeB_Death_{0:00}.png' -f $p.N)),[System.Drawing.Imaging.ImageFormat]::Png)
 $sg.DrawImageUnscaled($frame,$p.N*768,0);$frame.Dispose()
}
$sg.Dispose();$sheet.Save((Join-Path $folder 'SlimeB_Death_Sheet_v01.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$preview=[System.Drawing.Bitmap]::new(3072,512);$pg=[System.Drawing.Graphics]::FromImage($preview)
$pg.Clear([System.Drawing.Color]::FromArgb(38,48,65));$pg.DrawImageUnscaled($sheet,0,0)
$preview.Save((Join-Path $PSScriptRoot 'Death_Review.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$pg.Dispose();$preview.Dispose();$src.Dispose();$sheet.Dispose()
