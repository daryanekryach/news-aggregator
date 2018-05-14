using System.Collections.Generic;

namespace NewsAggregator.Models
{
    public class User : Login
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Dictionary<long, FeedCollection> Collections { get; set; }

        public User()
        {
            Collections = new Dictionary<long, FeedCollection>();
            Collections.Add(0, new FeedCollection
            {
                CollectionName = "All feeds"
            });
        }

        public long CreateCollection(string name)
        {
            FeedCollection newCollection = new FeedCollection(name);
            Collections.Add(newCollection.CollectionId, newCollection);
            return newCollection.CollectionId;
        }

        public FeedCollection GetCollection(long id)
        {
            if (Collections.ContainsKey(id))
            {
                return Collections[id];
            }

            return null;
        }
    }
}