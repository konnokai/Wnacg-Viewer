using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using System.Threading;

namespace Wnacg閱讀器
{
    class ButtonWithWebImage : Button
    {
        private bool isDispose = false;

        public ButtonWithWebImage(string url)
        {
            new Thread(new ThreadStart(delegate
            {
                using (WebClient webClient = new WebClient())
                {
                    Image image = null;

                    try
                    {
                        image = Image.FromStream(webClient.OpenRead(url));
                    }
                    catch (Exception) { }

                    if (!isDispose) Invoke(new Action(delegate { BackgroundImage = image != null ? image : Properties.Resources.error; }));
                }
            })).Start();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            isDispose = true;
        }
    }
}
