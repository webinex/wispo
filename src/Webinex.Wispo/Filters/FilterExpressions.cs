using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Webinex.Wispo.Filters
{
    internal static class FilterExpressions
    {
        public static Expression<Func<TEntity, bool>> Eq<TEntity>(
            Expression<Func<TEntity, object>> selector,
            object value)
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var valueType = LambdaExpressions.ReturnType(selector);
            var propertyExpression = LambdaExpressions.PropertyAccessExpression(selector, parameter);
            var valueExpression = Expression.Constant(value, valueType);

            return Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    propertyExpression,
                    valueExpression),
                parameter);
        }

        public static Expression<Func<TEntity, bool>> NotEq<TEntity>(
            Expression<Func<TEntity, object>> selector,
            object value)
        {
            var eq = Eq(selector, value);
            return Expression.Lambda<Func<TEntity, bool>>(Expression.Not(eq.Body), eq.Parameters);
        }

        public static Expression<Func<TEntity, bool>> In<TEntity>(
            Expression<Func<TEntity, object>> selector,
            IEnumerable<object> values)
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var propertyAccessExpression = LambdaExpressions.PropertyAccessExpression(selector, parameter);
            var valueType = LambdaExpressions.ReturnType(selector);

            var containsMethodInfo =
                typeof(Enumerable)
                    .GetMethods()
                    .Where(x => x.Name == nameof(Enumerable.Contains) && x.IsPublic && x.IsStatic)
                    .Single(x => x.GetParameters().Length == 2)
                    .MakeGenericMethod(valueType);

            var castMethodInfo = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast))!.MakeGenericMethod(valueType);
            var typedValues = castMethodInfo.Invoke(null, new object[] { values.ToArray() });

            return Expression.Lambda<Func<TEntity, bool>>(
                Expression.Call(
                    containsMethodInfo,
                    Expression.Constant(typedValues, typeof(IEnumerable<>).MakeGenericType(valueType)),
                    propertyAccessExpression
                ),
                parameter);
        }

        public static Expression<Func<TEntity, bool>> NotIn<TEntity>(
            Expression<Func<TEntity, object>> selector,
            IEnumerable<object> values)
        {
            var inExpression = In(selector, values);
            return Expression.Lambda<Func<TEntity, bool>>(Expression.Not(inExpression.Body), inExpression.Parameters);
        }

        public static Expression<Func<TEntity, bool>> Lte<TEntity>(
            Expression<Func<TEntity, object>> selector,
            object value)
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var valueType = LambdaExpressions.ReturnType(selector);
            var propertyAccessExpression = LambdaExpressions.PropertyAccessExpression(selector, parameter);

            return Expression.Lambda<Func<TEntity, bool>>(
                Expression.LessThanOrEqual(
                    propertyAccessExpression,
                    Expression.Constant(value, valueType)),
                parameter);
        }

        public static Expression<Func<TEntity, bool>> Lt<TEntity>(
            Expression<Func<TEntity, object>> selector,
            object value)
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var valueType = LambdaExpressions.ReturnType(selector);
            var propertyAccessExpression = LambdaExpressions.PropertyAccessExpression(selector, parameter);

            return Expression.Lambda<Func<TEntity, bool>>(
                Expression.LessThan(
                    propertyAccessExpression,
                    Expression.Constant(value, valueType)),
                parameter);
        }

        public static Expression<Func<TEntity, bool>> Gte<TEntity>(
            Expression<Func<TEntity, object>> selector,
            object value)
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var valueType = LambdaExpressions.ReturnType(selector);
            var propertyAccessExpression = LambdaExpressions.PropertyAccessExpression(selector, parameter);

            return Expression.Lambda<Func<TEntity, bool>>(
                Expression.GreaterThanOrEqual(
                    propertyAccessExpression,
                    Expression.Constant(value, valueType)),
                parameter);
        }

        public static Expression<Func<TEntity, bool>> Gt<TEntity>(
            Expression<Func<TEntity, object>> selector,
            object value)
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var valueType = LambdaExpressions.ReturnType(selector);
            var propertyAccessExpression = LambdaExpressions.PropertyAccessExpression(selector, parameter);

            return Expression.Lambda<Func<TEntity, bool>>(
                Expression.GreaterThan(
                    propertyAccessExpression,
                    Expression.Constant(value, valueType)),
                parameter);
        }

        public static Expression<Func<TEntity, bool>> Contains<TEntity>(
            Expression<Func<TEntity, object>> selector,
            object value)
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var propertyAccessExpression = LambdaExpressions.PropertyAccessExpression(selector, parameter);

            var method = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
            if (method == null)
            {
                throw new InvalidOperationException();
            }

            var valueConstantExpression = Expression.Constant(value, typeof(string));
            var containsResultExpression = Expression.Call(propertyAccessExpression, method, valueConstantExpression);
            return Expression.Lambda<Func<TEntity, bool>>(containsResultExpression, parameter);
        }

        public static Expression<Func<TEntity, bool>> NotContains<TEntity>(
            Expression<Func<TEntity, object>> selector,
            object value)
        {
            var containsExpression = Contains(selector, value);
            return Expression.Lambda<Func<TEntity, bool>>(
                Expression.Not(containsExpression.Body),
                containsExpression.Parameters);
        }
    }
}