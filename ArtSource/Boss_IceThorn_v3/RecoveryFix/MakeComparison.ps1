$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing.Common
$dir=Join-Path (Get-Location) 'Docs/Boss/IceThorn_v3/RecoveryFix'
$canvas=[System.Drawing.Bitmap]::new(1232,514)
$g=[System.Drawing.Graphics]::FromImage($canvas)
$g.Clear([System.Drawing.Color]::FromArgb(18,27,39))
$g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$font=[System.Drawing.Font]::new('Malgun Gothic',19)
$small=[System.Drawing.Font]::new('Malgun Gothic',12)
$white=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(235,241,247))
$muted=[System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(156,175,194))
$pen=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(100,94,200,230),1)
$ground=[System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(125,117,227,170),1)
$files=@("$dir/Before/Frame_16.png","$dir/Frame_16.png",'Docs/Boss/IceThorn_v01/Frame_0.png')
$titles=@('수정 전 · Frame 16','수정 후 · Frame 16','기준 · Idle')
$subtitles=@('높이 309px / 바닥 y=509','높이 349px / 바닥 y=511','높이 349px / 바닥 y=511')
try{
 for($i=0;$i -lt 3;$i++){
  $x=16+$i*400
  $g.DrawString($titles[$i],$font,$white,[single]$x,[single]15)
  $g.DrawString($subtitles[$i],$small,$muted,[single]$x,[single]51)
  $sprite=[System.Drawing.Bitmap]::new((Resolve-Path $files[$i]).Path)
  try{$g.DrawImage($sprite,[System.Drawing.Rectangle]::new($x,84,384,384))}finally{$sprite.Dispose()}
  $g.DrawLine($pen,[single]$x,[single]206.25,[single]($x+384),[single]206.25)
  $g.DrawLine($ground,[single]$x,[single]468,[single]($x+384),[single]468)
 }
 $g.DrawString('세 그림 모두 동일 배율 · 512 × 512 셀 · Bottom Center 피봇 · PPU 50',$small,$muted,[single]16,[single]483)
 $canvas.Save("$dir/Size_Comparison.png",[System.Drawing.Imaging.ImageFormat]::Png)
}finally{$g.Dispose();$canvas.Dispose();$font.Dispose();$small.Dispose();$white.Dispose();$muted.Dispose();$pen.Dispose();$ground.Dispose()}

