using System;
using System.Linq;
using System.Linq.Expressions;
using Webinex.Wispo.DataAccess;
using static Webinex.Wispo.FilterOperator;

namespace Webinex.Wispo.Filters
{
    internal class FilterFactory : IFilterFactory
    {
        private readonly IFieldMap _fieldMap;

        public FilterFactory(IFieldMap fieldMap)
        {
            _fieldMap = fieldMap;
        }

        public Expression<Func<NotificationRow, bool>> Create(Filter filter)
        {
            filter = filter ?? throw new ArgumentNullException(nameof(filter));
            return CreateInternal(filter);
        }

        private Expression<Func<NotificationRow, bool>> CreateInternal(Filter filter)
        {
            return filter switch
            {
                LogicalFilter logicalFilterNode => Create(logicalFilterNode),
                ValueFilter filterNode => Create(filterNode),
                InFilter collectionFilterNode => Create(collectionFilterNode),
                _ => throw new ArgumentOutOfRangeException(nameof(filter))
            };
        }

        private Expression<Func<NotificationRow, bool>> Create(LogicalFilter filter)
        {
            var expressions = filter.Children.Select(CreateInternal).ToArray();

            return filter.Operator switch
            {
                OR => ExpressionsUtils.Or(expressions),
                AND => ExpressionsUtils.And(expressions),
                _ => throw new ArgumentException($"Unknown keyword {filter.Operator}", nameof(filter))
            };
        }

        private Expression<Func<NotificationRow, bool>> Create(ValueFilter filter)
        {
            var field = _fieldMap[filter.FieldId];
            var value = filter.Value;

            return filter.Operator switch
            {
                EQ => FilterExpressions.Eq(field, value),
                NOT_EQ => FilterExpressions.NotEq(field, value),
                GTE => FilterExpressions.Gte(field, value),
                GT => FilterExpressions.Gt(field, value),
                LTE => FilterExpressions.Lte(field, value),
                LT => FilterExpressions.Lt(field, value),
                CONTAINS => FilterExpressions.Contains(field, value),
                NOT_CONTAINS => FilterExpressions.NotContains(field, value),
                _ => throw new ArgumentOutOfRangeException(nameof(filter.Operator)),
            };
        }

        private Expression<Func<NotificationRow, bool>> Create(InFilter filter)
        {
            var field = _fieldMap[filter.FieldId];
            var values = filter.Values;

            return filter.Operator switch
            {
                IN => FilterExpressions.In(field, values),
                NOT_IN => FilterExpressions.NotIn(field, values),
                _ => throw new ArgumentOutOfRangeException(nameof(filter.Operator)),
            };
        }
    }
}