using System;
using System.Collections.Generic;
using System.Text;

namespace Bluesky.NET.Models;

public abstract class BaseRecordBody
{
    public required string Repo { get; init; }

    public required string Collection { get; init; }
}

public class CreateRecordBody : BaseRecordBody
{
    public required SubmissionRecord Record { get; init; }
}

public class FollowRecordBody : BaseRecordBody
{
    public required FollowRecord Record { get; init; }
}

public class FollowRecord
{
    public required string Subject { get; init; }

    public required DateTime CreatedAt { get; init; }
}
