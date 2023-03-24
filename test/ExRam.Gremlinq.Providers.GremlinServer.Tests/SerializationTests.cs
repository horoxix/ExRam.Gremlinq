﻿using ExRam.Gremlinq.Core;
using ExRam.Gremlinq.Core.Tests;
using ExRam.Gremlinq.Providers.Core;
using Gremlin.Net.Process.Traversal;

using static ExRam.Gremlinq.Core.GremlinQuerySource;

namespace ExRam.Gremlinq.Providers.GremlinServer.Tests
{
    public sealed class SerializationTests : SerializationTestsBase<Bytecode>, IClassFixture<SerializationTests.Fixture>
    {
        public sealed class Fixture : GremlinqTestFixture
        {
            public Fixture() : base(g
                .UseGremlinServer(builder => builder
                    .AtLocalhost()))
            {
            }
        }

        public SerializationTests(Fixture fixture, ITestOutputHelper testOutputHelper) : base(
            fixture,
            testOutputHelper)
        {

        }
    }
}
