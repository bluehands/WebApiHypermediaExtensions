﻿namespace Hypermedia.Client.Exceptions
{
    using System;

    /// <summary>
    /// The resource resolved is not of the expected type.
    /// </summary>
    public class BadResource : HypermediaClientException
    {
        public BadResource(string Title, Uri ProblemType, string Detail)
            : base(Title, ProblemType, Detail)
        {
        }
    }
}