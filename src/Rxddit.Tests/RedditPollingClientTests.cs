using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;

namespace Rxddit.Tests
{
    [TestFixture]
    public class RedditPollingClientTests
    {
        [Test]
        public void TestPolling()
        {
            var reddit = new RedditClient(() => new HttpClient());

            var pollingClient = new RedditPollingClient(reddit, "funny", TimeSpan.FromSeconds(10), ex => { });

            var resetEvent = new ManualResetEvent(false);

            var subscription = pollingClient.Posts.Subscribe(
                x => Debug.WriteLine($"New post: {x.Title}"),
                ex =>
                {
                    Debug.WriteLine($"Error while retrieving reddit posts: {ex.Message}");
                    resetEvent.Set();
                },
                () =>
                {
                    Debug.WriteLine("Finished retrieving posts");
                    resetEvent.Set();
                });

            pollingClient.Start();

            resetEvent.WaitOne(60000);
        }
    }
}
