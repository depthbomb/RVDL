using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace RVDL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Submit(object sender, RoutedEventArgs e)
        {
            if (!Uri.IsWellFormedUriString(txtBoxURL.Text, UriKind.Absolute))
            {
                Failed("The URL provided is not valid");
                return;
            }
            var url = new Uri(txtBoxURL.Text);
            if (url.GetLeftPart(UriPartial.Authority).Substring(url.GetLeftPart(UriPartial.Scheme).Length) != "www.reddit.com")
            {
                Failed("The domain of the url was not found to be www.reddit.com");
                return;
            }

            DataSchema.Reddit_Video data;
            try
            {
                string json;
                using (WebClient wc = new WebClient())
                {
                    json = wc.DownloadString(url + ".json");
                }
                data = JsonConvert.DeserializeObject<List<DataSchema>>(json)[0].data.children[0].data.media.reddit_video;
            }
            catch (Exception ex)
            {
                Failed("The URL provided is not of a valid post");
                return;
            }
            string videoPath = Path.GetTempFileName() + ".mp4";
            string audioPath = Path.GetTempFileName() + ".mp3";

            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile(data.fallback_url, videoPath);
                wc.DownloadFile(new Uri(new Uri(data.fallback_url), ".") + "audio", audioPath);
            }
            string ffmpeg = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\ffmpeg\\ffmpeg.exe";
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "output";
            dlg.DefaultExt = "mp4";
            dlg.Filter = "MPEG Files (*.mp4)|*.mp4";

            dlg.InitialDirectory = Properties.Settings.Default.Path;
            if (dlg.InitialDirectory == "")
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

            if (!(bool)dlg.ShowDialog())
            {
                Failed("Save picker was closed");
                return;
            }
                

            Properties.Settings.Default.Path = Path.GetDirectoryName(dlg.FileName);
            Properties.Settings.Default.Save();

            var result = Process.Start(ffmpeg, "-i " + videoPath + " -i " + audioPath + " -c copy " + dlg.FileName);
        }

        private void Failed(string err)
        {
            txtError.Text = err;
            txtError.Visibility = Visibility.Visible;
        }
    }
}
