using System.Collections.Generic;
using System.Linq;

namespace NewsAggregator.Models
{
    public class FeedCollection
    {
        public string CollectionName { get; set; } = "Untitled";
        private static long collectionId = 0;
        public List<Channel> Channels { get; set; }

        public long CollectionId
        {
            get => collectionId;
            set => collectionId = value;
        }

        public FeedCollection()
        {
            collectionId++;
            Channels = new List<Channel>();
        }

        public FeedCollection(string title)
        {
            collectionId++;
            Channels = new List<Channel>();
            CollectionName = title;
        }

        public void AddChannel(string source)
        {
            Channels.Add(new Channel(source));
        }

        public List<Post> GetAllPosts()
        {
            if (Channels.Count != 0)
            {
                List<Post> posts = new List<Post>();
                Channels.ForEach(x =>
                    RssHandler.GetChannelPosts(x).ForEach(post => posts.Add(post)));
                return posts.OrderByDescending(x => x.Date).ToList();
            }
            else return new List<Post>();
        }
    }
}