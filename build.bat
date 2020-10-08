rmdir /s /q .\bin
dotnet clean .\src\otpcli.csproj
dotnet publish .\src\otpcli.csproj -r win-x64 -c Release -p:PublishSingleFile=true --self-contained true --output bin