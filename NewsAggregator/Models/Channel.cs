using System.Collections.Generic;

namespace NewsAggregator.Models
{
    public class Channel
    {
        private static long id { get; set; } = 1;
        public string Title { get; set; } = "Untitled";
        public string Description { get; set; } = "No description";
        public string Link { get; set; } = "No link";
        public string ImageLink { get; set; } = "No link";
        public List<Post> Posts { get; set; } = new List<Post>();

        public long Id
        {
            get => id;
            set => id = value;
        }


        public Channel()
        {
            Id++;
        }

        public Channel(string source)
        {
            Id++;
            Link = source;
        }

    }
}