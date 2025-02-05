namespace Adeptstack_App.ContextClasses
{
    public class News
    {
        public NewsContext[] Entry { get; set; }
    }

    public class NewsContext
    {
        public string title { get; set; }
        public string link { get; set; }
        public string image { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public string date { get; set; }
    }

}
