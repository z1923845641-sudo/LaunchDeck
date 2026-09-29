using System;
using System.IO;
using System.Threading;
using System.Windows;

namespace LaunchDeck {
  public class App : Application {
    [STAThread]public static int Main(string[] args){
      var baseDir=AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
      if(Array.IndexOf(args,"--self-test")>=0)return Diagnostics.SelfTest(baseDir);
      bool created;using(var mutex=new Mutex(true,"Local\\LaunchDeck-"+Environment.UserName,out created)){
        if(!created)return 0;
        var app=new App();app.ShutdownMode=ShutdownMode.OnMainWindowClose;return app.Run(new MainWindow(baseDir));
      }
    }
  }
}
