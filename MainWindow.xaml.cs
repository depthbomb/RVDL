using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using RVDL;

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
            string json;
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(url + ".json");
            }
            var data = JsonConvert.DeserializeObject<List<DataSchema>>(json)[0].data.children[0].data.media.reddit_video;
        }

        

        private void Failed(string err)
        {
            txtError.Text = err;
            txtError.Visibility = Visibility.Visible;
        }
    }
}
