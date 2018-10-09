using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace Wnacg閱讀器
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try { RegisterUriScheme(); }
            catch (Exception ex)
            {
                if (Properties.Settings.Default.UriSchemeInstallErrorShowMessageBox)
                MessageBox.Show("無法註冊URL啟動功能至登入表，所以無法使用網址啟動功能(Chrome插件)\r\n" +
                    "若需要使用該功能，請以系統管理員身分執行此程式\r\n" +
                    "此訊息只會於註冊錯誤時顯示一次，後續如一樣無權限註冊則不會提示\r\n" +
                    "錯誤內容: " + ex.Message);
                Properties.Settings.Default.UriSchemeInstallErrorShowMessageBox = false;
                Properties.Settings.Default.Save();
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(Environment.GetCommandLineArgs().Length == 2 ? Environment.GetCommandLineArgs()[1] : ""));
        }

        const string UriScheme = "wnacg";
        const string FriendlyName = "Wnacg閱讀器";

        public static void RegisterUriScheme()
        {
            using (RegistryKey rootKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes\\" + UriScheme))
            {
                string applicationLocation = Application.ExecutablePath;
                if (rootKey == null)
                {
                    using (var key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Classes\\" + UriScheme))
                    {

                        key.SetValue("", "URL:" + FriendlyName);
                        key.SetValue("URL Protocol", "");

                        using (var defaultIcon = key.CreateSubKey("DefaultIcon"))
                        {
                            defaultIcon.SetValue("", applicationLocation + ",1");
                        }

                        using (var commandKey = key.CreateSubKey(@"shell\open\command"))
                        {
                            commandKey.SetValue("", "\"" + applicationLocation + "\" \"%1\"");
                        }
                    }
                    MessageBox.Show("已註冊網址啟動功能，未來如果有變更此程式的路徑會重新註冊(需要系統管理員功能)");
                }
                else
                {
                    using (RegistryKey commandKey = rootKey.OpenSubKey(@"shell\open\command", true))
                    {
                        if (commandKey == null)
                        {
                            Registry.LocalMachine.DeleteSubKey("SOFTWARE\\Classes\\" + UriScheme, false);
                            RegisterUriScheme();
                            return;
                        }
                        if (commandKey.GetValue("").ToString() != "\"" + applicationLocation + "\" \"%1\"")
                        {
                            commandKey.SetValue("", "\"" + applicationLocation + "\" \"%1\"");
                        }
                    }
                }
            }
        }
    }
}
