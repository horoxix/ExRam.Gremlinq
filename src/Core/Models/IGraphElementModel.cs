﻿using System.Collections.Immutable;

namespace ExRam.Gremlinq.Core.Models
{
    public interface IGraphElementModel
    {
        ImmutableArray<Type> ElementTypes { get; }

        ElementMetadata GetMetadata(Type elementType);

        IGraphElementModel ConfigureMetadata(Type elementType, Func<ElementMetadata, ElementMetadata> metaDataTransformation);

        IGraphElementModel ConfigureLabels(Func<Type, string, string> overrideTransformation);
    }
}
