using System;
using System.Collections.Generic;

namespace LaunchDeck {
  [Serializable]
  public class AppEntry {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string Path { get; set; }
    public string Target { get; set; }
    public string Arguments { get; set; }
    public string IconPath { get; set; }
    public string StableKey { get; set; }
  }

  public class UserSettings {
    public int Version { get; set; }
    public List<string> Favorites { get; set; }
    public List<string> FavoriteKeys { get; set; }
    public List<string> Recent { get; set; }
    public Dictionary<string,string> Categories { get; set; }
    public bool ReduceMotion { get; set; }
    public string Language { get; set; }
    public UserSettings(){Version=3;Favorites=new List<string>();FavoriteKeys=new List<string>();Recent=new List<string>();Categories=new Dictionary<string,string>();Language="zh-CN";}
  }

  public class CatalogCache {
    public int Version { get; set; }
    public DateTime GeneratedUtc { get; set; }
    public List<AppEntry> Apps { get; set; }
  }

  public class ExtraApp {
    public string Name { get; set; }
    public string Path { get; set; }
    public string Category { get; set; }
    public string IconPath { get; set; }
  }
}
