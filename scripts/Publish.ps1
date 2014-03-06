function Publish-Project($project) {
    Build-Project -project $project -params @{Configuration="Release"}; 
    nuget pack $project -Properties "Configuration=Release" -OutputDirectory "build";
}
New-Item "build" -Force -Type Directory;
Get-ChildItem -Filter *.csproj -Recurse | ? { $_.Name -ne "Tests.csproj"; } | % {
    Write-Host "Publish " $_.Name;
    Publish-Project $_.FullName;
}