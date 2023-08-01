$sep = [IO.Path]::DirectorySeparatorChar;
[string[]]$architectures = 'osx-arm64','osx-x64','linux-x64','win-x64';

Remove-Item -Path (".{0}publish" -f $sep) -Force -Recurse

foreach ($a in $architectures)
{
    Write-Host "Publishing Architecture: $a";
    Invoke-Expression ("dotnet publish .{0}src{0}pfDataSource{0}pfDataSource.csproj -v q -c Release -r $a -p:PublishSingleFile=true --self-contained true -o .{0}publish{0}$a" -f $sep);
    Write-Host "Compressing: $a";
    $compress = @{
        Path = (".{0}publish{0}$a{0}*" -f $sep)
        CompressionLevel = "Optimal"
        DestinationPath = (".{0}publish{0}pfDataSource-$a.zip" -f $sep)
    };
    Compress-Archive @compress;
}