namespace Adeptstack_App.ContextClasses
{
    /// <summary>
    /// Ein Wert für die Filter-Chips aus /api/news/categories bzw. /api/changelogs/channels.
    /// </summary>
    public class FilterOption
    {
        public string name { get; set; }
        public int count { get; set; }
    }
}
