using Newtonsoft.Json;

namespace Rxddit.Data
{
    public class RedditPage
    {
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }

        [JsonProperty(PropertyName = "data")]
        public RedditPageData Data { get; set; }
    }
}
