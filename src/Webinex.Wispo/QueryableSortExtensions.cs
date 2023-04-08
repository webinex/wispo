using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Webinex.Wispo
{
    internal static class QueryableSortExtensions
    {
        public static IOrderedQueryable<TEntity> SortBy<TEntity>(
            this IQueryable<TEntity> queryable,
            IEnumerable<SortByArgs<TEntity>> args)
        {
            queryable = queryable ?? throw new ArgumentNullException(nameof(queryable));
            args = args?.ToArray() ?? throw new ArgumentNullException(nameof(args));

            if (!args.Any())
                throw new ArgumentException("Might contain at least one items", nameof(args));

            return args.Aggregate(queryable.SortBy(args.ElementAt(0).Selector, args.ElementAt(0).Dir),
                (q, arg) => q.ThenSortBy(arg.Selector, arg.Dir));
        }

        public static IOrderedQueryable<TEntity> SortBy<TEntity>(
            this IQueryable<TEntity> queryable,
            Expression<Func<TEntity, object>> selector,
            SortDir sortDir)
        {
            queryable = queryable ?? throw new ArgumentNullException(nameof(queryable));
            selector = selector ?? throw new ArgumentNullException(nameof(selector));

            if (!Enum.IsDefined(typeof(SortDir), sortDir))
            {
                throw new ArgumentException($"Unknown sort dir {sortDir}", nameof(sortDir));
            }

            return SortByInternal(queryable, selector, sortDir);
        }

        private static IOrderedQueryable<TEntity> SortByInternal<TEntity>(
            IQueryable<TEntity> queryable,
            Expression<Func<TEntity, object>> selector,
            SortDir sortDir)
        {
            return (IOrderedQueryable<TEntity>)typeof(QueryableSortExtensions).GetMethod(nameof(___SortBy),
                    BindingFlags.Static | BindingFlags.NonPublic)!
                .MakeGenericMethod(typeof(TEntity), LambdaExpressions.ReturnType(selector)).Invoke(null,
                    new[] { queryable, LambdaExpressions.ReplaceReturnTypeToTyped(selector), sortDir });
        }

        private static IOrderedQueryable<TEntity> ___SortBy<TEntity, TKey>(
            IQueryable<TEntity> queryable,
            Expression<Func<TEntity, TKey>> selector,
            SortDir sortDir)
        {
            return sortDir == SortDir.Asc
                ? queryable.OrderBy(selector)
                : queryable.OrderByDescending(selector);
        }

        public static IOrderedQueryable<TEntity> ThenSortBy<TEntity>(
            this IOrderedQueryable<TEntity> queryable,
            Expression<Func<TEntity, object>> selector,
            SortDir sortDir)
        {
            queryable = queryable ?? throw new ArgumentNullException(nameof(queryable));
            selector = selector ?? throw new ArgumentNullException(nameof(selector));

            if (!Enum.IsDefined(typeof(SortDir), sortDir))
            {
                throw new ArgumentException($"Unknown sort dir {sortDir}", nameof(sortDir));
            }

            return ThenSortByInternal(queryable, selector, sortDir);
        }

        private static IOrderedQueryable<TEntity> ThenSortByInternal<TEntity>(
            IOrderedQueryable<TEntity> queryable,
            Expression<Func<TEntity, object>> selector,
            SortDir sortDir)
        {
            return (IOrderedQueryable<TEntity>)typeof(QueryableSortExtensions).GetMethod(nameof(___ThenSortBy),
                    BindingFlags.Static | BindingFlags.NonPublic)!
                .MakeGenericMethod(typeof(TEntity), LambdaExpressions.ReturnType(selector)).Invoke(null,
                    new[] { queryable, LambdaExpressions.ReplaceReturnTypeToTyped(selector), sortDir });
        }

        private static IOrderedQueryable<TEntity> ___ThenSortBy<TEntity, TKey>(
            IOrderedQueryable<TEntity> queryable,
            Expression<Func<TEntity, TKey>> selector,
            SortDir sortDir)
        {
            return sortDir == SortDir.Asc
                ? queryable.ThenBy(selector)
                : queryable.ThenByDescending(selector);
        }
    }
}