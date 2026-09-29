# LaunchDeck 启界

简体中文 · [English](README.md)

![LaunchDeck 标志](assets/logo.png)

LaunchDeck 是一款中英文双语的 Windows 软件启动中心。它把桌面和开始菜单中的快捷方式按功能分类，提供搜索、收藏、最近使用、快速启动、自定义分类和失效入口过滤。

## 功能

- 原生 C# / WPF 程序，解压即可使用。
- 界面可切换中文与英文。
- 本地目录缓存，启动后在后台刷新。
- 收藏和分类选择保存在当前 Windows 用户资料中。
- 快捷键：`Ctrl+F` 搜索，`Esc` 清空，搜索结果唯一时按 `Enter` 启动。
- 程序本身不发送遥测数据，也不发起网络请求。

## 系统要求

- Windows 10 或 Windows 11
- .NET Framework 4.8

## 下载与使用

从 [Releases](../../releases) 下载 `LaunchDeck-windows-v1.0.0.zip`，解压后运行 `LaunchDeck` 文件夹中的 `LaunchDeck.exe`。请保留程序旁边的 `Assets` 和 `Data` 文件夹。

发布包中的 `Data/apps.json` 默认为空，程序会自动读取你电脑上的快捷方式。如果某个软件没有快捷方式，可以编辑 `Data/apps.json` 手动添加；格式示例：

```json
[
  {"Name":"示例软件","Path":"C:/Program Files/Example/App.exe","Category":"其他工具","IconPath":null}
]
```

示例路径仅用于说明，请改成你电脑上的实际路径。目标文件不存在的条目会被自动忽略。

## 从源码构建

用 Visual Studio 2022 打开 `src/LaunchDeck/LaunchDeck.csproj` 并构建 Release，或在已安装 MSBuild 的环境中运行 `./build.ps1`。项目基于 .NET Framework 4.8，无 NuGet 运行时依赖。

## 数据与隐私

偏好设置保存在 `%APPDATA%\LaunchDeck\preferences.json`；软件目录和图标缓存在 `%LOCALAPPDATA%\LaunchDeck`。这些文件仅保存在你的电脑上，不包含在公开仓库或发布压缩包中。程序扫描当前用户及公用桌面、开始菜单快捷方式，以及可选的本地 `Data/apps.json`。

## 许可与安全

项目采用 MIT 许可证，详见 [LICENSE](LICENSE)。安全问题请参阅 [SECURITY.md](SECURITY.md)。
