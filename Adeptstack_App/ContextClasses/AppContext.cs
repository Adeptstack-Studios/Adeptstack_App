using System;
using System.Collections.Generic;
using System.Text;

namespace Adeptstack_App.ContextClasses
{
    public class AppContext
    {
        public int id { get; set; }
        public string name { get; set; }
        public string slogan { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string slug { get; set; }
        public string category { get; set; }
        public string platforms { get; set; }
        public string image1Url { get; set; }
        public string feature1Desc { get; set; }
        public string image2Url { get; set; }
        public string feature2Desc { get; set; }
        public string image3Url { get; set; }
        public string feature3Desc { get; set; }
        public string publishedAt { get; set; }
        public ChangelogContext latestMainVersion { get; set; }
        public bool highlighted { get; set; }
        public bool legacy { get; set; }
    }
}
