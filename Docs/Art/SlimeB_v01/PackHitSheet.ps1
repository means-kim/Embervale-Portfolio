Add-Type -AssemblyName System.Drawing
$root=(Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$folder=Join-Path $root 'Assets/04.Sprites/NormalMonster/SlimeB_v01/Hit'
New-Item -ItemType Directory -Force $folder | Out-Null
$source=[System.Drawing.Bitmap]::new((Join-Path $PSScriptRoot 'Hit_generated.png'))
$sheet=[System.Drawing.Bitmap]::new(2304,512,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$sg=[System.Drawing.Graphics]::FromImage($sheet)
$sg.CompositingMode=[System.Drawing.Drawing2D.CompositingMode]::SourceCopy
$poses=@(@{N=0;X=43;Y=395;W=619;H=302;TW=518;TH=224},@{N=1;X=708;Y=289;W=483;H=408;TW=414;TH=308})
foreach($p in $poses){
 $frame=[System.Drawing.Bitmap]::new(768,512,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
 $g=[System.Drawing.Graphics]::FromImage($frame)
 $g.CompositingMode=[System.Drawing.Drawing2D.CompositingMode]::SourceCopy
 $g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
 $g.PixelOffsetMode=[System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
 $g.DrawImage($source,[System.Drawing.Rectangle]::new([int]((768-$p.TW)/2),448-$p.TH,$p.TW,$p.TH),$p.X,$p.Y,$p.W,$p.H,[System.Drawing.GraphicsUnit]::Pixel)
 $g.Dispose()
 $frame.Save((Join-Path $folder ('SlimeB_Hit_{0:00}.png' -f $p.N)),[System.Drawing.Imaging.ImageFormat]::Png)
 $sg.DrawImageUnscaled($frame,768*$p.N,0)
 $frame.Dispose()
}
$neutralPath=Join-Path $root 'Assets/04.Sprites/NormalMonster/SlimeB_v01/Idle/SlimeB_Idle_00.png'
Copy-Item -LiteralPath $neutralPath -Destination (Join-Path $folder 'SlimeB_Hit_02.png')
$neutral=[System.Drawing.Bitmap]::new($neutralPath)
$sg.DrawImageUnscaled($neutral,1536,0);$sg.Dispose()
$sheet.Save((Join-Path $folder 'SlimeB_Hit_Sheet_v01.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$preview=[System.Drawing.Bitmap]::new(2304,512)
$pg=[System.Drawing.Graphics]::FromImage($preview);$pg.Clear([System.Drawing.Color]::FromArgb(38,48,65));$pg.DrawImageUnscaled($sheet,0,0)
$preview.Save((Join-Path $PSScriptRoot 'Hit_Review.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$pg.Dispose();$preview.Dispose();$neutral.Dispose();$sheet.Dispose();$source.Dispose()
