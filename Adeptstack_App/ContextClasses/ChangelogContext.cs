namespace Adeptstack_App.ContextClasses
{

    public class Changelog
    {
        public ChangelogContext[] Entry { get; set; }
    }

    public class ChangelogContext
    {
        public string title { get; set; }
        public string link { get; set; }
        public string image { get; set; }
        public string date { get; set; }
        public string description { get; set; }
    }

}
