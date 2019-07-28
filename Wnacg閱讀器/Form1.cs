using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace Wnacg閱讀器
{
    public partial class Form1 : Form
    {
        void SetFormText(string text)
        {
            text = "Wnacg閱讀器 By 孤之界 | " + text;
            if (InvokeRequired) Invoke(new Action(delegate { Text = text; }));
            else Text = text;
        }

        List<string> listImageViewUrl = new List<string>();
        WebClient webClient = new WebClient();
        readonly string serverHost = "https://www.wnacg.org";
        string id = "";

        public Form1(string url)
        {
            InitializeComponent();

            txt_URL.Text = url.Replace("wnacg:", "");
            GetRss(url);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (int)Keys.Enter)
            {
                if (txt_URL.Text != "") GetRss(txt_URL.Text);
            }
        }

        private void btn_Download_Click(object sender, EventArgs e)
        {
            if (id == "") { MessageBox.Show("未載入本子"); return; }

            if (listImageViewUrl.Count > 60 && 
                MessageBox.Show("因此本子的頁數超過60頁(包含封面)，故將採用直接讀取網頁的方式來下載\r\n" +
                "此方式會導致下載速度變得極為緩慢，即使如此你還是要下載嗎?\r\n" +
                "(實際速度還是依照自身網速而定)", 
                "注意", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;

            if (Properties.Settings.Default.DownloadPath != "" && Directory.Exists(Properties.Settings.Default.DownloadPath))
                folderBrowserDialog1.SelectedPath = Properties.Settings.Default.DownloadPath;

            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;

            Properties.Settings.Default.DownloadPath = folderBrowserDialog1.SelectedPath;
            Properties.Settings.Default.Save();

            new DownloadBook(folderBrowserDialog1.SelectedPath, id, listImageViewUrl).ShowDialog();
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = sender as PictureBox;

            new ShowBigPicture(listImageViewUrl[int.Parse(pictureBox.Name.Replace("pic_", ""))]).ShowDialog();
        }

        void GetRss(string url)
        {
            btn_Download.Enabled = false;

            foreach (object item in panel1.Controls)
                if (item is PictureBox)
                {
                    PictureBox pictureBox = (item as PictureBox);
                    if (pictureBox.Image != null) pictureBox.Image.Dispose();
                    pictureBox.Dispose();
                }

            panel1.Controls.Clear();
            listImageViewUrl.Clear();

            new Thread(new ThreadStart(delegate
            {
                id = CheckAndConvertURLToID(url);

                if (id != "")
                {
                    SetFormText("取得資料中...");
                    List<string> imageURL = new List<string>();
                    int totalPage = 0;

                    try
                    {
                        HtmlWeb htmlWeb = new HtmlWeb();
                        IEnumerable<HtmlNode> htmlDocumentNode = htmlWeb.Load(string.Format("{0}/photos-index-aid-{1}.html", serverHost, id)).DocumentNode.Descendants();

                        string title = HttpUtility.HtmlDecode(htmlDocumentNode.First((x) => (x.ParentNode.Name == "head" && x.Name == "title")).InnerHtml);

                        IEnumerable<HtmlNode> htmlNodeCollection = htmlDocumentNode.Where((x) => x.NodeType == HtmlNodeType.Element && x.Name == "img" && x.ParentNode.Name == "a");
                        foreach (var item in htmlNodeCollection) if (item.Attributes["src"].Value.StartsWith("//")) imageURL.Add(item.Attributes["src"].Value);

                        IEnumerable<HtmlNode> htmlNodeCollection2 = htmlDocumentNode.Where((x) => x.NodeType == HtmlNodeType.Element && x.Name == "a" && x.ParentNode.Attributes.Any(x2 => x2.Name == "class" && x2.Value == "pic_box"));
                        foreach (var item in htmlNodeCollection2) listImageViewUrl.Add(item.Attributes["href"].Value);

                        try
                        {
                            htmlNodeCollection = htmlDocumentNode.Where((x) => x.NodeType == HtmlNodeType.Element && x.Name == "a" && x.ParentNode.Attributes.Contains("class") && x.ParentNode.Attributes["class"].Value == "f_left paginator");
                            totalPage = int.Parse(htmlNodeCollection.Last().InnerText);
                        }
                        catch (Exception) { totalPage = 1; }
                        

                        for (int i = 2; i < totalPage + 1; i++)
                        {
                            htmlDocumentNode = htmlWeb.Load(string.Format("{0}/photos-index-page-{1}-aid-{2}.html", serverHost, i.ToString(), id)).DocumentNode.Descendants();
                            htmlNodeCollection = htmlDocumentNode.Where((x) => x.NodeType == HtmlNodeType.Element && x.Name == "img" && x.ParentNode.Name == "a");
                            foreach (var item2 in htmlNodeCollection) if (item2.Attributes["src"].Value.StartsWith("//")) imageURL.Add(item2.Attributes["src"].Value);

                            htmlNodeCollection2 = htmlDocumentNode.Where((x) => x.NodeType == HtmlNodeType.Element && x.Name == "a" && x.ParentNode.Attributes.Any(x2 => x2.Name == "class" && x2.Value == "pic_box"));
                            foreach (var item2 in htmlNodeCollection2) listImageViewUrl.Add(item2.Attributes["href"].Value);
                        }

                        SetFormText(title.Remove(title.Length - 21));
                    }
                    catch (Exception)
                    {
                        SetFormText("等待中...");
                        MessageBox.Show("無法取得資料，請確定ID是否正確");
                        return;
                    }

                    btn_Download.Invoke(new Action(delegate { btn_Download.Enabled = true; }));

                    try
                    {
                        int width = 0, height = 0;
                        foreach (string item in imageURL)
                        {
                            PictureBox pictureBox = new PictureBox();

                            if (((width + 1) * 200) > panel1.ClientSize.Width) { width = 0; height++; }
                            if (imageURL.IndexOf(item) == 0) pictureBox.InitialImage = Properties.Resources.loading2;
                            else pictureBox.InitialImage = Properties.Resources.loading;

                            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                            pictureBox.ImageLocation = "https:" + item;
                            pictureBox.Size = new System.Drawing.Size(180, 240);
                            pictureBox.Location = new System.Drawing.Point(12 + (width * 200), 10 + (height * 260));
                            pictureBox.Name = "pic_" + imageURL.IndexOf(item).ToString();
                            pictureBox.Cursor = Cursors.Hand;
                            pictureBox.Click += pictureBox_Click;
                            panel1.Invoke(new Action(delegate { panel1.Controls.Add(pictureBox); }));

                            width++;
                        }
                    }
                    catch (Exception ex) { SetFormText("等待中..."); MessageBox.Show(ex.Message); }
                }
            })).Start();
        }

        string CheckAndConvertURLToID(string url)
        {
            url = url.Replace("wnacg:", "").Replace("www.", "").Replace("m.", "");
            if (url.StartsWith("https://wnacg.com") || url.StartsWith("https://wnacg.org") || url.StartsWith("https://wnacg.net"))
            {
                try
                {
                    string[] urlSplit = url.Split(new char[] { '?' })[0].Split(new char[] { '-' });
                    string tempID = urlSplit[urlSplit.Length - 1], ID = "";

                    foreach (char item in tempID)
                    {
                        if (item >= 48 && item <= 57) ID += item;
                    }

                    return ID;
                }
                catch (Exception) { return ""; }
            }
            else return IsNumber(url) ? url : "";
        }

        bool IsNumber(string text)
        {
            string ID = "";
            foreach (char item in text)
            {
                if (item >= 48 && item <= 57) ID += item;
            }
            return ID == text;
        }

        string FormatImageUrl(string url)
        {
            string[] tempUrl = url.Split(new char[] { '\"' });
            return (tempUrl.Length == 3 ? "https:" + tempUrl[1] : "");
        }
    }    
}
