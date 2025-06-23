// #if NET_4_6
// using System;
// using System.Collections.Generic;
// using System.Drawing;
// using System.Windows.Forms;
// using NonsensicalKit.Core;
// using NonsensicalKit.Core.Log;
//
// namespace NonsensicalKit.Windows.Tray
// {
//     public class TrayIconMenuInfo
//     {
//         public string Path;
//         public EventHandler Callback;
//     }
//
//     /// <summary>
//     /// 系统托盘图标修改，由于使用了System.Drawing，所以必须使用.net framework框架
//     /// 需要导入System.Windows.Forms.dll和System.Drawing.dll
//     /// 可在路径：Unity2018安装目录/Editor\Data\Mono\lib\mono\2.0里找到
//     ///
//     /// 目前使用会出现崩溃的情况，因为Forms在unity中使用不兼容，应改为直接使用winapi的方式加载
//     /// </summary>
//     public class TrayIconModifier
//     {
//         //NotifyIcon 设置托盘相关参数
//         private NotifyIcon _notifyIcon;
//
//         private IntPtr _currentWindowPtr;
//
//         public Action OnClick;
//
//         public TrayIconModifier(string text, string iconPath)
//         {
//             if (!PlatformInfo.IsEditor && PlatformInfo.IsWindow)
//             {
//                 _notifyIcon = new();
//                 _notifyIcon.Text = text;
//                 _notifyIcon.Icon = CustomTrayIcon(iconPath);
//                 _notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick; //双击托盘图标响应事件
//
//                 ShowTray();
//             }
//         }
//
//         ~TrayIconModifier()
//         {
//             Dispose();
//         }
//
//         public void SetMenu(IEnumerable<TrayIconMenuInfo> infos)
//         {
//             if (!PlatformInfo.IsEditor && PlatformInfo.IsWindow)
//             {
//                 if (_notifyIcon != null)
//                 {
//                     Dictionary<string, ToolStripMenuItem> items = new();
//                     List<ToolStripMenuItem> tops = new();
//                     foreach (var info in infos)
//                     {
//                         string[] paths = info.Path.Split('\\');
//                         var crtPath = string.Empty;
//                         ToolStripMenuItem parent = null;
//                         foreach (var p in paths)
//                         {
//                             crtPath += p + "\\";
//                             ToolStripMenuItem crt = null;
//                             if (items.TryGetValue(crtPath, out var item))
//                             {
//                                 crt = item;
//                             }
//                             else
//                             {
//                                 crt = new ToolStripMenuItem();
//                                 crt.Text = p;
//                                 items.Add(crtPath, crt);
//                             }
//
//                             if (parent == null)
//                             {
//                                 if (tops.Contains(crt) == false)
//                                 {
//                                     tops.Add(crt);
//                                 }
//                             }
//                             else
//                             {
//                                 parent.DropDownItems.Add(crt);
//                             }
//
//                             parent = crt;
//                         }
//
//                         if (parent == null)
//                         {
//                             LogCore.Error($"路径 {info.Path} 有误");
//                         }
//                         else
//                         {
//                             parent.Click += info.Callback;
//                         }
//                     }
//
//                     _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
//                     foreach (var top in tops)
//                     {
//                         _notifyIcon.ContextMenuStrip.Items.Add(top);
//                     }
//                 }
//             }
//         }
//
//         public void ShowTray()
//         {
//             if (!PlatformInfo.IsEditor && PlatformInfo.IsWindow)
//             {
//                 if (_notifyIcon != null)
//                 {
//                     _notifyIcon.Visible = true;
//                 }
//             }
//         }
//
//         public void HideTray()
//         {
//             if (!PlatformInfo.IsEditor && PlatformInfo.IsWindow)
//             {
//                 if (_notifyIcon != null)
//                 {
//                     _notifyIcon.Visible = false;
//                 }
//             }
//         }
//
//         public void Dispose()
//         {
//             if (!PlatformInfo.IsEditor && PlatformInfo.IsWindow)
//             {
//                 if (_notifyIcon != null)
//                 {
//                     HideTray();
//                     _notifyIcon.Icon = null;
//                     _notifyIcon.Dispose();
//                     _notifyIcon = null;
//                 }
//             }
//         }
//
//         private Icon CustomTrayIcon(string iconPath)
//         {
//             Bitmap bt = new Bitmap(iconPath);
//             Bitmap fitSizeBt = new Bitmap(bt, 32, 32);
//             return Icon.FromHandle(fitSizeBt.GetHicon());
//         }
//
//         private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
//         {
//             if (e.Button == MouseButtons.Left)
//             {
//                 OnClick?.Invoke();
//             }
//         }
//     }
// }
// #endif
