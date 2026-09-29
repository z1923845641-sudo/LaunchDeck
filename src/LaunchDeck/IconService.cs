using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace LaunchDeck {
  public static class IconService {
    [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Unicode)]struct SHFILEINFO{public IntPtr hIcon;public int iIcon;public uint dwAttributes;[MarshalAs(UnmanagedType.ByValTStr,SizeConst=260)]public string szDisplayName;[MarshalAs(UnmanagedType.ByValTStr,SizeConst=80)]public string szTypeName;}
    [DllImport("shell32.dll",CharSet=CharSet.Unicode)]static extern IntPtr SHGetFileInfo(string path,uint attrs,ref SHFILEINFO info,uint size,uint flags);
    [DllImport("user32.dll")]static extern bool DestroyIcon(IntPtr handle);
    static readonly Dictionary<string,BitmapSource> Memory=new Dictionary<string,BitmapSource>();
    public static BitmapSource Get(AppEntry app){
      BitmapSource found;if(Memory.TryGetValue(app.Id,out found))return found;
      var cache=Path.Combine(Storage.IconCache,app.Id+".png");if(File.Exists(cache)){try{return Memory[app.Id]=Load(cache);}catch{}}
      var source=!string.IsNullOrWhiteSpace(app.IconPath)&&File.Exists(app.IconPath)?app.IconPath:(File.Exists(app.Target)?app.Target:app.Path);
      try{
        if(source.EndsWith(".png",StringComparison.OrdinalIgnoreCase))found=Load(source);else{var info=new SHFILEINFO();SHGetFileInfo(source,0,ref info,(uint)Marshal.SizeOf(info),0x100|0x0);if(info.hIcon!=IntPtr.Zero){try{found=Imaging.CreateBitmapSourceFromHIcon(info.hIcon,Int32Rect.Empty,BitmapSizeOptions.FromWidthAndHeight(64,64));found.Freeze();}finally{DestroyIcon(info.hIcon);}}}
        if(found!=null){Memory[app.Id]=found;try{var e=new PngBitmapEncoder();e.Frames.Add(BitmapFrame.Create(found));using(var s=File.Create(cache))e.Save(s);}catch{}}
      }catch{}
      return found;
    }
    public static BitmapSource Load(string path){var b=new BitmapImage();b.BeginInit();b.CacheOption=BitmapCacheOption.OnLoad;b.UriSource=new Uri(Path.GetFullPath(path));b.EndInit();b.Freeze();return b;}
  }
}
