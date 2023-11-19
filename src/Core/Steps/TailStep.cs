﻿using Gremlin.Net.Process.Traversal;

namespace ExRam.Gremlinq.Core.Steps
{
    public sealed class TailStep : Step
    {
        public static readonly TailStep TailLocal1 = new(1, Scope.Local);
        public static readonly TailStep TailGlobal1 = new(1, Scope.Global);

        internal static readonly MapStep TailLocal1Workaround = new (Traversal.Empty.Push(
            UnfoldStep.Instance,
            TailGlobal1,
            FoldStep.Instance));

        public TailStep(long count, Scope scope)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Count = count;
            Scope = scope;
        }

        public long Count { get; }
        public Scope Scope { get; }
    }
}
