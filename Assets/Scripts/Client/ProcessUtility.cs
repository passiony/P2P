    using System;
    using System.Diagnostics;
    using Debug = UnityEngine.Debug;

    public class ProcessUtility
    {
        public static void OpenApp(string path)
        {
            try
            {
                // 设置应用程序的路径
                string appPath = path;

                // 启动应用程序
                Process.Start(appPath);
            }
            catch (Exception ex)
            {
                Debug.LogError("无法打开应用程序：" + ex.Message);
            }
        }
    }
