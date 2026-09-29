using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace LaunchDeck {
  public static class Storage {
    public static readonly string Roaming=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"LaunchDeck");
    public static readonly string Local=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"LaunchDeck");
    public static readonly string SettingsPath=Path.Combine(Roaming,"preferences.json");
    public static readonly string CachePath=Path.Combine(Local,"catalog.xml");
    public static readonly string IconCache=Path.Combine(Local,"icons");
    static readonly JavaScriptSerializer Json=new JavaScriptSerializer{MaxJsonLength=16*1024*1024,RecursionLimit=64};

    public static UserSettings LoadSettings(){
      Directory.CreateDirectory(Roaming);Directory.CreateDirectory(Local);Directory.CreateDirectory(IconCache);
      var candidates=new[]{SettingsPath,SettingsPath+".lastgood",SettingsPath+".bak",Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"DesktopSoftwareCenter","preferences.json"),Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"DesktopSoftwareCenter","preferences.json")};
      foreach(var path in candidates){
        try{
          if(!File.Exists(path))continue;
          var s=Json.Deserialize<UserSettings>(File.ReadAllText(path,Encoding.UTF8));
          if(s==null)continue;Normalize(s);return s;
        }catch{}
      }
      return new UserSettings();
    }

    static void Normalize(UserSettings s){
      if(s.Favorites==null)s.Favorites=new List<string>();if(s.FavoriteKeys==null)s.FavoriteKeys=new List<string>();if(s.Recent==null)s.Recent=new List<string>();if(s.Categories==null)s.Categories=new Dictionary<string,string>();if(string.IsNullOrEmpty(s.Language))s.Language="zh-CN";
      s.Favorites=s.Favorites.Where(x=>!string.IsNullOrWhiteSpace(x)).Distinct().ToList();s.FavoriteKeys=s.FavoriteKeys.Where(x=>!string.IsNullOrWhiteSpace(x)).Distinct().ToList();s.Recent=s.Recent.Where(x=>!string.IsNullOrWhiteSpace(x)).Distinct().Take(20).ToList();
      if(s.Version<3)s.Version=3;
    }

    public static void SaveSettings(UserSettings s){
      Directory.CreateDirectory(Roaming);Normalize(s);var temp=SettingsPath+"."+Guid.NewGuid().ToString("N")+".tmp";
      File.WriteAllText(temp,Json.Serialize(s),new UTF8Encoding(true));
      if(File.Exists(SettingsPath)){
        try{File.Replace(temp,SettingsPath,SettingsPath+".bak",true);}catch{File.Copy(SettingsPath,SettingsPath+".bak",true);File.Copy(temp,SettingsPath,true);File.Delete(temp);}
      }else File.Move(temp,SettingsPath);
      File.Copy(SettingsPath,SettingsPath+".lastgood",true);
    }

    public static CatalogCache LoadCache(){
      try{if(!File.Exists(CachePath))return null;using(var s=File.OpenRead(CachePath))return (CatalogCache)new XmlSerializer(typeof(CatalogCache)).Deserialize(s);}catch{return null;}
    }
    public static void SaveCache(List<AppEntry> apps){
      Directory.CreateDirectory(Local);var temp=CachePath+".tmp";using(var s=File.Create(temp))new XmlSerializer(typeof(CatalogCache)).Serialize(s,new CatalogCache{Version=1,GeneratedUtc=DateTime.UtcNow,Apps=apps});if(File.Exists(CachePath))File.Delete(CachePath);File.Move(temp,CachePath);
    }
    public static List<ExtraApp> LoadExtras(string baseDir){
      try{var path=Path.Combine(baseDir,"Data","apps.json");return File.Exists(path)?Json.Deserialize<List<ExtraApp>>(File.ReadAllText(path,Encoding.UTF8)):new List<ExtraApp>();}catch{return new List<ExtraApp>();}
    }
  }
}
