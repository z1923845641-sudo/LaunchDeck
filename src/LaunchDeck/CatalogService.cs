using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace LaunchDeck {
  public class CatalogService {
    public static readonly string[] Categories={"游戏与游戏平台","办公与学习","设计与影音制作","编程与数据分析","浏览器","聊天通讯","影音娱乐","网盘与下载","网络与远程工具","系统与外设工具","其他工具"};
    readonly string _baseDir;
    public CatalogService(string baseDir){_baseDir=baseDir;}

    public List<AppEntry> LoadFast(UserSettings settings){
      var cache=Storage.LoadCache();
      if(cache==null||cache.Apps==null)return Scan(settings);
      var apps=cache.Apps.Where(IsAvailable).ToList();ApplySettings(apps,settings);return apps;
    }

    public List<AppEntry> Scan(UserSettings settings){
      var result=new List<AppEntry>();var seen=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      var roots=new[]{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu)};
      dynamic shell=null;try{shell=Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell"));}catch{}
      foreach(var root in roots){
        if(string.IsNullOrEmpty(root)||!Directory.Exists(root))continue;
        foreach(var file in EnumerateLaunchers(root)){
          try{
            if(Path.GetFileName(file).Equals("软件启动中心.lnk",StringComparison.OrdinalIgnoreCase)||Path.GetFileName(file).Equals("LaunchDeck.lnk",StringComparison.OrdinalIgnoreCase))continue;
            string target="",args="";
            if(file.EndsWith(".url",StringComparison.OrdinalIgnoreCase)){var line=File.ReadLines(file).FirstOrDefault(x=>x.StartsWith("URL=",StringComparison.OrdinalIgnoreCase));if(line!=null)target=line.Substring(4);}
            else if(shell!=null){dynamic shortcut=shell.CreateShortcut(file);target=(string)shortcut.TargetPath;args=(string)shortcut.Arguments;}
            var name=Regex.Replace(Path.GetFileNameWithoutExtension(file),@"\s*-\s*快捷方式$","");
            if(string.IsNullOrWhiteSpace(target)||Skip(name)||(!file.EndsWith(".url",StringComparison.OrdinalIgnoreCase)&&Directory.Exists(target)))continue;
            var identity=Identity(target,args);if(!seen.Add(identity))continue;
            var app=Create(name,file,target,args,null);if(IsAvailable(app))result.Add(app);
          }catch{}
        }
      }
      foreach(var extra in Storage.LoadExtras(_baseDir)){
        if(extra==null||string.IsNullOrWhiteSpace(extra.Path))continue;
        var identity=Identity(extra.Path,"");if(!seen.Add(identity))continue;
        var icon=extra.IconPath;if(!string.IsNullOrWhiteSpace(icon)&&!Path.IsPathRooted(icon))icon=Path.Combine(_baseDir,icon);
        var app=Create(extra.Name,extra.Path,extra.Path,"",icon);app.Category=string.IsNullOrWhiteSpace(extra.Category)?Classify(app.Name,app.Path,app.Target):extra.Category;if(IsAvailable(app))result.Add(app);
      }
      result=result.GroupBy(x=>x.Id).Select(x=>x.First()).OrderBy(x=>x.Name,StringComparer.CurrentCultureIgnoreCase).ToList();ApplySettings(result,settings);Storage.SaveCache(result);return result;
    }

    static AppEntry Create(string name,string path,string target,string args,string icon){
      var id=Hash(Identity(target,args)).Substring(0,24);return new AppEntry{Id=id,Name=name,Path=path,Target=target,Arguments=args??"",IconPath=icon,Category=Classify(name,path,target),StableKey=Stable(name,target,args)};
    }
    static string Identity(string target,string args){var value=(target??"").Replace('/','\\');try{if(Path.IsPathRooted(value))value=Path.GetFullPath(value).TrimEnd('\\');}catch{}return (value+"|"+(args??"")).ToLowerInvariant();}
    public static string Stable(string name,string target,string args){
      if((target??"").StartsWith("shell:AppsFolder\\",StringComparison.OrdinalIgnoreCase))return target.ToLowerInvariant();
      var canonical=Regex.Replace((target??"").Replace('/','\\'),@"\\(?:app-)?v?\d+(?:\.\d+){1,4}(?:[-_][^\\]*)?(?=\\)",@"\{version}",RegexOptions.IgnoreCase);return ((name??"").Trim()+"|"+canonical+"|"+(args??"")).ToLowerInvariant();
    }
    public static void ApplySettings(List<AppEntry> apps,UserSettings s){
      foreach(var app in apps){if(string.IsNullOrEmpty(app.StableKey))app.StableKey=Stable(app.Name,app.Target,app.Arguments);string category;if(s.Categories.TryGetValue(app.Id,out category))app.Category=category;if(s.Favorites.Contains(app.Id)&&!s.FavoriteKeys.Contains(app.StableKey))s.FavoriteKeys.Add(app.StableKey);}
      s.Favorites=apps.Where(a=>s.FavoriteKeys.Contains(a.StableKey)).Select(a=>a.Id).Distinct().ToList();
    }
    public static bool IsAvailable(AppEntry app){if(app==null||string.IsNullOrWhiteSpace(app.Target))return false;if(app.Target.StartsWith("shell:AppsFolder\\",StringComparison.OrdinalIgnoreCase))return true;if(app.Target.StartsWith("http",StringComparison.OrdinalIgnoreCase)||app.Target.StartsWith("steam:",StringComparison.OrdinalIgnoreCase)||app.Target.StartsWith("com.epicgames",StringComparison.OrdinalIgnoreCase))return File.Exists(app.Path);return File.Exists(app.Path)&&File.Exists(app.Target);}
    static string Hash(string value){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-","");}
    static IEnumerable<string> EnumerateLaunchers(string root){
      var pending=new Stack<string>();pending.Push(root);
      while(pending.Count>0){
        var dir=pending.Pop();string[] files=new string[0],dirs=new string[0];
        try{files=Directory.GetFiles(dir);dirs=Directory.GetDirectories(dir);}catch{}
        foreach(var file in files)if(file.EndsWith(".lnk",StringComparison.OrdinalIgnoreCase)||file.EndsWith(".url",StringComparison.OrdinalIgnoreCase))yield return file;
        foreach(var child in dirs){try{var a=File.GetAttributes(child);if((a&FileAttributes.ReparsePoint)==0)pending.Push(child);}catch{}}
      }
    }
    static bool Skip(string n){return Regex.IsMatch(n,@"卸载|uninstall|unins|readme|帮助|documentation|reset spyder|release notes|updater|error reporter|许可证|repair|environment",RegexOptions.IgnoreCase);}
    static string Classify(string n,string p,string t){
      if(Regex.IsMatch(n,@"WeGame|游戏|Steam|Epic|Ubisoft|Rockstar|^EA$|MuMu|PUBG|Counter|战地|我的世界|Among Us|启动游戏",RegexOptions.IgnoreCase)||Regex.IsMatch(t??"",@"^steam:|^com.epicgames",RegexOptions.IgnoreCase))return Categories[0];
      if(Regex.IsMatch(n,@"WPS|PDF|有道|学习|WordWing|^Word$|Excel|PowerPoint|OneNote|Outlook|ChatGPT",RegexOptions.IgnoreCase))return Categories[1];
      if(Regex.IsMatch(n,@"Canva|美图|Canvas|Bambu|Topaz|OBS|剪映|Adobe|Cinema|Color",RegexOptions.IgnoreCase))return Categories[2];
      if(Regex.IsMatch(n,@"TraceMemo|SPSS|Gephi|Spyder|PyCharm|Visual Studio|Git |Python|Node.js|Ollama|WSL|Ubuntu",RegexOptions.IgnoreCase))return Categories[3];
      if(Regex.IsMatch(n,@"Chrome|Edge|Firefox|浏览器|夸克",RegexOptions.IgnoreCase))return Categories[4];
      if(Regex.IsMatch(n,@"^微信|^QQ$|Telegram|Discord",RegexOptions.IgnoreCase))return Categories[5];
      if(Regex.IsMatch(n,@"播放器|影音|PotPlayer|抖音|虎牙|kuwo|音乐",RegexOptions.IgnoreCase))return Categories[6];
      if(Regex.IsMatch(n,@"网盘|迅雷|download|OneDrive",RegexOptions.IgnoreCase))return Categories[7];
      if(Regex.IsMatch(n,@"加速器|v2ray|Clash|远程|Remote Desktop",RegexOptions.IgnoreCase))return Categories[8];
      if(Regex.IsMatch(n,@"驱动|压缩|工具箱|MD5|Logitech|ugee|7-Zip|NVIDIA|PowerShell",RegexOptions.IgnoreCase)||Regex.IsMatch(t??"",@"^C:\\Windows\\",RegexOptions.IgnoreCase))return Categories[9];
      return Categories[10];
    }
  }
}
