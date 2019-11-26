using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;

using RVDL.Models;
using RVDL.Windows;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RVDL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow root;
        private static Button _downloadButton;
        private static TextBox _urlInput;

        private static string _ffmpegBin;
        private static string _downloadPath;

        public MainWindow()
        {
            root = this;
            root.InitializeComponent();

            _downloadButton = root.DownloadButton;
            _urlInput = root.UrlInput;

            CheckForFfmpeg();
            LoadSettings();
        }

        void UrlInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = _urlInput.Text.Trim();
            Uri res;
            bool valid = !string.IsNullOrEmpty(input) &&
                         (
                            Uri.TryCreate(input, UriKind.Absolute, out res) &&
                            res != null
                         );
            _downloadButton.IsEnabled = valid;
        }

        void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (_downloadButton.IsEnabled)
                DownloadVideo(_urlInput.Text);
        }

        void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var about = new AboutWindow();
                about.ShowInTaskbar = false;
                about.ShowDialog();
        }

        void DownloadVideo(string input)
        {
            _downloadButton.IsEnabled = false;

            //  Won't stop every non-reddit URL but we will catch those later
            if (input.Contains("www.reddit.com"))
            {
                string url = input + ".json";
                string response = Get(url);
                if (response != null)
                {
                    JArray json = JArray.Parse(response);
                    string videoName = json[0]["data"]["children"][0]["data"]["title"].ToString();
                    var secureMedia = json[0]["data"]["children"][0]["data"]["secure_media"];
                    if (secureMedia.Type != JTokenType.Null)
                    {
                        string redditVideo = secureMedia["reddit_video"].ToString();
                        string outputFile = Path.Combine(_downloadPath, SafeFileName(videoName) + ".mp4");

                        //  Just go ahead and delete existing file
                        if (File.Exists(outputFile))
                            File.Delete(outputFile);

                        RedditVideo rv = JsonConvert.DeserializeObject<RedditVideo>(redditVideo);

                        string tempVideo = Path.GetTempFileName() + ".mp4";
                        string tempAudio = Path.GetTempFileName() + ".mp4";

                        using (var wc = new WebClient())
                        {
                            wc.DownloadFile(rv.fallback_url, tempVideo);
                            wc.DownloadFile(rv.AudioUrl(), tempAudio);
                        }

                        CombineMedia(tempVideo, tempAudio, outputFile, out string output);

                        if (string.IsNullOrWhiteSpace(output))
                            MessageBox.Show($"Video successfully downloaded to {_downloadPath}", "Download Successful", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                        else
                            //  TODO: parse output to show different messages
                            MessageBox.Show(output);

                        //  Clean up the temporary files
                        File.Delete(tempVideo);
                        File.Delete(tempAudio);
                    }
                    else
                        MessageBox.Show("The URL you provided is not a valid Reddit media URL.", "Invalid URL", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            }

            _urlInput.Text = string.Empty;
        }

        void CombineMedia(string videoFile, string audioFile, string outputFile, out string output)
        {
            string args = $"-i {videoFile} -i {audioFile} -c copy \"{outputFile}\" -loglevel error";
            using (var p = new Process())
            {
                p.StartInfo.FileName = _ffmpegBin;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.Arguments = args;
                p.Start();

                var reader = p.StandardOutput;
                
                output = reader.ReadToEnd().Trim();

                p.WaitForExit();
            }
        }

        void LoadSettings()
            => _downloadPath = KnownFolders.GetPath(KnownFolders.KnownFolder.Downloads);

        void CheckForFfmpeg()
        {
            string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
            if (File.Exists(ffmpegPath))
                _ffmpegBin = ffmpegPath;
            else
            {
                MessageBox.Show("The FFmpeg binary is missing. This normally doesn't happen unless you mess with the program's files after installation.\n\nYou can either reinstall RVDL or download a new FFmpeg binary from its website.", "Missing Binary", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);

                Environment.Exit(2);
            }
        }

        string Get(string uri)
        {
            try
            {
                var request = WebRequest.Create(uri) as HttpWebRequest;
                    request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                using (var response = request.GetResponse() as HttpWebResponse)
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                return null;
            }
        }

        string SafeFileName(string fileName)
            => string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
    }
}
