﻿namespace ExRam.Gremlinq.Core.AspNet
{
    public static class GremlinqSetupExtensions
    {
        public static ExRam.Gremlinq.Providers.Core.AspNet.ProviderSetup<ExRam.Gremlinq.Providers.CosmosDb.ICosmosDbConfigurator<TVertexBase>> UseCosmosDb<TVertexBase, TEdgeBase>(this ExRam.Gremlinq.Core.AspNet.GremlinqSetup setup) { }
    }
}