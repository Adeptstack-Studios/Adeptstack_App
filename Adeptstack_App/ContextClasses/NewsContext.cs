using System.Text.Json.Serialization;

namespace Adeptstack_App.ContextClasses
{

    public class NewsContext
    {
        public int id { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
        public string category { get; set; }
        public string content { get; set; }
        public DateTime publishedAt { get; set; }

        /// <summary>
        /// Wird vom Backend berechnet. Ältere Lesezeichen haben den Wert noch nicht (0).
        /// </summary>
        public int readingTimeMinutes { get; set; }

        [JsonIgnore]
        public bool HasReadingTime => readingTimeMinutes > 0;

        [JsonIgnore]
        public string ReadingTimeText => HasReadingTime ? $"{readingTimeMinutes} min read" : "";
    }


}
