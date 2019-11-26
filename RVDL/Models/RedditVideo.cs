using System;

namespace RVDL.Models
{
    public class RedditVideo
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
        public string AudioUrl() => (new Uri(new Uri(fallback_url), ".") + "audio") as string;
    }
}
