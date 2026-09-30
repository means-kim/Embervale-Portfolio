param([switch]$Partial)
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$legacy=[IO.File]::ReadAllText((Join-Path $PSScriptRoot 'Inspect_AirBoss_Idle_Float_v01.ps1'))
$helper=[regex]::Match($legacy,"(?s)using System;.*?(?=\r?\n'@)").Value
$refs=@([System.Drawing.Bitmap].Assembly.Location,[System.Drawing.Color].Assembly.Location)+@(Get-ChildItem -LiteralPath $PSHOME -Filter 'System.Private.Windows.*.dll'|Select-Object -ExpandProperty FullName)
Add-Type -ReferencedAssemblies $refs -TypeDefinition $helper
$out=Join-Path $root 'ArtSource/AirBoss/move/frames/v01'
$raw=Join-Path $root 'ArtSource/AirBoss/move/source/Move_Bank_v01'
$atlas=[byte[]]::new(4096*4096*4)
$report=@()
$preview=[System.Drawing.Bitmap]::new(1536,1632)
$g=[System.Drawing.Graphics]::FromImage($preview)
$g.Clear([System.Drawing.Color]::FromArgb(255,35,42,54))
$g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$lightPreview=[System.Drawing.Bitmap]::new(1536,1632)
$lg=[System.Drawing.Graphics]::FromImage($lightPreview)
$lg.Clear([System.Drawing.Color]::FromArgb(255,222,227,234))
$lg.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$font=[System.Drawing.Font]::new('Segoe UI',12)
foreach($i in 0..15){
 $id=$i.ToString('00');$dest=Join-Path $out ('AirBoss_Move_Bank_v01_'+$id+'.png');$removed=0;$rawSize=@(1024,1024)
 if($i -eq 0){
  Copy-Item -LiteralPath (Join-Path $root 'ArtSource/AirBoss/idle/AirBoss_Idle_Master_image2.png') -Destination $dest -Force
  $b=[System.Drawing.Bitmap]::new($dest);$p=[AirBossPixels]::Read($b);$b.Dispose()
 }else{
  $origin=Join-Path $raw ('frame'+$id+'_generated.png')
  if(-not(Test-Path -LiteralPath $origin)){if($Partial){continue}else{throw "Missing $origin"}}
  $input=[System.Drawing.Bitmap]::new($origin);$rawSize=@($input.Width,$input.Height)
  if($input.Width -ne $input.Height){throw 'Expected square canvas'}
  $b=[System.Drawing.Bitmap]::new(1024,1024,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
  $bg=[System.Drawing.Graphics]::FromImage($b)
  $bg.CompositingMode=[System.Drawing.Drawing2D.CompositingMode]::SourceCopy
  $bg.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
  $bg.PixelOffsetMode=[System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
  $bg.DrawImage($input,[System.Drawing.Rectangle]::new(0,0,1024,1024),0,0,$input.Width,$input.Height,[System.Drawing.GraphicsUnit]::Pixel)
  $bg.Dispose();$input.Dispose();$p=[AirBossPixels]::Read($b);$b.Dispose()
  # Remove only detached nearly invisible generated specks. Preserve connected soft edges.
  $removed=[AirBossPixels]::CleanDetachedFaint($p,1024,1024)
  [AirBossPixels]::Save($p,1024,1024,$dest)
 }
 $bounds=[AirBossPixels]::Bounds($p,1024,1024,1);$visible=[AirBossPixels]::Bounds($p,1024,1024,8)
 if($bounds[5] -ne 0){if($Partial){Write-Warning "64px safety failed frame $id, outside pixels $($bounds[5])"}else{throw "64px safety failed frame $id, outside pixels $($bounds[5])"}}
 [AirBossPixels]::Pack($p,$atlas,$i)
 $b=[System.Drawing.Bitmap]::new($dest)
 if(-not[AirBossPixels]::Equal($p,[AirBossPixels]::Read($b))){throw 'PNG roundtrip mismatch'}
 $x=($i%4)*384;$y=[Math]::Floor($i/4)*408
 $g.DrawImage($b,[System.Drawing.Rectangle]::new($x,$y+24,384,384))
 $lg.DrawImage($b,[System.Drawing.Rectangle]::new($x,$y+24,384,384));$b.Dispose()
 $phase=if($i -lt 4){'Entry'}elseif($i -lt 12){'Loop'}else{'Exit'}
 $g.DrawString(($id+' '+$phase),$font,[System.Drawing.Brushes]::White,[single]($x+8),[single]($y+3))
 $lg.DrawString(($id+' '+$phase),$font,[System.Drawing.Brushes]::Black,[single]($x+8),[single]($y+3))
 $report += [pscustomobject]@{frame=$id;phase=$phase;rawSize=$rawSize;registrationTranslation=@(0,0);detachedFaintPixelsRemoved=$removed;alphaBounds=$bounds;visibleBounds=$visible;minimumMargin=(@($bounds[0],$bounds[1],(1023-$bounds[2]),(1023-$bounds[3]))|Measure-Object -Minimum).Minimum;sha256=(Get-FileHash -LiteralPath $dest -Algorithm SHA256).Hash}
}
$preview.Save((Join-Path $PSScriptRoot 'AirBoss_Move_Bank_v01_contact_dark.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$lightPreview.Save((Join-Path $PSScriptRoot 'AirBoss_Move_Bank_v01_contact_light.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$lg.Dispose();$lightPreview.Dispose()
$g.Dispose();$font.Dispose();$preview.Dispose()
$report|ConvertTo-Json -Depth 6|Set-Content -LiteralPath (Join-Path $PSScriptRoot 'AirBoss_Move_Bank_v01.metrics.json') -Encoding utf8
if(-not $Partial){
 $sheet=Join-Path $root 'ArtSource/AirBoss/move/sheets/AirBoss_Move_Bank_4x4_1024_v01.png'
 [AirBossPixels]::Save($atlas,4096,4096,$sheet)
 $b=[System.Drawing.Bitmap]::new($sheet);$decoded=[AirBossPixels]::Read($b);$b.Dispose()
 foreach($i in 0..15){$b=[System.Drawing.Bitmap]::new((Join-Path $out ('AirBoss_Move_Bank_v01_'+$i.ToString('00')+'.png')));if(-not[AirBossPixels]::CellEquals([AirBossPixels]::Read($b),$decoded,$i)){throw 'Atlas mismatch'};$b.Dispose()}
 if(($report.sha256|Select-Object -Unique).Count -ne 16){throw 'Duplicate frame'}
 Copy-Item -LiteralPath $sheet -Destination (Join-Path $root 'Assets/04.Sprites/AirBoss/AirBoss_Move_Bank_4x4_1024_v01.png') -Force
}
$report|Select-Object frame,phase,rawSize,minimumMargin|Format-Table
