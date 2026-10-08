$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$root = (Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$target = Join-Path $root 'Assets/04.Sprites/Light'
$backup = Join-Path $PSScriptRoot 'Original'
New-Item -ItemType Directory -Path $backup -Force | Out-Null
$sheet = [System.Drawing.Bitmap]::FromFile((Join-Path $PSScriptRoot 'Remastered_16Frames.png'))
if ($sheet.Width -ne 1024 -or $sheet.Height -ne 1536) { throw 'Unexpected sheet dimensions' }
# Generated rows have unequal vertical spacing. Crop at verified transparent gaps.
$rowCuts = @(0,384,768,1132,1536)
for ($i=0; $i -lt 16; $i++) {
    $name = 'Flame_A_Soft_{0:D2}.png' -f $i
    $originalPath = Join-Path $target $name
    foreach ($suffix in @('', '.meta')) {
        $saved = Join-Path $backup ($name+$suffix)
        if (!(Test-Path -LiteralPath $saved)) { Copy-Item -LiteralPath ($originalPath+$suffix) -Destination $saved }
    }
    $row = [int][math]::Floor($i/4)
    $left = ($i%4)*256
    $top = $rowCuts[$row]
    $height = $rowCuts[$row+1]-$top
    $rect = [System.Drawing.Rectangle]::new($left,$top,256,$height)
    $cell = $sheet.Clone($rect,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $bottom = -1
    for ($y=0; $y -lt $height; $y++) {
        for ($x=0; $x -lt 256; $x++) {
            if ($cell.GetPixel($x,$y).A -gt 16) { $bottom = $y; break }
        }
    }
    if ($bottom -lt 0) { throw "Empty frame $i" }
    $frame = [System.Drawing.Bitmap]::new(256,384,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($frame)
    $g.Clear([System.Drawing.Color]::Transparent)
    $g.DrawImageUnscaled($cell,0,373-$bottom)
    $frame.Save($originalPath,[System.Drawing.Imaging.ImageFormat]::Png)
    $g.Dispose(); $frame.Dispose(); $cell.Dispose()
    # Preserve GUID, sprite identity, pivot, animation references and nominal world size.
    $meta = [System.IO.File]::ReadAllText((Join-Path $backup ($name+'.meta')))
    $meta = $meta.Replace('spritePixelsToUnits: 100','spritePixelsToUnits: 160')
    [System.IO.File]::WriteAllText(($originalPath+'.meta'),$meta,[System.Text.UTF8Encoding]::new($false))
    Write-Output "$name : 256x384, PPU 160, baseline y=373"
}
$sheet.Dispose()
