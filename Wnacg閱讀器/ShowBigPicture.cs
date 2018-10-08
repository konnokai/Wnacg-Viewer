using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Wnacg閱讀器
{
    public partial class ShowBigPicture : Form
    {
        readonly string serverHost = "https://www.wnacg.org";
        string preViewURL = "", nextViewURL = "";

        void SetFormText(string text)
        {
            if (InvokeRequired) Invoke(new Action(delegate { Text = text; }));
            else Text = text;
        }

        public ShowBigPicture(string url)
        {
            InitializeComponent();

            SetFormData(url);
        }
        
        private void btn_Click(object sender, EventArgs e)
        {
            if (sender == btn_preView) SetFormData(preViewURL);
            else if (sender == btn_nextView) SetFormData(nextViewURL);
        }

        void SetFormData(string url)
        {
            new Thread(new ThreadStart(delegate
            {
                using (WebClient webClient = new WebClient())
                {
                    SetFormText("載入中...");

                    HtmlWeb htmlWeb = new HtmlWeb();
                    IEnumerable<HtmlNode> htmlNodeCollection = htmlWeb.Load(string.Format("{0}{1}", serverHost, url)).DocumentNode.Descendants();

                    string title = htmlNodeCollection.First((x) => x.Name == "title").InnerHtml;
                    SetFormText(title.Remove(title.Length - 26));

                    pictureBox1.Invoke(new Action(delegate
                    {
                        pictureBox1.InitialImage = Properties.Resources.loading2;
                        pictureBox1.ImageLocation = "https:" + htmlNodeCollection.First((x) => x.Name == "img" && x.Attributes.Any((x2) => x2.Name == "class" && x2.Value == "photo")).Attributes["src"].Value;
                    }));

                    txt_page.Invoke(new Action(delegate { txt_page.Text = htmlNodeCollection.Last((x) => x.Name == "span").InnerText; }));

                    IEnumerable<HtmlNode> htmlNodeCollection2 = htmlNodeCollection.Where((x) => x.Name == "a" && x.Attributes.Any((x2) => x2.Name == "class" && x2.Value == "btntuzao"));
                    foreach (var item in htmlNodeCollection)
                    {
                        if (item.Attributes.Contains("href"))
                        {
                            if (item.InnerText == "上一頁") preViewURL = item.Attributes["href"].Value;
                            else if (item.InnerText == "下一頁") nextViewURL = item.Attributes["href"].Value;
                        }
                    }
                }
                GC.Collect();
            })).Start();
        }
    }
}
