﻿using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Diagnostics.CodeAnalysis;
using ExRam.Gremlinq.Core.Transformation;
using ExRam.Gremlinq.Core;

namespace ExRam.Gremlinq.Support.NewtonsoftJson
{
    internal sealed class ExpandoObjectConverterFactory : IConverterFactory
    {
        private sealed class ExpandoObjectConverter<TTarget> : IConverter<JObject, TTarget>
        {
            private readonly IGremlinQueryEnvironment _environment;

            public ExpandoObjectConverter(IGremlinQueryEnvironment environment)
            {
                _environment = environment;
            }

            public bool TryConvert(JObject serialized, ITransformer recurse, [NotNullWhen(true)] out TTarget? value)
            {
                var expando = new ExpandoObject();

                foreach (var property in serialized)
                {
                    if (property.Value is { } propertyValue && recurse.TryTransform<JToken, object>(propertyValue, _environment, out var item))
                        expando.TryAdd(property.Key, item);
                }

                value = (TTarget)(object)expando;
                return true;
            }
        }

        public IConverter<TSource, TTarget>? TryCreate<TSource, TTarget>(IGremlinQueryEnvironment environment)
        {
            return typeof(TSource) == typeof(JObject) && typeof(TTarget).IsAssignableFrom(typeof(ExpandoObject)) && typeof(TTarget) != typeof(IDictionary<string, object?>)
                ? (IConverter<TSource, TTarget>)(object)new ExpandoObjectConverter<TTarget>(environment)
                : default;
        }
    }
}
