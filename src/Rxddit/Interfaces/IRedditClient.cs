using Rxddit.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rxddit
{
    public interface IRedditClient
    {
        Task<IEnumerable<RedditPostData>> GetPosts(string pageUrl);
    }
}
