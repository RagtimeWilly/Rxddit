using Newtonsoft.Json;
using System.Collections.Generic;

namespace Rxddit.Data
{
    public class RedditPageData
    {
        [JsonProperty(PropertyName = "modHash")]
        public string ModHash { get; set; }

        [JsonProperty(PropertyName = "children")]
        public List<RedditPost> Children { get; set; }
    }
}
