﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ExRam.Gremlinq.Core.Tests
{
    public abstract class QueryDeserializationTest : QueryExecutionTest
    {
        [ThreadStatic]
        private static Context? XUnitContext;

        protected QueryDeserializationTest(IConfigurableGremlinQuerySource g, ITestOutputHelper testOutputHelper, [CallerFilePath] string callerFilePath = "") : base(
            g
                .ConfigureEnvironment(env => env
                    .UseExecutor(GremlinQueryExecutor.Create((_, _) =>
                    {
                        if (XUnitContext is { } context)
                        {
                            var prefix = context.ClassName.Substring(0, context.ClassName.Length - "DeserializationTests".Length);

                            try
                            {
                                var jArray = JsonConvert.DeserializeObject<JArray>(
                                    File.ReadAllText(System.IO.Path.Combine(context.SourceDirectory, prefix + "IntegrationTests." + XUnitContext.MethodName + ".verified.json")));

                                return jArray
                                    .Select(x => (object)x)
                                    .ToAsyncEnumerable();
                            }
                            catch (IOException)
                            {
                                return AsyncEnumerable.Empty<object>();
                            }
                        }

                        throw new InvalidOperationException();
                    }))),
            testOutputHelper,
            callerFilePath)
        {
            XUnitContext = Context;
        }
    }
}
