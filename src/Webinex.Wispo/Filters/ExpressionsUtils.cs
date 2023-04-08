using System;
using System.Linq;
using System.Linq.Expressions;

namespace Webinex.Wispo.Filters
{
    internal static class ExpressionsUtils
    {
        public static Expression<Func<T, bool>> And<T>(
            Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            return GetAggregatedExpression(Expression.AndAlso, expr1, expr2);
        }

        public static Expression<Func<T, bool>> Or<T>(
            Expression<Func<T, bool>>[] expressions)
        {
            if (expressions.Length < 2)
            {
                throw new ArgumentException("Might be at least 2 expressions", nameof(expressions));
            }

            return expressions.Skip(1).Aggregate(expressions.ElementAt(0), Or);
        }

        public static Expression<Func<T, bool>> And<T>(
            Expression<Func<T, bool>>[] expressions)
        {
            if (expressions.Length < 2)
            {
                throw new ArgumentException("Might be at least 2 expressions", nameof(expressions));
            }

            return expressions.Skip(1).Aggregate(expressions.ElementAt(0), And);
        }

        public static Expression<Func<T, bool>> Or<T>(
            Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            return GetAggregatedExpression(Expression.Or, expr1, expr2);
        }

        private static Expression<Func<T, bool>> GetAggregatedExpression<T>(
            Func<Expression, Expression, BinaryExpression> aggregate,
            Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));
            var left = ReplaceParameter(expr1.Body, expr1.Parameters[0], parameter);
            var right = ReplaceParameter(expr2.Body, expr2.Parameters[0], parameter);

            return Expression.Lambda<Func<T, bool>>(aggregate(left, right), parameter);
        }

        internal static Expression ReplaceParameter(
            Expression expression,
            ParameterExpression oldParameter,
            ParameterExpression newParameter)
        {
            return new ReplaceExpressionVisitor(oldParameter, newParameter).Visit(expression);
        }

        private class ReplaceExpressionVisitor
            : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                {
                    return _newValue;
                }

                return base.Visit(node);
            }
        }
    }
}