using Newtonsoft.Json;
using Rxddit.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rxddit
{
    public class RedditClient : IRedditClient
    {
        private Func<HttpClient> _clientFactory;

        public RedditClient(Func<HttpClient> clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IEnumerable<RedditPostData>> GetPosts(string pageUrl)
        {
            using (var httpClient = _clientFactory())
            {
                var json = await httpClient.GetStringAsync(pageUrl);

                var page = JsonConvert.DeserializeObject<RedditPage>(json);

                return page.Data.Children.Select(p => p.Data);
            }
        } 
    }
}
