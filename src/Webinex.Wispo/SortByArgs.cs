using System;
using System.Linq.Expressions;

namespace Webinex.Wispo
{
    internal class SortByArgs<TEntity>
    {
        public SortByArgs(Expression<Func<TEntity, object>> selector, SortDir dir)
        {
            Selector = selector ?? throw new ArgumentNullException(nameof(selector));
            Dir = dir;
        }

        public Expression<Func<TEntity, object>> Selector { get; }
        
        public SortDir Dir { get; }
    }
}