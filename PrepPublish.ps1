param([string] $buildType)

Write-Host "Cleaning existing item"
$newOutDir = Join-Path $env:APPDATA "\SpaceEngineers\Mods\UnFoundBug.AutoRecharge\"
Remove-Item -LiteralPath "$newOutDir" -Recurse -Force

$nDir = mkdir "$newOutDir"

Write-Host "Copying Bulk"
Get-ChildItem .\* -File | Foreach-Object { Copy-Item -Path "$($_.FullName)" -Destination "$(Join-Path $newOutDir $($_.Name))" }

Write-Host "Copying Scripts"

$nDir = mkdir "$newOutDir\Data"
$nDir = mkdir "$newOutDir\Data\Scripts"
$nDir = mkdir "$newOutDir\Data\Scripts\AutoRecharge"
Get-ChildItem .\Data\*.sbc -File | Foreach-Object { Copy-Item -Path "$($_.FullName)" -Destination "$(Join-Path $newOutDir "Data" )"}
Get-ChildItem .\Data\Scripts\AutoRecharge\*.cs -File | Foreach-Object { Copy-Item -Path "$($_.FullName)" -Destination "$(Join-Path $newOutDir "Data\Scripts\AutoRecharge\" )"}

Write-Host "Activating build info"
if ($buildType -eq "Debug") {
	Write-Host "using debug mod info";
	remove-item "$(Join-Path $newOutDir "modinfo_Release.sbmi")"
} else {
	Write-Host "using release mod info";
	remove-item "$(Join-Path $newOutDir "modinfo.sbmi")"
	move-item "$(Join-Path $newOutDir "modinfo_Release.sbmi")" "$(Join-Path $newOutDir "modinfo.sbmi")"
}
