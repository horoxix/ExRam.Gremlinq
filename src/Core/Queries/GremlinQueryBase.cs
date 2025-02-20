﻿using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;

namespace ExRam.Gremlinq.Core
{
    internal abstract class GremlinQueryBase
    {
        private delegate IGremlinQueryBase QueryContinuation(
            GremlinQueryBase existingQuery,
            Traversal? maybeNewTraversal,
            IImmutableDictionary<StepLabel, LabelProjections>? maybeNewLabelProjections);

        private static readonly ConcurrentDictionary<Type, QueryContinuation> QueryContinuations = new();
        private static readonly Type[] QueryGenericTypeDefinitionArguments = typeof(GremlinQuery<,,,>).GetGenericArguments();
        private static readonly QueryContinuation ObjectQueryContinuation = CreateQueryContinuation<object, object, object, IGremlinQueryBase>();
        private static readonly Type[] ImplementedInterfaces = typeof(GremlinQuery<,,,>).GetInterfaces().Append(typeof(GremlinQuery<,,,>)).ToArray();
        private static readonly MethodInfo TryCreateQueryContinuationMethod = typeof(GremlinQueryBase).GetMethod(nameof(CreateQueryContinuation), BindingFlags.NonPublic | BindingFlags.Static)!;

        protected GremlinQueryBase(
            IGremlinQueryEnvironment environment,
            Traversal steps,
            IImmutableDictionary<StepLabel, LabelProjections> labelProjections,
            IImmutableDictionary<object, object?> metadata)
        {
            Steps = steps;
            Metadata = metadata;
            Environment = environment;
            LabelProjections = labelProjections;
        }

        public override string ToString() => $"GremlinQuery(Steps.Count: {Steps.Count})";

        protected internal TTargetQuery CloneAs<TTargetQuery>(Traversal? maybeNewTraversal = null, IImmutableDictionary<StepLabel, LabelProjections>? maybeNewLabelProjections = null)
        {
            if (maybeNewTraversal == null && maybeNewLabelProjections == null && this is TTargetQuery targetQuery)
                return targetQuery;

            var queryFactory = typeof(TTargetQuery).IsGenericType
                ? QueryContinuations.GetOrAdd(
                    typeof(TTargetQuery),
                    static requestedType =>
                    {
                        var requestedTypeDefinition = requestedType.GetGenericTypeDefinition();
                        var queryTypeArguments = new Type[QueryGenericTypeDefinitionArguments.Length];

                        for (var i = 0; i < ImplementedInterfaces.Length; i++)
                        {
                            if (ImplementedInterfaces[i] is { IsGenericType: true } queryImplementedInterface && queryImplementedInterface.GetGenericTypeDefinition() == requestedTypeDefinition)
                            {
                                var matchingImplementedInterfaceTypeArguments = queryImplementedInterface.GetGenericArguments();

                                for (var j = 0; j < QueryGenericTypeDefinitionArguments.Length; j++)
                                {
                                    for (var k = 0; k < matchingImplementedInterfaceTypeArguments.Length; k++)
                                    {
                                        if (matchingImplementedInterfaceTypeArguments[k] == QueryGenericTypeDefinitionArguments[j])
                                        {
                                            queryTypeArguments[j] = requestedType.GetGenericArguments()[k];

                                            break;
                                        }
                                    }

                                    if (queryTypeArguments[j] == null)
                                    {
                                        queryTypeArguments[j] = j == 1 && queryTypeArguments[0].IsArray
                                            ? queryTypeArguments[0].GetElementType()!
                                            : QueryGenericTypeDefinitionArguments[j].GetGenericParameterConstraints().SingleOrDefault() ?? typeof(object);
                                    }
                                }

                                break;
                            }
                        }

                        return (QueryContinuation?)TryCreateQueryContinuationMethod
                            .MakeGenericMethod(queryTypeArguments)
                            .Invoke(null, null)!;
                    })
                : ObjectQueryContinuation;

            return queryFactory(this, maybeNewTraversal, maybeNewLabelProjections) is TTargetQuery newTargetQuery
                ? newTargetQuery
                : throw new NotSupportedException($"Cannot create a query of type {typeof(TTargetQuery)}.");
        }

        private static QueryContinuation CreateQueryContinuation<T1, T2, T3, T4>() where T4 : IGremlinQueryBase => (existingQuery, maybeNewTraversal, maybeNewLabelProjections) => new GremlinQuery<T1, T2, T3, T4>(
            existingQuery.Environment,
            maybeNewTraversal ?? existingQuery.Steps,
            maybeNewLabelProjections ?? existingQuery.LabelProjections,
            existingQuery.Metadata);

        protected internal Traversal Steps { get; }
        protected internal IGremlinQueryEnvironment Environment { get; }
        protected internal IImmutableDictionary<object, object?> Metadata { get; }
        protected internal IImmutableDictionary<StepLabel, LabelProjections> LabelProjections { get; }
    }
}
