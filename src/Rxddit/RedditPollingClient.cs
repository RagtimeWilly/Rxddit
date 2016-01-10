using Rxddit.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Rxddit
{
    /// <summary>
    /// Retrieves new posts periodically
    /// </summary>
    public class RedditPollingClient
    {
        private readonly IRedditClient _reddit;
        private readonly string _subredditName;
        private readonly Subject<RedditPostData> _posts;
        private readonly TimeSpan _interval;

        private bool _stop;
        private IEnumerable<string> _knownPostIds;

        public RedditPollingClient(IRedditClient reddit, string subredditName, TimeSpan interval)
        {
            _reddit = reddit;
            _subredditName = subredditName;
            _interval = interval;

            _posts = new Subject<RedditPostData>();

            _stop = false;
        }

        public IObservable<RedditPostData> Posts { get { return _posts; } }

        public async Task Start()
        {
            await Task.Run(async () =>
            {
                var url = $"https://www.reddit.com/r/{_subredditName}/new.json";

                var existingPosts = await _reddit.GetPosts(url);
                _knownPostIds = existingPosts.Select(p => p.Id);

                Observable
                    .Interval(_interval)
                    .Subscribe(
                        async x =>
                        {
                            var newPosts = await _reddit.GetPosts(url);

                            foreach (var post in newPosts.Where(p => !_knownPostIds.Contains(p.Id)))
                            {
                                _posts.OnNext(post);
                            }

                            _knownPostIds = newPosts.Select(p => p.Id);
                        });
            });
        }
    }
}
