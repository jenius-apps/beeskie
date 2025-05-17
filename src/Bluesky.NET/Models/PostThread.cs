using System;
using System.Collections.Generic;
using System.Text;

namespace Bluesky.NET.Models;

public class PostThreadResponse
{
    public PostThread? Thread { get; init; }
}

public class PostThread
{
    public FeedPost? Post { get; init; }

    public PostThread[] Replies { get; init; } = [];
}
