$ErrorActionPreference = 'Stop'
# Packing/QA only: drawings are the built-in image_gen outputs kept in ArtSource.
. (Join-Path $PSScriptRoot 'Inspect_AirBoss_Idle_Float_v01.ps1') | Out-Null
$airBossOffsets = @(@(0,0),@(-6,2),@(-2,1),@(-2,5),@(-6,8),@(-7,8),@(-6,8),@(-1,-5))
$airBossAtlasPixels = [byte[]]::new(4096*2048*4)
$airBossFinalReport = @()
$airBossMasterBitmap = [System.Drawing.Bitmap]::new($airBossMasterPath)
$airBossMasterPixelData = [AirBossPixels]::Read($airBossMasterBitmap)
$airBossMasterBitmap.Dispose()
$airBossPreviewDark = [System.Drawing.Bitmap]::new(2048,1088)
$airBossPreviewLight = [System.Drawing.Bitmap]::new(2048,1088)
$airBossGD = [System.Drawing.Graphics]::FromImage($airBossPreviewDark)
$airBossGL = [System.Drawing.Graphics]::FromImage($airBossPreviewLight)
$airBossGD.Clear([System.Drawing.Color]::FromArgb(255,35,42,54))
$airBossGL.Clear([System.Drawing.Color]::FromArgb(255,225,229,236))
$airBossLabelFont=[System.Drawing.Font]::new('Segoe UI',14)
foreach($airBossIndex in 0..7){
 $airBossId=$airBossIndex.ToString('00')
 $airBossFramePath=Join-Path $airBossFrameRoot ('AirBoss_Idle_Float_v01_'+$airBossId+'.png')
 if($airBossIndex -eq 0){
  $airBossP=$airBossMasterPixelData
  Copy-Item -LiteralPath $airBossMasterPath -Destination $airBossFramePath -Force
  $airBossRemoved=0
 }else{
  $airBossBitmap=[System.Drawing.Bitmap]::new((Join-Path $airBossRawRoot ('frame'+$airBossId+'_canvas1024.png')))
  $airBossP=[AirBossPixels]::Read($airBossBitmap);$airBossBitmap.Dispose()
  $airBossRemoved=[AirBossPixels]::CleanDetachedFaint($airBossP,1024,1024)
  $airBossP=[AirBossPixels]::Translate($airBossP,1024,1024,$airBossOffsets[$airBossIndex][0],$airBossOffsets[$airBossIndex][1])
  [AirBossPixels]::Save($airBossP,1024,1024,$airBossFramePath)
 }
 $airBossFinalBounds=[AirBossPixels]::Bounds($airBossP,1024,1024,1)
 $airBossFinalVisible=[AirBossPixels]::Bounds($airBossP,1024,1024,8)
 if($airBossFinalBounds[5] -ne 0){throw ('64px safety margin failed: '+$airBossId+' pixels='+$airBossFinalBounds[5])}
 [AirBossPixels]::Pack($airBossP,$airBossAtlasPixels,$airBossIndex)
 $airBossFrameBitmap=[System.Drawing.Bitmap]::new($airBossFramePath)
 $airBossDecoded=[AirBossPixels]::Read($airBossFrameBitmap)
 if(-not [AirBossPixels]::Equal($airBossP,$airBossDecoded)){throw ('PNG roundtrip changed pixels: '+$airBossId)}
 $airBossRow=[Math]::Floor($airBossIndex/4);$airBossColumn=$airBossIndex%4
 foreach($airBossReviewG in @($airBossGD,$airBossGL)){
  $airBossReviewG.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
  $airBossReviewG.DrawImage($airBossFrameBitmap,[System.Drawing.Rectangle]::new($airBossColumn*512,$airBossRow*544+32,512,512))
 }
 $airBossGD.DrawString($airBossId,$airBossLabelFont,[System.Drawing.Brushes]::White,[single]($airBossColumn*512+16),[single]($airBossRow*544+7))
 $airBossGL.DrawString($airBossId,$airBossLabelFont,[System.Drawing.Brushes]::Black,[single]($airBossColumn*512+16),[single]($airBossRow*544+7))
 $airBossFrameBitmap.Dispose()
 $airBossFinalReport += [pscustomobject]@{frame=$airBossId;dimensions=@(1024,1024);registrationTranslation=$airBossOffsets[$airBossIndex];detachedFaintPixelsRemoved=$airBossRemoved;alphaBounds=$airBossFinalBounds;visibleBounds=$airBossFinalVisible;minimumMargin=(@($airBossFinalBounds[0],$airBossFinalBounds[1],(1023-$airBossFinalBounds[2]),(1023-$airBossFinalBounds[3]))|Measure-Object -Minimum).Minimum;sha256=(Get-FileHash -LiteralPath $airBossFramePath -Algorithm SHA256).Hash}
}
$airBossSheetPath=Join-Path $airBossRoot 'ArtSource/AirBoss/idle/sheets/AirBoss_Idle_Float_4x2_1024_v01.png'
[AirBossPixels]::Save($airBossAtlasPixels,4096,2048,$airBossSheetPath)
$airBossSheetBitmap=[System.Drawing.Bitmap]::new($airBossSheetPath)
$airBossSavedAtlas=[AirBossPixels]::Read($airBossSheetBitmap);$airBossSheetBitmap.Dispose()
foreach($airBossIndex in 0..7){
 $airBossCellBitmap=[System.Drawing.Bitmap]::new((Join-Path $airBossFrameRoot ('AirBoss_Idle_Float_v01_'+$airBossIndex.ToString('00')+'.png')))
 $airBossP=[AirBossPixels]::Read($airBossCellBitmap);$airBossCellBitmap.Dispose()
 if(-not [AirBossPixels]::CellEquals($airBossP,$airBossSavedAtlas,$airBossIndex)){throw ('Atlas pixels differ in cell '+$airBossIndex)}
}
if(($airBossFinalReport.sha256|Select-Object -Unique).Count -ne 8){throw 'Duplicate frame file.'}
$airBossPreviewDark.Save((Join-Path $PSScriptRoot 'AirBoss_Idle_Float_v01_contact_dark.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$airBossPreviewLight.Save((Join-Path $PSScriptRoot 'AirBoss_Idle_Float_v01_contact_light.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$airBossGD.Dispose();$airBossGL.Dispose();$airBossPreviewDark.Dispose();$airBossPreviewLight.Dispose();$airBossLabelFont.Dispose()
$airBossFinalReport|ConvertTo-Json -Depth 6|Set-Content -LiteralPath (Join-Path $PSScriptRoot 'AirBoss_Idle_Float_v01.metrics.json') -Encoding utf8
$airBossFinalReport|ConvertTo-Json -Depth 6
