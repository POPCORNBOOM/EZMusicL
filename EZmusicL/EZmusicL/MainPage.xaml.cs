using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace EZmusicL
{
    public partial class MainPage : ContentPage
    {
        List<string> Songs_Url = new List<string>();
        MusicResult nowMusicResult = new MusicResult();
        string savepath = "/storage/emulated/0/Download";
        static string defaultpath = "/storage/emulated/0/Download";
        public MainPage()
        {
            InitializeComponent();
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            changealine();
        }

        public void btn_search_Clicked(object sender, EventArgs e)
        {
            searchlist();
        }

        private async void searchlist()
        {
            var tcm_s_ = Tcm_s();
            btn_search.Text = "搜索中";
            btn_search.IsEnabled = false;
            MusicResult result = await tcm_s_;
            btn_search.Text = "搜索";
            btn_search.IsEnabled = true;
            ts.Clear();
            nowMusicResult = result;
            if (result.music == null)
                return;
            ts.Title = String.Format("歌单:{0}({1})", in_id.Text, result.music.Count);
            foreach (var item in result.music)
            {
                SwitchCell sw = new SwitchCell();
                sw.Text = item.name + " - " + item.artist;
                /*sw.On = false;
                if (sw_ison.IsToggled)
                    sw.On = true;
                */
                sw.On = sw_ison.IsToggled;
                ts.Add(sw);
            }


        }        


       

        private Task<MusicResult> Tcm_s()
        {
            var task = Task.Run(() =>
            {
                MusicResult rt = new MusicResult();
                try
                {
                    string resultstr = DoGetRequestSendData("https:/" + "/api.injahow.cn/meting/?server=netease&type=playlist&id=" + in_id.Text);
                    JObject jObject = new JObject();
                    rt = JsonConvert.DeserializeObject<MusicResult>("{\"music\":" + resultstr + "}");

                }
                catch (Exception ex)
                {
                    Console.WriteLine("<!Error-->" + ex.Message);
                }
                return rt;
            });

            return task;
        }


        public static string DoGetRequestSendData(string url)
        {
            string a;
            WebClient webClient = new WebClient();
            try
            {
                a = webClient.DownloadString(url);
                
            }
            catch (Exception ex)
            {
                a = ex.Message;
            }
            return a;
        }

        private void btn_download_Clicked(object sender, EventArgs e)
        {
            AsyncMethod();
        }

        private async void AsyncMethod()
        {
            var rtcm = TCM();
            btn_download.Text = "正在下载中";
            btn_download.IsEnabled = false;
            await rtcm;
            btn_download.Text = "下载";
            btn_download.IsEnabled = true;

        }

        private async void changealine()
        {
            btn_getaline.IsEnabled = false;
            btn_getaline.Text = "找一找...";
            var gal = getaline();
            aline.Text = await gal;
            btn_getaline.IsEnabled = true;
            btn_getaline.Text = "换一个";

        }

        private Task<string> getaline()
        {
            var task = Task.Run(() =>
            {
                string a = "";
                try
                {
                    WebClient webClient = new WebClient();
                    a = webClient.DownloadString("https://api.injahow.cn/hitokoto/");
                }
                catch(Exception ex)
                {
                    a = "get不到...";
                }
                return a;
            });
            return task;
        }

        private Task TCM()
        {
            var task = Task.Run(() =>
            {
                int index = 0;
                foreach (SwitchCell i in ts)
                {

                    string filenamelegal = "/storage/emulated/0/Download/" + i.Text.Replace("*","").Replace("/", "").Replace(":", "").Replace("?", "") + ".mp3";
                    if (i.On&&(!File.Exists(filenamelegal)))
                    {
                        try
                        {
                            Console.WriteLine("trydownloading:" + nowMusicResult.music[index].url);
                            HttpDownloadFile(nowMusicResult.music[index].url, filenamelegal);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.Error.WriteLine("Error occur when downloading " + nowMusicResult.music[index].url + "\nException ex:" + ex.Message);
                        }
                    }
                    index++;
                }


            });

            return task;
        }



        public static void HttpDownloadFile(string url, string path)
        {
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();

            //创建本地文件写入流
            Stream stream = new FileStream(path, FileMode.Create);

            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0)
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            stream.Close();
            responseStream.Close();
        }


        private void btn_confirm_Clicked(object sender, EventArgs e)
        {
            savepath = folderpath.Text;
        }

        private void btn_reset_Clicked(object sender, EventArgs e)
        {
            folderpath.Text = defaultpath;
            savepath = folderpath.Text;
        }

        public class BrowserTest
        {
            public async Task OpenBrowser(Uri uri)
            {
                try
                {
                    await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
                }
                catch (Exception ex)
                {
                    // An unexpected error occured. No browser may be installed on the device.
                }
            }
        }

        private void btn_opennt_Clicked(object sender, EventArgs e)
        {
            BrowserTest bt = new BrowserTest();
            Uri uri = new Uri("https://music.163.com/");
            bt.OpenBrowser(uri);
        }

        private void btn_getaline_Clicked(object sender, EventArgs e)
        {
            changealine();
        }

        private void btn_random_Clicked(object sender, EventArgs e)
        {
            Random r = new Random();
            int a = r.Next(1,99999);
            int b = r.Next(1,99999);
            string randomid = a.ToString() + b.ToString();
            in_id.Text = randomid;
            searchlist();
        }

        private void btn_openws_Clicked(object sender, EventArgs e)
        {
            BrowserTest bt = new BrowserTest();
            Uri uri = new Uri("https://github.com/POPCORNBOOM/EZMusicL");
            bt.OpenBrowser(uri);

        }
    }
}
