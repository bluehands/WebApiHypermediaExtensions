﻿using System;

namespace WebApi.HypermediaExtensions.Hypermedia.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class HypermediaPropertyAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
