$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$root=(Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$frames=Import-Csv -LiteralPath (Join-Path $PSScriptRoot 'Manifest.tsv') -Delimiter "`t"
$output=Join-Path $root 'Assets/04.Sprites/VFX/AttackRemaster_v01'
New-Item -ItemType Directory -Path $output -Force | Out-Null
$report=@()
foreach($group in ($frames | Group-Object clip)) {
    $count=if($group.Name -eq 'StabAttackVFX'){10}else{7}
    $cols=if($count -eq 10){5}else{4}
    $src=[Drawing.Bitmap]::FromFile((Join-Path $PSScriptRoot ($group.Name+'_Generated.png')))
    foreach($f in ($group.Group | Select-Object -First $count)) {
        $i=[int]$f.index
        $x0=[int][math]::Round(($i%$cols)*$src.Width/$cols)
        $x1=[int][math]::Round((($i%$cols)+1)*$src.Width/$cols)
        $row=[int][math]::Floor($i/$cols)
        $y0=[int][math]::Round($row*$src.Height/2)
        $y1=[int][math]::Round(($row+1)*$src.Height/2)
        $cell=$src.Clone([Drawing.Rectangle]::new($x0,$y0,$x1-$x0,$y1-$y0),[Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $minx=$cell.Width;$miny=$cell.Height;$maxx=-1;$maxy=-1
        for($y=0;$y -lt $cell.Height;$y++){for($x=0;$x -lt $cell.Width;$x++){if($cell.GetPixel($x,$y).A -gt 8){$minx=[math]::Min($minx,$x);$miny=[math]::Min($miny,$y);$maxx=[math]::Max($maxx,$x);$maxy=[math]::Max($maxy,$y)}}}
        if($maxx -lt 0){throw "Missing generated frame $($f.clip) $i"}
        # Only layout/cropping/resampling here; artwork is produced by image_gen.
        # Fit each generated frame to the original imported sprite's occupied rectangle.
        $canvas=[Drawing.Bitmap]::new(384,384,[Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $g=[Drawing.Graphics]::FromImage($canvas)
        $g.Clear([Drawing.Color]::Transparent)
        $g.InterpolationMode=[Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $g.PixelOffsetMode=[Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $dx=[int][math]::Round((48-[double]$f.pivotX)*4)
        $dy=[int][math]::Round((96-[double]$f.height+[double]$f.pivotY)*4)
        $dest=[Drawing.Rectangle]::new($dx,$dy,([int]$f.width*4),([int]$f.height*4))
        $g.DrawImage($cell,$dest,$minx,$miny,$maxx-$minx+1,$maxy-$miny+1,[Drawing.GraphicsUnit]::Pixel)
        $name=$f.clip+'_'+$i.ToString('D2')+'.png'
        $canvas.Save((Join-Path $output $name),[Drawing.Imaging.ImageFormat]::Png)
        $report += [pscustomobject]@{clip=$f.clip;index=$i;sourceWidth=$src.Width;sourceHeight=$src.Height;x=$dx;y=$dy;width=$dest.Width;height=$dest.Height;output=$name}
        $g.Dispose();$canvas.Dispose();$cell.Dispose()
    }
    $src.Dispose()
}
$report | Export-Csv -LiteralPath (Join-Path $PSScriptRoot 'Assembly.csv') -NoTypeInformation
Write-Output "Assembled $($report.Count) unique frames at 384x384."
