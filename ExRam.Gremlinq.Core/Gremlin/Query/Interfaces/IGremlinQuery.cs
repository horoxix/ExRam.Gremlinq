﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LanguageExt;

namespace ExRam.Gremlinq.Core
{
    public partial interface IGremlinQuery : IGremlinQuerySource
    {
        IGremlinQueryAdmin AsAdmin();
        IGremlinQuery<long> Count();
        IGremlinQuery<Unit> Drop();
        IGremlinQuery<string> Explain();

        IGremlinQuery<string> Profile();

        IGremlinQuery<TStepElement> Select<TStepElement>(StepLabel<TStepElement> label);
        TQuery Select<TQuery, TElement>(StepLabel<TQuery, TElement> label) where TQuery : IGremlinQuery;

        IGremlinQuery<(T1, T2)> Select<T1, T2>(StepLabel<T1> label1, StepLabel<T2> label2);
        IGremlinQuery<(T1, T2, T3)> Select<T1, T2, T3>(StepLabel<T1> label1, StepLabel<T2> label2, StepLabel<T3> label3);
        IGremlinQuery<(T1, T2, T3, T4)> Select<T1, T2, T3, T4>(StepLabel<T1> label1, StepLabel<T2> label2, StepLabel<T3> label3, StepLabel<T4> label4);
    }

    public partial interface IGremlinQuery<TElement> : IGremlinQuery, IAsyncEnumerable<TElement>
    {
        IGremlinQuery<TElement[]> Fold();
        IGremlinQuery<TElement> Inject(params TElement[] elements);

        IGremlinQuery<TElement> SumLocal();
        IGremlinQuery<TElement> SumGlobal();
        IGremlinQuery<TItem> Unfold<TItem>();

        IGremlinQuery<TElement> Where(Expression<Func<TElement, bool>> predicate);
    }
}
