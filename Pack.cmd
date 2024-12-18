@echo

dotnet build --configuration Release
dotnet build --configuration Debug

nuget pack .\Packages\Nebulae.RimWorld.nuspec -OutputDirectory .\buildedPackages