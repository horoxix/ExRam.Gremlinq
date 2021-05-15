﻿using System.Collections.Immutable;

namespace ExRam.Gremlinq.Core.Steps
{
    public sealed class InjectStep : Step
    {
        public InjectStep(ImmutableArray<object> elements) : base()
        {
            Elements = elements;
        }

        public ImmutableArray<object> Elements { get; }
    }
}
