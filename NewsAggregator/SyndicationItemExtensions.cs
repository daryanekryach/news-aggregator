using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml.Linq;

namespace NewsAggregator
{
    public static class SyndicationItemExtensions
    {
        public static string GetCreator(this SyndicationItem item)
        {
            var creator = item.GetElementExtensionValueByOuterName("creator");
            return creator;
        }

        public static string GetPublisher(this SyndicationItem item)
        {
            var publisher = item.GetElementExtensionValueByOuterName("publisher");
            return publisher;
        }

        public static string GetTitle(this SyndicationItem item)
        {
            var title = item.GetElementExtensionValueByOuterName("title");
            return title;
        }

        private static string GetElementExtensionValueByOuterName(this SyndicationItem item, string outerName)
        {
            if (item.ElementExtensions.All(x => x.OuterName != outerName)) return null;
            return item.ElementExtensions.First(x => x.OuterName == outerName).GetObject<XElement>().Value;
        }
    }
}
