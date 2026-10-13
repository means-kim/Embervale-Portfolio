Add-Type -AssemblyName System.Drawing
$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$folder = Join-Path $projectRoot 'Assets/04.Sprites/NormalMonster/SlimeB_v01/Move'
New-Item -ItemType Directory -Force $folder | Out-Null
$generated = [System.Drawing.Bitmap]::new((Join-Path $PSScriptRoot 'Move_generated.png'))
$neutralPath = Join-Path $projectRoot 'Assets/04.Sprites/NormalMonster/SlimeB_v01/Idle/SlimeB_Idle_00.png'
Copy-Item -LiteralPath $neutralPath -Destination (Join-Path $folder 'SlimeB_Move_00.png')
$neutral = [System.Drawing.Bitmap]::new($neutralPath)
$sheet = [System.Drawing.Bitmap]::new(4608,512,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$sg = [System.Drawing.Graphics]::FromImage($sheet)
$sg.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceCopy
$sg.DrawImageUnscaled($neutral,0,0)
$poses = @(
 @{N=1;X=693;Y=143;W=473;H=250;TW=480;TH=258},
 @{N=2;X=1299;Y=148;W=487;H=245;TW=518;TH=247},
 @{N=3;X=94;Y=509;W=471;H=265;TW=490;TH=270},
 @{N=4;X=742;Y=509;W=412;H=266;TW=440;TH=302},
 @{N=5;X=1352;Y=509;W=433;H=266;TW=450;TH=290}
)
foreach($p in $poses){
 $frame=[System.Drawing.Bitmap]::new(768,512,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
 $g=[System.Drawing.Graphics]::FromImage($frame)
 $g.CompositingMode=[System.Drawing.Drawing2D.CompositingMode]::SourceCopy
 $g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
 $g.PixelOffsetMode=[System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
 $g.DrawImage($generated,[System.Drawing.Rectangle]::new([int]((768-$p.TW)/2),448-$p.TH,$p.TW,$p.TH),$p.X,$p.Y,$p.W,$p.H,[System.Drawing.GraphicsUnit]::Pixel)
 $g.Dispose()
 $frame.Save((Join-Path $folder ('SlimeB_Move_{0:00}.png' -f $p.N)),[System.Drawing.Imaging.ImageFormat]::Png)
 $sg.DrawImageUnscaled($frame,768*$p.N,0)
 $frame.Dispose()
}
$sg.Dispose()
$sheet.Save((Join-Path $folder 'SlimeB_Move_Sheet_v01.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$preview=[System.Drawing.Bitmap]::new(2304,1024)
$pg=[System.Drawing.Graphics]::FromImage($preview)
$pg.Clear([System.Drawing.Color]::FromArgb(38,48,65))
for($n=0;$n -lt 6;$n++){$x=($n%3)*768;$y=[int][Math]::Floor($n/3)*512;$pg.DrawImage($sheet,[System.Drawing.Rectangle]::new($x,$y,768,512),$n*768,0,768,512,[System.Drawing.GraphicsUnit]::Pixel)}
$preview.Save((Join-Path $PSScriptRoot 'Move_Review.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$pg.Dispose();$preview.Dispose();$sheet.Dispose();$neutral.Dispose();$generated.Dispose()
