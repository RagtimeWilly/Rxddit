using Newtonsoft.Json;

namespace Rxddit.Data
{
    public class RedditPost
    {
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }

        [JsonProperty(PropertyName = "data")]
        public RedditPostData Data { get; set; }
    }
}
