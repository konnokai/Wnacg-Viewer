using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Threading;

namespace Wnacg閱讀器
{
    public partial class DownloadBook : Form
    {
        bool isCancel = false;
        string savePath, bookID;
        AutoResetEvent evtDownload = new AutoResetEvent(false);

        void SetLabelText(string text)
        {
            if (label1.InvokeRequired) label1.Invoke(new Action(delegate { label1.Text = text; label1.Left = (ClientSize.Width - label1.Size.Width) / 2; }));
            else { label1.Text = text; label1.Left = (ClientSize.Width - label1.Size.Width) / 2; }
        }

        void SetProgressValue(int value)
        {
            if (progressBar1.InvokeRequired) progressBar1.Invoke(new Action(delegate { progressBar1.Value = value; }));
            else progressBar1.Value = value;
        }

        public DownloadBook(string savePath, string bookID)
        {
            InitializeComponent();
            this.savePath = savePath;
            this.bookID = bookID;
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            isCancel = true;
            btn_cancel.Enabled = false;
        }

        private void DownloadBook_Load(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(delegate
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadProgressChanged += (sender1, e1) => { SetProgressValue(e1.ProgressPercentage); };
                    webClient.DownloadFileCompleted += (sender1, e1) => { evtDownload.Set(); };

                    XmlSerializer serializer = new XmlSerializer(typeof(Rss));
                    MemoryStream memStream = new MemoryStream(webClient.DownloadData(string.Format("https://www.wnacg.com/feed-index-aid-{0}.html", bookID)));
                    Rss rss = (Rss)serializer.Deserialize(memStream);
                    string title = rss.Channel.Title.Remove(rss.Channel.Title.Length - 26);

                    savePath += string.Format("\\{0}-{1}", bookID.ToString(), title);

                    if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

                    for (int i = 0; i < rss.Channel.Item.Count; i++)
                    {
                        if (isCancel) break;

                        var item = rss.Channel.Item[i];
                        string url = "https:" + item.Description.Split(new char[] { '\"' })[1];

                        if (!File.Exists(savePath + "\\" + Path.GetFileName(url)))
                        {
                            try
                            {
                                SetLabelText(string.Format("{0} ({1}/{2})", title, i.ToString(), rss.Channel.Item.Count));
                                webClient.DownloadFileAsync(new Uri(url), savePath + "\\" + Path.GetFileName(url));
                                evtDownload.WaitOne();
                            }
                            catch (Exception) { }
                        }
                    }
                }

                Invoke(new Action(delegate { Close(); }));
            })).Start();
        }
    }
}
