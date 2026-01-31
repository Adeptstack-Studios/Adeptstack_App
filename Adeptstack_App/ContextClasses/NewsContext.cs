namespace Adeptstack_App.ContextClasses
{

    public class NewsContext
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
        public string category { get; set; }
        public string content { get; set; }
        public DateTime publishedAt { get; set; }
    }


}
