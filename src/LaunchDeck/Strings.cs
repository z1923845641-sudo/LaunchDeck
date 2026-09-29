using System.Collections.Generic;
namespace LaunchDeck {
  public static class L {
    static readonly Dictionary<string,string[]> S=new Dictionary<string,string[]>{
      {"tag",new[]{"把软件装进秩序里","Your software, in order."}},{"all",new[]{"全部软件","All apps"}},{"fav",new[]{"我的收藏","Favorites"}},{"recent",new[]{"最近使用","Recent"}},{"search",new[]{"搜索软件名称…","Search apps…"}},{"refresh",new[]{"刷新","Refresh"}},{"quick",new[]{"快速启动","Quick launch"}},{"motion",new[]{"减少动画","Reduce motion"}},{"launch",new[]{"启动","Launch"}},{"favorite",new[]{"收藏","Favorite"}},{"favorited",new[]{"已收藏","Saved"}},{"move",new[]{"移动到分类","Move to category"}},{"folder",new[]{"打开所在位置","Open location"}},{"loading",new[]{"正在整理软件…","Organizing apps…"}},{"ready",new[]{"已读取 {0} 款软件 · 失效入口已过滤","{0} apps indexed · broken entries filtered"}},{"empty",new[]{"这里还没有软件","Nothing here yet"}},{"emptyhelp",new[]{"换个分类或搜索词试试","Try another category or search"}},{"launched",new[]{"已发送启动请求：{0}","Launch request sent: {0}"}},{"failed",new[]{"启动失败，请刷新后重试","Could not launch. Refresh and try again."}},{"settings",new[]{"偏好","Preferences"}},{"brand",new[]{"启界","LaunchDeck"}}
    };
    public static bool En(string language){return language=="en-US";}
    public static string T(string key,string language){return S[key][En(language)?1:0];}
    public static string Category(string category,string language){if(!En(language))return category;var map=new Dictionary<string,string>{{"游戏与游戏平台","Games & platforms"},{"办公与学习","Work & learning"},{"设计与影音制作","Creative studio"},{"编程与数据分析","Development & data"},{"浏览器","Browsers"},{"聊天通讯","Communication"},{"影音娱乐","Media"},{"网盘与下载","Cloud & downloads"},{"网络与远程工具","Network & remote"},{"系统与外设工具","System & devices"},{"其他工具","Utilities"}};return map.ContainsKey(category)?map[category]:category;}
  }
}
