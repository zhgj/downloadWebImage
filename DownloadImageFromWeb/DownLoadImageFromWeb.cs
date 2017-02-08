using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownloadImageFromWeb
{
    public partial class DownLoadImageFromWeb : Form
    {
        public DownLoadImageFromWeb()
        {
            InitializeComponent();
            //Base64StringToImage(@"E:\xunlei\down\base64string.txt");
        }

        public void btnDownload_Click(object sender, EventArgs e)
        {
            string webUrl = txtWebUrl.Text;
            string localPath = @"E:\xunlei\down\";

            CheckForIllegalCrossThreadCalls = false;

            string WebUrlQueue = "WebUrlQueue";
            string ImageUrlQueue = "ImageUrlQueue";

            RabbitMQ.RabbitMQ rabbitmq = new RabbitMQ.RabbitMQ();
            rabbitmq.Send(webUrl, WebUrlQueue, WebUrlQueue, WebUrlQueue);

            string WebUrlHistory = "WebUrlHistory";
            string ImageUrlHistory = "ImageUrlHistory";
            Redis.Redis redis = new Redis.Redis();


            // 准备资源
            var getUrlTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    //try
                    //{
                    //richTextBox.Clear();
                    richTextBox.AppendText("准备资源任务活动中...\r\n");
                    richTextBox.AppendText("Redis中key的个数：" + redis.GetCount() + "...\r\n");
                    richTextBox.Focus();
                    richTextBox.Select(richTextBox.Text.Length, 0);
                    string webUrlRabbit = rabbitmq.Receive(WebUrlQueue);
                    if (string.IsNullOrWhiteSpace(webUrlRabbit))
                        Thread.Sleep(500);
                    string htmlString = GetWebHtmlString(webUrlRabbit);

                    // 获取所有图片路径
                    List<string> imageTag = GetImgTag(htmlString);
                    foreach (var tag in imageTag)
                    {
                        if (string.IsNullOrWhiteSpace(tag))
                            continue;
                        string imageUrl = GetImgUrl(tag);
                        if (string.IsNullOrWhiteSpace(imageUrl))
                            continue;
                        else
                        {
                            if (string.IsNullOrWhiteSpace(redis.Get(imageUrl)))
                            {
                                rabbitmq.Send(imageUrl, ImageUrlQueue, ImageUrlQueue, ImageUrlQueue);
                                redis.Add(imageUrl, imageUrl);
                            }
                        }
                        Thread.Sleep(15);
                    }

                    // 获取所有网址
                    List<string> aTag = GetATag(htmlString);
                    foreach (var tag in aTag)
                    {
                        string url = GetAUrl(tag);
                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            if (string.IsNullOrWhiteSpace(redis.Get(url)))
                            {
                                if (IsFile(url))
                                    rabbitmq.Send(url, ImageUrlQueue, ImageUrlQueue, ImageUrlQueue);
                                else
                                    rabbitmq.Send(url, WebUrlQueue, WebUrlQueue, WebUrlQueue);
                                redis.Add(url, url);
                            }
                        }
                        Thread.Sleep(15);
                        //File.AppendAllText(@"E:\xunlei\down\webUrl.txt", url + "\r\n");
                    }
                    //}
                    //catch
                    //{
                    //    richTextBox.AppendText("Oh，My God 准备资源任务异常！\r\n");
                    //}
                }
            });

            // 下载
            var download1 = Task.Factory.StartNew(() =>
             {
                 while (true)
                 {
                     //try
                     //{
                     //richTextBox.Clear();
                     richTextBox.AppendText(string.Format("图片下载任务{0}活动中...\r\n", 1));
                     richTextBox.Focus();
                     richTextBox.Select(richTextBox.Text.Length, 0);
                     string imageUrl = rabbitmq.Receive(ImageUrlQueue);
                     if (!string.IsNullOrWhiteSpace(imageUrl))
                     {
                         if (imageUrl.StartsWith("data:image"))
                         {
                             // data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAegAAADSCAYAAACBxlNzAA....
                             string[] base64String = imageUrl.Split(',');
                             string filter = "." + base64String[0].Split(new char[] { '/', ';' })[1];
                             SaveImageFromBase64String(base64String[1], localPath + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fffffff_") + new Random().Next(1000, 10000) + filter);
                         }
                         else
                         {
                             int lastIndex = imageUrl.LastIndexOf("http");
                             if (lastIndex >= 0)
                             {
                                 //string ss = imageUrl.Remove(0, lastIndex);
                                 imageUrl = imageUrl.Remove(0, lastIndex);
                                 string imageName = GetImageName(imageUrl);
                                 Download(imageUrl, localPath + imageName);
                             }
                             else
                             {
                                 // 另外还有不带域名的相对路径，记录下域名拼接一下就行了
                                 // todo...
                             }
                         }
                     }
                     else
                     {
                         //Thread.Sleep(88);
                     }
                     //}
                     //catch
                     //{
                     //    richTextBox.AppendText("Oh，My God 图片下载任务" + 1 + "异常！\r\n");
                     //}
                     Thread.Sleep(15);
                 }
             });

            var download2 = Task.Factory.StartNew(() =>
             {
                 while (true)
                 {
                     //try
                     //{
                     richTextBox.Clear();
                     richTextBox.AppendText(string.Format("图片下载任务{0}活动中...\r\n", 2));
                     richTextBox.Focus();
                     richTextBox.Select(richTextBox.Text.Length, 0);
                     string imageUrl = rabbitmq.Receive(ImageUrlQueue);
                     if (!string.IsNullOrWhiteSpace(imageUrl))
                     {
                         if (imageUrl.StartsWith("data:image"))
                         {
                             // data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAegAAADSCAYAAACBxlNzAA....
                             string[] base64String = imageUrl.Split(',');
                             string filter = "." + base64String[0].Split(new char[] { '/', ';' })[1];
                             SaveImageFromBase64String(base64String[1], localPath + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fffffff_") + new Random().Next(1000, 10000) + filter);
                         }
                         else
                         {
                             int lastIndex = imageUrl.LastIndexOf("http");
                             if (lastIndex >= 0)
                             {
                                 //string ss = imageUrl.Remove(0, lastIndex);
                                 imageUrl = imageUrl.Remove(0, lastIndex);
                                 string imageName = GetImageName(imageUrl);
                                 Download(imageUrl, localPath + imageName);
                             }
                             else
                             {
                                 // 另外还有不带域名的相对路径，记录下域名拼接一下就行了
                                 // todo...
                             }
                         }
                     }
                     else
                     {
                         //Thread.Sleep(88);
                     }
                     //}
                     //catch
                     //{
                     //    richTextBox.AppendText("Oh，My God 图片下载任务" + 2 + "异常！\r\n");
                     //}
                     Thread.Sleep(15);
                 }
             });

            var download3 = Task.Factory.StartNew(() =>
             {
                 while (true)
                 {
                     //try
                     //{
                     richTextBox.Clear();
                     richTextBox.AppendText(string.Format("图片下载任务{0}活动中...\r\n", 3));
                     richTextBox.Focus();
                     richTextBox.Select(richTextBox.Text.Length, 0);
                     string imageUrl = rabbitmq.Receive(ImageUrlQueue);
                     if (!string.IsNullOrWhiteSpace(imageUrl))
                     {
                         if (imageUrl.StartsWith("data:image"))
                         {
                             // data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAegAAADSCAYAAACBxlNzAA....
                             string[] base64String = imageUrl.Split(',');
                             string filter = "." + base64String[0].Split(new char[] { '/', ';' })[1];
                             SaveImageFromBase64String(base64String[1], localPath + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fffffff_") + new Random().Next(1000, 10000) + filter);
                         }
                         else
                         {
                             int lastIndex = imageUrl.LastIndexOf("http");
                             if (lastIndex >= 0)
                             {
                                 //string ss = imageUrl.Remove(0, lastIndex);
                                 imageUrl = imageUrl.Remove(0, lastIndex);
                                 string imageName = GetImageName(imageUrl);
                                 Download(imageUrl, localPath + imageName);
                             }
                             else
                             {
                                 // 另外还有不带域名的相对路径，记录下域名拼接一下就行了
                                 // todo...
                             }
                         }
                     }
                     else
                     {
                         //Thread.Sleep(88);
                     }
                     //}
                     //catch
                     //{
                     //    richTextBox.AppendText("Oh，My God 图片下载任务" + 1 + "异常！\r\n");
                     //}
                     Thread.Sleep(15);
                 }
             });

            var clearText = Task.Factory.StartNew(() =>
             {
                 richTextBox.Clear();
             });

            richTextBox.AppendText("启动完成...\r\n");
            //Task.Factory.StartNew(() =>
            //{
            //    while (true)
            //    {
            //        richTextBox.AppendText("getUrlTask：" + getUrlTask.Status.ToString() + "...\r\n");

            //        richTextBox.AppendText("download1：" + download1.Status.ToString() + "...\r\n");
            //        richTextBox.AppendText("download2：" + download2.Status.ToString() + "...\r\n");
            //        richTextBox.AppendText("download3：" + download3.Status.ToString() + "...\r\n");

            //        richTextBox.AppendText("clearText：" + clearText.Status.ToString() + "...\r\n");
            //    }
            //}
            //);

        }

        public string GetWebHtmlString(string webUrl)
        {
            WebClient client = new WebClient();
            var result = client.DownloadString(webUrl);
            return result;
        }

        public List<string> GetImgTag(string strHTML)
        {
            string startHtml = "<script";
            string endHtml = "</script>";
            
            while (strHTML.IndexOf(startHtml) >= 0 && strHTML.IndexOf(endHtml) >= 0)
            {
                int startIndex = strHTML.IndexOf(startHtml); // 3
                int endIndex = strHTML.IndexOf(endHtml); // 15

                strHTML = strHTML.Remove(startIndex, endIndex - startIndex + endHtml.Length);
            }

            Regex regObj = new Regex("<img.+?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            List<string> strAry = new List<string>();
            foreach (Match matchItem in regObj.Matches(strHTML))
            {
                if (matchItem.Value.Contains("src="))
                    strAry.Add(matchItem.Value);
            }
            return strAry;
        }

        public string GetImgUrl(string imgTagStr)
        {
            //http和https开头的
            Regex regObj = new Regex(@"(http|https)://.+?.(gif|bmp|jpg|jpeg|png)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var match = regObj.Match(imgTagStr);
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                //base64的
                Regex regObj1 = new Regex(@"data:image.+?==", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var match1 = regObj1.Match(imgTagStr);
                if (string.IsNullOrWhiteSpace(match1.Value))
                {
                    //双斜杠开头的
                    Regex regObj2 = new Regex(@"src=.+?.(gif|bmp|jpg|jpeg|png)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    var match2 = regObj2.Match(imgTagStr);
                    if (match2.Success)
                    {
                        string url = match2.Value.Split('=')[1].Replace("\"", "");
                        if (url.StartsWith("//"))
                            return "http:" + url;
                        else
                            return url;
                    }
                    else return match2.Value;
                }
                else return match1.Value;
            }
            else return match.Value;
        }

        public string GetImageName(string imageUrl)
        {
            string b = System.Web.HttpUtility.UrlDecode(imageUrl, System.Text.Encoding.Default);  //解码
            string[] imageUrlArray = b.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var imageName = imageUrlArray[imageUrlArray.Length - 1];
            ////if (imageName.Contains("1120036054_14806365568021n") || imageName.Contains("7154795352798288728_11n") || imageName.Contains("loading_3") || imageName.Contains("sUFejrblcFihxtt"))
            ////{
            ////    string ss = "";
            ////}
            return imageName;
        }

        public void Download(string webUrl, string localPath)
        {
            WebClient client = new WebClient();
            try
            {
                string b = System.Web.HttpUtility.UrlDecode(webUrl, System.Text.Encoding.Default);  //解码
                client.DownloadFile(b, localPath);
                //client.DownloadFileTaskAsync(b, localPath);
            }
            catch
            {
                //string s = System.Web.HttpUtility.UrlEncode(webUrl, System.Text.Encoding.Default); //编码
                //string b = System.Web.HttpUtility.UrlDecode(webUrl, System.Text.Encoding.Default);  //解码
                //File.AppendAllText(@"E:\xunlei\down\downFail.txt", b + "\r\n");
            }
        }

        public void SaveImageFromBase64String(string base64String,string localPath)
        {
            byte[] arr = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(arr);
            Bitmap bmp = new Bitmap(ms);
            ms.Close();
            bmp.Save(localPath);
        }

        //图片转为base64编码的文本
        private void ImgToBase64String(string imagePath)
        {
            Image fromImage = Image.FromFile(imagePath);
            var fs = File.OpenRead(imagePath);
            MemoryStream stream = new MemoryStream();
            fs.CopyTo(stream);
            var base64 = Convert.ToBase64String(stream.GetBuffer());
        }

        public void Base64StringToImage(string txtFileName)
        {
            try
            {
                FileStream ifs = new FileStream(txtFileName, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(ifs);

                string inputStr = sr.ReadToEnd();
                byte[] arr = Convert.FromBase64String(inputStr);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);

                //bmp.Save(txtFileName + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                //bmp.Save(txtFileName + ".bmp", ImageFormat.Bmp);
                //bmp.Save(txtFileName + ".gif", ImageFormat.Gif);
                //bmp.Save(txtFileName + ".png", ImageFormat.Png);
                ms.Close();
                sr.Close();
                ifs.Close();
                //this.pictureBox1.Image = bmp;
                //if (File.Exists(txtFileName))
                //{
                //    File.Delete(txtFileName);
                //}
                //MessageBox.Show("转换成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Base64StringToImage 转换失败\nException：" + ex.Message);
            }
        }



        public List<string> GetATag(string strHTML)
        {
            string startHtml = "<script";
            string endHtml = "</script>";
            while (strHTML.IndexOf(startHtml) >= 0 && strHTML.IndexOf(endHtml) >= 0)
            {
                int startIndex = strHTML.IndexOf(startHtml); // 3
                int endIndex = strHTML.IndexOf(endHtml); // 15
                int length = endIndex - startIndex + endHtml.Length;
                if (length > 0)
                    strHTML = strHTML.Remove(startIndex, endIndex - startIndex + endHtml.Length);
            }

            Regex regObj = new Regex("<a.+?/a>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            List<string> strAry = new List<string>();
            foreach (Match matchItem in regObj.Matches(strHTML))
            {
                if (matchItem.Value.Contains("href="))
                    strAry.Add(matchItem.Value);
            }
            return strAry;
        }

        public string GetAUrl(string aTagStr)
        {
            // <a href="testCall.html" class="hhhh">hgdgjg</a>
            string webUrl = null;
            var tagStringArr = aTagStr.Split(new char[] { ' ', '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var arr in tagStringArr)
            {
                if (arr.Contains("href="))
                {
                    webUrl = arr.Split('=')[1].Trim('"').Trim('\'');
                    break;
                }
            }
            if (!webUrl.StartsWith("http"))
                return null;
            return webUrl;
        }

        public bool IsFile(string url)
        {
            bool isFile = false;
            List<string> common = new List<string>();
            common.AddRange(new string[] { ".apk", ".txt", ".doc", ".xls", ".ppt", ".docx", ".xlsx", ".pptx", ".pdf", ".rar", ".zip", ".7z", ".wav" });
            foreach (var item in common)
            {
                if (url.ToLower().EndsWith(item))
                {
                    isFile = true;
                    break;
                }
            }
            return isFile;
        }
    }
}
