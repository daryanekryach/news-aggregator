using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NewsAggregator.Models;

namespace NewsAggregator.Controllers
{
    [Produces("application/json")]
    [Route("api/collections")]
    public class CollectionController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;

        private static User user = new User();

        public CollectionController(IMemoryCache memoryCache, ILogger<CollectionController> logger)
        {
            _cache = memoryCache;
            _logger = logger;
        }

        // GET ALL COLLECTIONS 
        [ResponseCache(Duration = 60)]
        [HttpGet, Authorize]
        public JsonResult Get()
        {
            long collectionId = 0;
            List<Post> posts;

            if (!_cache.TryGetValue(collectionId, out posts))
            {
                posts = user.GetCollection(0).GetAllPosts();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                _cache.Set(collectionId, posts, cacheEntryOptions);
                _logger.LogInformation("Posts from all collections have been cached");
            }

            return Json(posts);
        }

        // GET COLLECTION WITH ID 
        [ResponseCache(Duration = 60)]
        [HttpGet("{id}"), Authorize]
        public JsonResult Get(long id)
        {
            List<Post> posts;
            if (user.GetCollection(id) != null)
            {
                if (!_cache.TryGetValue(id, out posts))
                {
                    posts = user.GetCollection(0).GetAllPosts();

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                    _cache.Set(id, posts, cacheEntryOptions);
                    _logger.LogInformation("Posts from collection have been cached");
                }

                return Json(posts);
            }
            else
            {
                _logger.LogError("No such collection");
                return Json("");
            }
        }

        // ADD CHANNEL TO COLLECTION
        [HttpPost("{id}"), Authorize]
        public void Post(long id, [FromQuery] string feed)
        {
            if (user.GetCollection(id) != null && feed != "")
            {
                user.GetCollection(id).AddChannel(feed);
                Channel isInAll = user.GetCollection(0).Channels.Find(x => x.Link == feed);
                if (isInAll == null && id != 0) user.GetCollection(0).AddChannel(feed);
                _logger.LogInformation("New channel was added to collection");
            }
            else _logger.LogError("No such collection");
        }

        // CREATE COLLECTION
        [HttpPost, Authorize]
        public long Post([FromBody] string title)
        {
            var collectionId = user.CreateCollection(title);
            _logger.LogInformation("New collection was created");
            return collectionId;
        }
    }
}