$ErrorActionPreference = 'Stop'
$projectRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '../..')).Path
$mapRoot = Join-Path $projectRoot 'Assets/MapReview'
if (-not $mapRoot.StartsWith($projectRoot + [IO.Path]::DirectorySeparatorChar)) { throw 'Invalid source root' }
$bossRoot = Join-Path $projectRoot 'Assets/04.Sprites/Boss'
$bossBefore = @(Get-ChildItem -LiteralPath $bossRoot -Recurse -File | ForEach-Object { [PSCustomObject]@{ Path=$_.FullName; Hash=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash } })
$moves = [Collections.Generic.List[object]]::new()
function Plan-File([string]$source,[string]$destination) {
    if (Test-Path -LiteralPath (Join-Path $projectRoot $destination)) { throw "Destination exists: $destination" }
    $moves.Add([PSCustomObject]@{Source=$source; Destination=$destination})
}
Get-ChildItem -LiteralPath (Join-Path $mapRoot 'Textures') -File | Where-Object Extension -ne '.meta' | ForEach-Object {
    Plan-File ('Assets/MapReview/Textures/'+$_.Name) ('Assets/04.Sprites/Backgrounds/SharedTextures/'+$_.Name)
}
Get-ChildItem -LiteralPath (Join-Path $mapRoot 'Prefabs') -File -Filter '*.prefab' | ForEach-Object {
    Plan-File ('Assets/MapReview/Prefabs/'+$_.Name) ('Assets/03.Prefabs/Map/'+$_.Name)
}
Get-ChildItem -LiteralPath (Join-Path $mapRoot 'Runtime') -File | Where-Object Extension -ne '.meta' | ForEach-Object {
    $target = if ($_.Extension -eq '.shader') {'Assets/08.Materials/Map/Shaders/'} elseif ($_.BaseName -eq 'ReviewPlayer') {'Assets/02.Scripts/Map/Testing/'} else {'Assets/02.Scripts/Map/Runtime/'}
    Plan-File ('Assets/MapReview/Runtime/'+$_.Name) ($target+$_.Name)
}
Plan-File 'Assets/MapReview/Editor/ReviewArtInspector.cs' 'Assets/02.Scripts/Map/Editor/ReviewArtInspector.cs'
# Include geometry used by shared scenes/prefabs, including inactive preserved art.
$sharedFiles = @(Get-ChildItem -LiteralPath (Join-Path $projectRoot 'Assets') -Recurse -File | Where-Object { $_.Extension -in '.unity','.prefab' -and (-not $_.FullName.StartsWith($mapRoot+[IO.Path]::DirectorySeparatorChar) -or $_.DirectoryName -eq (Join-Path $mapRoot 'Prefabs')) })
$referenced = [Collections.Generic.HashSet[string]]::new()
foreach ($file in $sharedFiles) { foreach ($match in [regex]::Matches([IO.File]::ReadAllText($file.FullName),'guid: ([0-9a-f]{32})')) { [void]$referenced.Add($match.Groups[1].Value) } }
Get-ChildItem -LiteralPath $mapRoot -File -Filter '*.json' | ForEach-Object {
    $meta=[IO.File]::ReadAllText($_.FullName+'.meta'); $guid=[regex]::Match($meta,'guid: ([0-9a-f]{32})').Groups[1].Value
    if ($referenced.Contains($guid)) { Plan-File ('Assets/MapReview/'+$_.Name) ('Assets/07.Datas/Map/Geometry/'+$_.Name) }
}
$moves | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $PSScriptRoot 'SharedMap_v12_moves.json') -Encoding utf8
foreach ($move in $moves) {
    $source=[IO.Path]::GetFullPath((Join-Path $projectRoot $move.Source));$target=[IO.Path]::GetFullPath((Join-Path $projectRoot $move.Destination))
    if (-not $source.StartsWith($mapRoot+[IO.Path]::DirectorySeparatorChar) -or -not $target.StartsWith($projectRoot+[IO.Path]::DirectorySeparatorChar) -or $target.StartsWith($bossRoot)) { throw 'Move boundary check failed' }
    [IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($target)) | Out-Null
    Move-Item -LiteralPath $source -Destination $target
    Move-Item -LiteralPath ($source+'.meta') -Destination ($target+'.meta')
}
# Legacy local editor utilities keep their literal file references usable after relocation.
Get-ChildItem -LiteralPath (Join-Path $mapRoot 'Editor') -Filter '*.cs' -File | ForEach-Object {
    $text=[IO.File]::ReadAllText($_.FullName)
    foreach($move in $moves){
        $text=$text.Replace($move.Source,$move.Destination)
        $relative=$move.Source.Substring('Assets/MapReview/'.Length)
        $text=$text.Replace('Folder+"'+$relative+'"','"'+$move.Destination+'"')
        $text=$text.Replace('Folder + "'+$relative+'"','"'+$move.Destination+'"')
    }
    [IO.File]::WriteAllText($_.FullName,$text,[Text.UTF8Encoding]::new($false))
}
$bossAfter = @(Get-ChildItem -LiteralPath $bossRoot -Recurse -File)
if ($bossAfter.Count -ne $bossBefore.Count) { throw 'Boss file count changed' }
foreach ($item in $bossBefore) { if ((Get-FileHash -LiteralPath $item.Path -Algorithm SHA256).Hash -ne $item.Hash) { throw "Boss changed: $($item.Path)" } }
"Moved $($moves.Count) assets with original meta GUIDs. Boss $($bossBefore.Count) files unchanged." | Set-Content -LiteralPath (Join-Path $PSScriptRoot 'SharedMap_v12_move_check.txt') -Encoding utf8
