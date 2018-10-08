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
            RegisterUriScheme();
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
