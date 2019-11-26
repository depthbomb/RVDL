using System.Windows;

namespace RVDL.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            this.AppVersion.Text = Version.FullVersion;

        }

        private void DepthbombLink(object sender, RoutedEventArgs e)
            => OpenUrl("https://github.com/depthbomb");

        private void SaghenLink(object sender, RoutedEventArgs e)
            => OpenUrl("https://github.com/saghen");

        private void FfmpegLink(object sender, RoutedEventArgs e)
            => OpenUrl("https://www.ffmpeg.org/");

        private void OpenUrl(string url)
            => System.Diagnostics.Process.Start("explorer.exe", url);
    }
}
