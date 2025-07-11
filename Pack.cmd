dotnet build --configuration Release
% dotnet build --configuration Debug %

nuget pack .\Nebulae.RimWorld.nuspec -OutputDirectory .\build

% start explorer.exe .\build %