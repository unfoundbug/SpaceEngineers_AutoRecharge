$newOutDir = Join-Path $env:APPDATA "\SpaceEngineers\Mods\UnFoundBug.AutoRecharge\"
if (Test-Path $newOutDir)
{
    Get-ChildItem "$newOutDir\*" -Recurse | Remove-Item
}
else
{
mkdir "$newOutDir"
}

Get-ChildItem .\* -File | Foreach-Object { Copy-Item -Path "$($_.FullName)" -Destination "$(Join-Path $newOutDir $($_.Name))" }

mkdir "$newOutDir\Data"
mkdir "$newOutDir\Data\Scripts"
mkdir "$newOutDir\Data\Scripts\AutoRecharge"
Get-ChildItem .\Data\*.sbc -File | Foreach-Object { Copy-Item -Path "$($_.FullName)" -Destination "$(Join-Path $newOutDir "Data" )"}
mkdir "$newOutDir\Data\Scripts"
mkdir "$newOutDir\Data\Scripts\AutoRecharge"
Get-ChildItem .\Data\Scripts\AutoRecharge\*.cs -File | Foreach-Object { Copy-Item -Path "$($_.FullName)" -Destination "$(Join-Path $newOutDir "Data\Scripts\AutoRecharge\" )"}