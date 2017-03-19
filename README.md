![Build Status](https://ci.appveyor.com/api/projects/status/github/RagtimeWilly/Rxddit?branch=master&svg=true) [![NuGet](https://img.shields.io/nuget/v/Rxddit.svg)](https://www.nuget.org/packages/Rxddit/)

.NET Client for reddit utilizing Reactive Extensions

## Polling Client

The polling client exposes an `IObservable<RedditPostData>` which will be populated with new post data from a specified subreddit peridodically.

For example, the below code polls _/r/funny_ for new posts every 10 seconds:

```
var reddit = new RedditClient(() => new HttpClient());

Action<Exception> onError = (ex) => { Console.WriteLine($"An error occurred: {ex}"); };

var pollingClient = new RedditPollingClient(reddit, "funny", TimeSpan.FromSeconds(10), onError);

var resetEvent = new ManualResetEvent(false);

var subscription = pollingClient
  .Posts
  .Subscribe(
    x => Console.WriteLine($"New post: {x.Title}"),
    ex =>
    {
        Console.WriteLine($"Error while retrieving reddit posts: {ex.Message}");
        resetEvent.Set();
    },
    () =>
    {
        Console.WriteLine("Finished retrieving posts");
        resetEvent.Set();
    });

pollingClient.Start();

resetEvent.WaitOne();
```

## Historical Client

The historical client will retrieve post data for a specified subreddit back to a specified date (or until it runs out of posts) and expose them as a `IObservable<RedditPostData>`.

For example, the below code will retrieve all posts back to _2017-03-01_ from _/r/music_:

```
var reddit = new RedditClient(() => new HttpClient());

Action<Exception, string> onError = (ex, s) => { Console.WriteLine($"An error occurred: {s} : {ex}"); };

var historicalClient = new RedditHistoricalClient(reddit, "music", new DateTime(2017, 03, 03), onError);

var subscription = historicalClient
  .Posts
  .Subscribe(
    x => Console.WriteLine($"Post found: {x.Title}"),
    ex =>
    {
        Console.WriteLine($"Error while retrieving reddit posts: {ex.Message}");
        resetEvent.Set();
    },
    () =>
    {
        Console.WriteLine("Finished retrieving posts");
        resetEvent.Set();
    });

historicalClient.Start();
```

## Getting help

If you have any problems or suggestions please create an [issue](https://github.com/RagtimeWilly/Rxddit/issues) or a [pull request](https://github.com/RagtimeWilly/Rxddit/pulls)