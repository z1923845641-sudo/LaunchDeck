using System;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
namespace LaunchDeck {
  public static class Diagnostics {
    public static int SelfTest(string baseDir){
      try{var settings=Storage.LoadSettings();var apps=new CatalogService(baseDir).Scan(settings);var invalid=apps.Count(x=>!CatalogService.IsAvailable(x));var result=new{Apps=apps.Count,Invalid=invalid,Status=invalid==0?"PASS":"FAIL"};var json=new JavaScriptSerializer().Serialize(result);File.WriteAllText(Path.Combine(baseDir,"self-test.json"),json);Console.WriteLine(json);return invalid==0?0:2;}catch(Exception e){File.WriteAllText(Path.Combine(baseDir,"self-test-error.txt"),e.ToString());return 1;}}
  }
}
