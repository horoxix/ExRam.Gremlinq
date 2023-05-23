﻿using ExRam.Gremlinq.Core.Serialization;
using ExRam.Gremlinq.Core.Tests;
using ExRam.Gremlinq.Core.Tests.Verifiers;
using ExRam.Gremlinq.Providers.CosmosDb.Tests.Fixtures;
using ExRam.Gremlinq.Tests.Entities;

namespace ExRam.Gremlinq.Providers.CosmosDb.Tests
{
    public sealed class SerializationTests : QueryExecutionTest, IClassFixture<SimpleCosmosDbFixture>
    {
        public SerializationTests(SimpleCosmosDbFixture fixture, ITestOutputHelper testOutputHelper) : base(
            fixture,
            new SerializingVerifier<GroovyGremlinQuery>(),
            testOutputHelper)
        {

        }

        [Fact]
        public async Task CosmosDbKey()
        {
            await _g
                .V<Person>(new CosmosDbKey("pk", "id"))
                .Verify();
        }

        [Fact]
        public async Task Inlined_CosmosDbKey()
        {
            await _g
                .ConfigureEnvironment(e => e
                    .ConfigureSerializer(s => s
                        .PreferGroovySerialization()))
                .V<Person>(new CosmosDbKey("pk", "id"))
                .Verify();
        }

        [Fact]
        public async Task CosmosDbKey_with_null_partitionKey()
        {
            await _g
                .V<Person>(new CosmosDbKey("id"))
                .Verify();
        }

        [Fact]
        public async Task Mixed_StringKey_CosmosDbKey()
        {
            await _g
                .V<Person>(new CosmosDbKey("pk", "id"), "id2")
                .Verify();
        }

        [Fact]
        public async Task Mixed_StringKey_CosmosDbKey_with_null_partitionKey()
        {
            await _g
                .V<Person>(new CosmosDbKey("id"), "id2")
                .Verify();
        }
    }
}
