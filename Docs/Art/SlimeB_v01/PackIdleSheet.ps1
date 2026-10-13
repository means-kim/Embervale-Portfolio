Add-Type -AssemblyName System.Drawing
$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$assetFolder = Join-Path $projectRoot 'Assets/04.Sprites/NormalMonster/SlimeB_v01/Idle'
$generated = [System.Drawing.Bitmap]::new((Join-Path $PSScriptRoot 'Idle_Breathing_generated.png'))
$neutral = [System.Drawing.Bitmap]::new((Join-Path $assetFolder 'SlimeB_Idle_00.png'))
$sheet = [System.Drawing.Bitmap]::new(3072,512,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$sheetGraphics = [System.Drawing.Graphics]::FromImage($sheet)
$sheetGraphics.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceCopy
$sheetGraphics.DrawImageUnscaled($neutral,0,0)
# Cell packing of Image Gen poses. Neutral image remains byte-identical.
$poses = @(
    @{ Frame=1; X=603; Y=242; W=441; H=350; TargetW=438; TargetH=297 },
    @{ Frame=2; X=1145; Y=205; W=426; H=387; TargetW=426; TargetH=314 }
)
foreach($pose in $poses) {
    $frame = [System.Drawing.Bitmap]::new(768,512,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $graphics = [System.Drawing.Graphics]::FromImage($frame)
    $graphics.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceCopy
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $target = [System.Drawing.Rectangle]::new([int]((768-$pose.TargetW)/2),448-$pose.TargetH,$pose.TargetW,$pose.TargetH)
    $graphics.DrawImage($generated,$target,$pose.X,$pose.Y,$pose.W,$pose.H,[System.Drawing.GraphicsUnit]::Pixel)
    $graphics.Dispose()
    $frame.Save((Join-Path $assetFolder ('SlimeB_Idle_{0:00}.png' -f $pose.Frame)),[System.Drawing.Imaging.ImageFormat]::Png)
    $sheetGraphics.DrawImageUnscaled($frame,768*$pose.Frame,0)
    if($pose.Frame -eq 1) {
        $frame.Save((Join-Path $assetFolder 'SlimeB_Idle_03.png'),[System.Drawing.Imaging.ImageFormat]::Png)
        $sheetGraphics.DrawImageUnscaled($frame,2304,0)
    }
    $frame.Dispose()
}
$sheetGraphics.Dispose()
$sheet.Save((Join-Path $assetFolder 'SlimeB_Idle_Sheet_v01.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$preview = [System.Drawing.Bitmap]::new(3072,512)
$previewGraphics = [System.Drawing.Graphics]::FromImage($preview)
$previewGraphics.Clear([System.Drawing.Color]::FromArgb(38,48,65))
$previewGraphics.DrawImageUnscaled($sheet,0,0)
$preview.Save((Join-Path $PSScriptRoot 'Idle_Sheet_Review.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$previewGraphics.Dispose(); $preview.Dispose(); $sheet.Dispose(); $neutral.Dispose(); $generated.Dispose()
