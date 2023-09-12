﻿using ExRam.Gremlinq.Core;
using static ExRam.Gremlinq.Core.GremlinQuerySource;

namespace ExRam.Gremlinq.Tests.Fixtures
{
    public abstract class GremlinqFixture : IAsyncLifetime
    {
        private sealed class EmptyGremlinqTestFixture : GremlinqFixture
        {
            protected override async Task<IGremlinQuerySource> TransformQuerySource(IConfigurableGremlinQuerySource g) => g.ConfigureEnvironment(_ => _);
        }

        public static readonly GremlinqFixture Empty = new EmptyGremlinqTestFixture();

        private static readonly TaskCompletionSource<IGremlinQuerySource> Disposed = new ();
        private TaskCompletionSource<IGremlinQuerySource>? _lazyQuerySource;

        static GremlinqFixture()
        {
            Disposed.TrySetException(new ObjectDisposedException(nameof(GremlinqFixture)));
        }

        protected abstract Task<IGremlinQuerySource> TransformQuerySource(IConfigurableGremlinQuerySource g);

        public Task<IGremlinQuerySource> GremlinQuerySource => GetGremlinQuerySource();

        private async Task<IGremlinQuerySource> GetGremlinQuerySource()
        {
            if (Volatile.Read(ref _lazyQuerySource) is { } tcs)
                return await tcs.Task;

            var newTcs = new TaskCompletionSource<IGremlinQuerySource>();

            if (Interlocked.CompareExchange(ref _lazyQuerySource, newTcs, null) == null)
            {
                try
                {
                    var g1 = await TransformQuerySource(g);
                    newTcs.TrySetResult(g1);
                }
                catch (Exception ex)
                {
                    newTcs.TrySetException(ex);

                    Interlocked.CompareExchange(ref _lazyQuerySource, null, newTcs);
                }

                return await newTcs.Task;
            }
            else
                return await GetGremlinQuerySource();
        }

        public async Task InitializeAsync()
        {
           
        }

        public async Task DisposeAsync()
        {
            if (Interlocked.Exchange(ref _lazyQuerySource, Disposed) is { } tcs && tcs != Disposed)
            {
                if (await tcs.Task is IAsyncDisposable disposable)
                    await disposable.DisposeAsync();
            }
        }
    }
}
