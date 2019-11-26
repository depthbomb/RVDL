using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVDL
{
    class DataSchema
    {
        public Data data { get; set; }

        public class Data
        {
            public Child[] children { get; set; }
        }

        public class Child
        {
            public Data1 data { get; set; }
        }

        public class Data1
        {
            public Media media { get; set; }
        }

        public class Secure_Media
        {
            public Reddit_Video reddit_video { get; set; }
        }

        public class Media
        {
            public Reddit_Video reddit_video { get; set; }
        }

        public class Reddit_Video
        {
            public string fallback_url { get; set; }
            public int height { get; set; }
            public int width { get; set; }
            public string scrubber_media_url { get; set; }
            public string dash_url { get; set; }
            public int duration { get; set; }
            public string hls_url { get; set; }
            public bool is_gif { get; set; }
            public string transcoding_status { get; set; }
        }
    }
}
