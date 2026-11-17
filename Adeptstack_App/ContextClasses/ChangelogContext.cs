using System.Text.Json.Serialization;

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
        public int appId { get; set; }
        public string version { get; set; }
        public DateTime publishedAt { get; set; }
        public bool upcoming { get; set; }
        public DateTime? releaseAt { get; set; }

        /// <summary>
        /// Bewusst zeitbasiert statt nur das Flag: Lesezeichen speichern das ganze Objekt als JSON
        /// und blieben sonst auch nach dem Release für immer "upcoming".
        /// </summary>
        [JsonIgnore]
        public bool IsUpcoming => upcoming && (releaseAt?.ToUniversalTime() ?? publishedAt) > DateTime.UtcNow;
    }


}
