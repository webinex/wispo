using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Webinex.Wispo.DataAccess;

namespace Webinex.Wispo
{
    internal interface ISortService
    {
        IOrderedQueryable<NotificationRow> Apply(
            [NotNull] IQueryable<NotificationRow> queryable,
            [NotNull] SortRule[] sort);
    }

    internal class SortService : ISortService
    {
        private readonly IFieldMap _fieldMap;

        public SortService(IFieldMap fieldMap)
        {
            _fieldMap = fieldMap;
        }

        public IOrderedQueryable<NotificationRow> Apply(IQueryable<NotificationRow> queryable, SortRule[] sort)
        {
            queryable = queryable ?? throw new ArgumentNullException(nameof(queryable));
            sort = sort ?? throw new ArgumentNullException(nameof(sort));

            if (sort.Any(x => x == null))
                throw new ArgumentException("Might not contain nulls", nameof(sort));

            if (!sort.Any())
                throw new ArgumentException("Might have at least one sort rule", nameof(sort));

            var args = sort
                .Select(rule => new SortByArgs<NotificationRow>(_fieldMap[rule.FieldId], rule.Dir))
                .ToArray();

            return queryable.SortBy(args);
        }
    }
}