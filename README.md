# Nebulae.RimWorld

## RimWorld 1.7 build

To build the UI assembly for the RimWorld 1.7 unstable branch:

```sh
dotnet build Nebulae.RimWorld.UI/Nebulae.RimWorld.UI.csproj -c RimWorld17 -p:RimWorldVersion=1.7
```

The 1.7 build runs startup quests on the main thread and provides its own `MouseButton` enum because Unity's UIElements enum is unavailable. Mods that directly use that enum through the UI library's public API must be rebuilt against the 1.7 assembly. The default build remains for earlier RimWorld versions.

## Known Issues

- Since change using language will not happen offen and many people use [Hugslib](https://github.com/UnlimitedHugs/RimworldHugsLib/) witch restart game, so I removed the feaure that respawn setting windows to fit current language.
- When an InputControl get focused, KeyDownEvent and KeyUpEvent will not work.

## Acknowledgements

- [__[.NET Platform]__](https://github.com/dotnet) - [WPF](https://github.com/dotnet/wpf)