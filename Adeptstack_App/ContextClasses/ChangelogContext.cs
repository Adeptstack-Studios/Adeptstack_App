namespace Adeptstack_App.ContextClasses
{

    public class ChangelogContext
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
        public string appUrl { get; set; }
        public string content { get; set; }
        public string channel { get; set; }
        public string app { get; set; }
        public string version { get; set; }
        public DateTime publishedAt { get; set; }
    }


}
