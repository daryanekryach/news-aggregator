using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Xml;
using System.ServiceModel.Syndication;
using NewsAggregator.Models;

namespace NewsAggregator
{
    public class RssHandler
    {
        private static HttpClient client = new HttpClient(new HttpClientHandler()
        {
            SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });


        public static Channel GetChannelInfo(Channel channel)
        {
            try
            {
                var result = client.GetStreamAsync(new Uri(channel.Link)).Result;

                using (var xmlReader = XmlReader.Create(result))
                {
                    var feed = SyndicationFeed.Load(xmlReader);

                    if (feed != null)
                    {
                        channel = HandleChannelInfo(feed);
                        return channel;
                    }
                }
            }
            catch (UriFormatException e)
            {
                Console.WriteLine("URI isn't correct");
            }

            return channel;
        }

        public static List<Post> GetChannelPosts(Channel channel)
        {
            try
            {
                var result = client.GetStreamAsync(new Uri(channel.Link)).Result;

                using (var xmlReader = XmlReader.Create(result))
                {
                    var feed = SyndicationFeed.Load(xmlReader);

                    if (feed != null)
                    {
                        return HandleChannelPosts(feed);
                    }
                }
            }
            catch (UriFormatException e)
            {
                Console.WriteLine("URI isn't correct");
            }

            return new List<Post>();
        }

        private static Channel HandleChannelInfo(SyndicationFeed feed)
        {
            var channel = new Channel
            {
                Title = feed.Title.Text,
                Description = feed.Description.Text,
                Link = feed.Links[0].Uri.AbsoluteUri,
                ImageLink = feed.ImageUrl != null ? feed.ImageUrl.AbsoluteUri : "none"
            };
            return channel;
        }

        public static List<Post> HandleChannelPosts(SyndicationFeed feed)
        {
            var channelPosts = new List<Post>();
            feed.Items.ToList().ForEach(feedPost =>
            {
                channelPosts.Add(new Post
                {
                    Title = feedPost.Title.Text,
                    Summary = feedPost.Summary.Text,
                    Author = feedPost.GetCreator() ?? feedPost.GetPublisher() ??
                             (feed.Authors.FirstOrDefault() ?? new SyndicationPerson()).Name,
                    Link = feedPost.Links[0].Uri.AbsoluteUri,
                    Date = feedPost.PublishDate.DateTime
                });
            });
            return channelPosts;
        }
    }
}