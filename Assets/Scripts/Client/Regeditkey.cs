using Microsoft.Win32;
using TMPro;
using UnityEngine;

public class Regeditkey : MonoBehaviour
{
    // public Button setupStartupButton;
    // public Button cancelStartupButton;
    public TextMeshProUGUI hintText;

    // private void OnEnable()
    // {
    //     string path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
    //     //regeditkey();
    //     setupStartupButton.onClick.AddListener(OnSetupStartupButtonClick);
    //     cancelStartupButton.onClick.AddListener(OnCancelStartupButtonClick);
    // }
    //
    // private void OnDisable()
    // {
    //     setupStartupButton.onClick.RemoveListener(OnSetupStartupButtonClick);
    //     cancelStartupButton.onClick.RemoveListener(OnCancelStartupButtonClick);
    // }

    private void Start()
    {
        OnSetupStartupButtonClick();
    }

    private void OnSetupStartupButtonClick()
    {
        // 提示，需要更改注册表
        try
        {
            string path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            RegistryKey rgkRun =
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rgkRun == null)
            {
                rgkRun = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            }

            rgkRun.SetValue("dhstest", path); // 名字请自行设置
        }
        catch
        {
            Debug.Log(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        }
        finally
        {
            regeditkey();
        }
    }

    private void OnCancelStartupButtonClick()
    {
        // 提示，需要更改注册表
        try
        {
            string path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            RegistryKey rgkRun =
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rgkRun == null)
            {
                rgkRun = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            }

            rgkRun.DeleteValue("dhstest", false);
        }
        catch
        {
            Debug.Log("error");
        }
        finally
        {
            regeditkey();
        }
    }


    private void regeditkey()
    {
        RegistryKey rgkRun =
            Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        if (rgkRun.GetValue("dhstest") == null)
        {
            hintText.text = "auto start";
        }
        else
        {
            hintText.text = "";
        }
    }
}