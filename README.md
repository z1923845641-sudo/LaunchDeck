# LaunchDeck

[简体中文](README.zh-CN.md) · English

![LaunchDeck logo](assets/logo.png)

LaunchDeck is a bilingual Windows app launcher. It groups shortcuts from your Desktop and Start Menu by function and provides search, favorites, recent apps, quick launch, category overrides, and filtering for broken shortcuts.

## Features

- Native C# / WPF app with a portable folder layout.
- Chinese and English interface.
- Local catalog cache with background refresh.
- Favorites and category choices saved to your Windows profile.
- Keyboard shortcuts: `Ctrl+F` to search, `Esc` to clear, and `Enter` to launch a single search result.
- No telemetry or network requests from the app.

## Requirements

- Windows 10 or Windows 11
- .NET Framework 4.8

## Download and run

Download `LaunchDeck-windows-v1.0.0.zip` from [Releases](../../releases), extract it, and run `LaunchDeck.exe` from the extracted `LaunchDeck` folder. Keep the `Assets` and `Data` folders beside the executable.

The included `Data/apps.json` is empty. LaunchDeck discovers shortcuts on your own computer. To add an app that does not have a shortcut, edit `Data/apps.json` using `assets/apps.json` as the starting point. Example:

```json
[
  {"Name":"Example App","Path":"C:/Program Files/Example/App.exe","Category":"其他工具","IconPath":null}
]
```

The path in this example is illustrative; replace it with a path on your computer. Custom entries with missing targets are ignored.

## Build from source

Open `src/LaunchDeck/LaunchDeck.csproj` in Visual Studio 2022 and build Release, or run `./build.ps1` with MSBuild installed. The app targets .NET Framework 4.8 and has no NuGet runtime dependency.

## Data and privacy

Preferences are stored at `%APPDATA%\LaunchDeck\preferences.json`. The catalog and icon cache are stored under `%LOCALAPPDATA%\LaunchDeck`. These files stay on your computer and are excluded from this repository and the release ZIP. LaunchDeck scans current and public Desktop and Start Menu shortcuts, plus the optional local `Data/apps.json`.

## License and security

MIT license. See [LICENSE](LICENSE). For security reports, see [SECURITY.md](SECURITY.md).
