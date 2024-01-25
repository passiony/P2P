 
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartMenu : MonoBehaviour
{
    // public Button setupStartupButton;
    // public Button cancelStartupButton;
    public TextMeshProUGUI hintText;
    private static string ShortcutName = "test.lnk";

    private void Start()
    {
        isStartup();
        OnSetupStartupButtonClick();
    }

    // private void OnEnable()
    // {
    //     isStartup();
    //     setupStartupButton.onClick.AddListener(OnSetupStartupButtonClick);
    //     cancelStartupButton.onClick.AddListener(OnCancelStartupButtonClick);
    // }
    //
    // private void OnDisable()
    // {
    //     setupStartupButton.onClick.RemoveListener(OnSetupStartupButtonClick);
    //     cancelStartupButton.onClick.RemoveListener(OnCancelStartupButtonClick);
    // }
 
    private void OnSetupStartupButtonClick()
    {
        CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Startup), ShortcutName, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        isStartup();
    }
 
    private void OnCancelStartupButtonClick()
    {
        if (System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + ShortcutName))
            System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + ShortcutName);
        isStartup();
    }
 
    private void isStartup()
    {
        if (System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + ShortcutName))
            hintText.text = "auto start";
        else
            hintText.text = "";
    }
 
    public static bool CreateShortcut(string direstory, string shortcurName, string targetPath, string description = null, string iconLocation = null)
    {
 
        try
        {
            if (!Directory.Exists(direstory))
            {
                Directory.CreateDirectory(direstory);
            }
 
            // 添加引用com中搜索Windows Script Host Object Model, 如果在unity中使用则需下载 Interop.IWshRuntimeLibrary.dll 并放到代码同一文件夹
            string shortscurPath = Path.Combine(direstory, string.Format("{0}", shortcurName));
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortscurPath); // 创建快捷方式对象
            shortcut.TargetPath = targetPath; // 指定目标路径
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath); //设置起始位置
            shortcut.WindowStyle = 1; // 设置运行方式，默认为常规窗口
            shortcut.Description = description; // 设置备注
            shortcut.IconLocation = string.IsNullOrEmpty(iconLocation) ? targetPath : iconLocation; //设置图标路径
            shortcut.Save(); // 保存快捷方式
            return true;
        }
        catch (Exception)
        {
            // ignored
        }

        return false;
    }
}