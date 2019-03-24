using HtmlAgilityPack;
using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace Wnacg閱讀器
{
    public partial class DownloadBook : Form
    {
        readonly string serverHost = "https://www.wnacg.org";
        bool isCancel = false;
        string savePath, bookID;
        List<string> listImageViewUrl;
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

        public DownloadBook(string savePath, string bookID, List<string> listImageViewUrl)
        {
            InitializeComponent();
            this.savePath = savePath;
            this.bookID = bookID;
            this.listImageViewUrl = listImageViewUrl;
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
                    SetLabelText("讀取資料中...");

                    webClient.DownloadProgressChanged += (sender1, e1) => { SetProgressValue(e1.ProgressPercentage); };
                    webClient.DownloadFileCompleted += (sender1, e1) => { evtDownload.Set(); };

                    XmlSerializer serializer = new XmlSerializer(typeof(Rss));
                    MemoryStream memStream = new MemoryStream(webClient.DownloadData(string.Format("https://www.wnacg.com/feed-index-aid-{0}.html", bookID)));
                    Rss rss = (Rss)serializer.Deserialize(memStream);
                    string title = rss.Channel.Title.Remove(rss.Channel.Title.Length - 26);

                    savePath += string.Format("\\{0}-{1}", bookID.ToString(), title);

                    if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
                    else SetLabelText("確認已下載的頁數中...");

                    if (listImageViewUrl.Count <= 60)
                    {
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
                    else
                    {
                        HtmlWeb htmlWeb = new HtmlWeb(); int i = 1;

                        foreach (string item in listImageViewUrl)
                        {
                            if (isCancel) break;

                            IEnumerable<HtmlNode> htmlDocumentNode = htmlWeb.Load(string.Format("{0}{1}", serverHost, item)).DocumentNode.Descendants();
                            string url= "https:" + htmlDocumentNode.First((x) => x.Name == "img" && x.Attributes.Any((x2) => x2.Name == "class" && x2.Value == "photo")).Attributes["src"].Value;

                            if (!File.Exists(savePath + "\\" + Path.GetFileName(url)))
                            {
                                try
                                {
                                    SetLabelText(string.Format("{0} ({1}/{2})", title, i.ToString(), listImageViewUrl.Count));
                                    webClient.DownloadFileAsync(new Uri(url), savePath + "\\" + Path.GetFileName(url));
                                    evtDownload.WaitOne();
                                }
                                catch (Exception) { }
                            }

                            i++;
                        }
                    }                    
                }

                Invoke(new Action(delegate { Close(); }));
            })).Start();
        }
    }
}
