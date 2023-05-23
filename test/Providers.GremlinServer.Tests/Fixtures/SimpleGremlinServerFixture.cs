﻿using ExRam.Gremlinq.Core;
using ExRam.Gremlinq.Core.Tests;
using ExRam.Gremlinq.Providers.Core;
using static ExRam.Gremlinq.Core.GremlinQuerySource;

namespace ExRam.Gremlinq.Providers.GremlinServer.Tests
{
    public sealed class SimpleGremlinServerFixture : GremlinqFixture
    {
        public SimpleGremlinServerFixture() : base(g
            .UseGremlinServer(_ => _
                .AtLocalhost()
                .UseNewtonsoftJson()))
        {
        }
    }
}
