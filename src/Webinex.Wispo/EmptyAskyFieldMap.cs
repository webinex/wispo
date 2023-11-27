using System;
using System.Linq.Expressions;
using Webinex.Asky;

namespace Webinex.Wispo;

internal class EmptyAskyFieldMap<TData> : IAskyFieldMap<TData>
{
    public Expression<Func<TData, object>>? this[string fieldId] => null;
}