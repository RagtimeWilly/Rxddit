using Rxddit.Data;
using Rxddit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Rxddit
{
    /// <summary>
    /// Client will retrieve posts until the time specified or until it reaches end of posts and then stop
    /// </summary>
    public class RedditHistoricalClient
    {
        private readonly IRedditClient _reddit;
        private readonly string _subredditName;
        private readonly DateTime _minimumDate;
        private readonly Subject<RedditPostData> _posts;
        private readonly Action<Exception, string> _logError;

        private bool _stop;

        public RedditHistoricalClient(IRedditClient reddit, string subredditName, DateTime minimumDate, Action<Exception, string> logError)
        {
            _reddit = reddit;
            _subredditName = subredditName;
            _minimumDate = minimumDate;
            _logError = logError;

            _posts = new Subject<RedditPostData>();

            _stop = false;
        }

        public IObservable<RedditPostData> Posts => _posts;

        public async void Start()
        {
            const string urlFormat = "http://www.reddit.com/r/{0}/search.json?sort=new&q=timestamp%3A{1}..{2}&restrict_sr=on&syntax=cloudsearch";

            var from = DateTime.Now.AddMonths(-1);
            var to = DateTime.Now;

            do
            {
                var url = string.Format(urlFormat, _subredditName, from.ToUnixTime(), to.ToUnixTime());

                await GetAllPosts(url);

                to = from;
                from = to.AddMonths(-1);

            } while (from >= _minimumDate);
            
            _posts.OnCompleted();
        }

        private async Task GetAllPosts(string baseUrl)
        {
            var url = baseUrl;
            var count = 0;
            IEnumerable<RedditPostData> posts = null;

            do
            {
                try
                {
                    posts = await _reddit.GetPosts(url);

                    if (posts.Any())
                    {
                        foreach (var post in posts.Where(p => p.CreatedUtc.FromUnixTime() >= _minimumDate))
                            _posts.OnNext(post);

                        count += 25;
                        var lastPost = posts?.Last();

                        url = $"{baseUrl}&count={count}&after={lastPost?.Name}";

                        _stop = posts == null || posts.Any(p => p.CreatedUtc.FromUnixTime() < _minimumDate);
                    }
                    else
                    {
                        _stop = true;
                    }
                }
                catch (Exception ex)
                {
                    _logError(ex, "Error retrieving posts");
                }
            }
            while (!_stop && posts != null && posts.Any());
        }

        public void Cancel()
        {
            _stop = true;
            _posts.OnCompleted();
        }
    }
}
