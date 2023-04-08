using System.Collections.Generic;
using System.Linq;

namespace Webinex.Wispo
{
    public class Filter
    {
        public static ValueFilter Eq(string fieldId, object value)
        {
            return new ValueFilter(fieldId, FilterOperator.EQ, value);
        }
        
        public static ValueFilter NotEq(string fieldId, object value)
        {
            return new ValueFilter(fieldId, FilterOperator.NOT_EQ, value);
        }
        
        public static ValueFilter Gt(string fieldId, object value)
        {
            return new ValueFilter(fieldId, FilterOperator.GT, value);
        }
        
        public static ValueFilter Gte(string fieldId, object value)
        {
            return new ValueFilter(fieldId, FilterOperator.GTE, value);
        }
        
        public static ValueFilter Lt(string fieldId, object value)
        {
            return new ValueFilter(fieldId, FilterOperator.LT, value);
        }
        
        public static ValueFilter Lte(string fieldId, object value)
        {
            return new ValueFilter(fieldId, FilterOperator.LTE, value);
        }
        
        public static ValueFilter Contains(string fieldId, object value)
        {
            return new ValueFilter(fieldId, FilterOperator.CONTAINS, value);
        }
        
        public static ValueFilter Contains(string fieldId, string value)
        {
            return new ValueFilter(fieldId, FilterOperator.CONTAINS, value);
        }
        
        public static ValueFilter NotContains(string fieldId, object value)
        {
            return new ValueFilter(fieldId, FilterOperator.NOT_CONTAINS, value);
        }
        
        public static ValueFilter NotContains(string fieldId, string value)
        {
            return new ValueFilter(fieldId, FilterOperator.NOT_CONTAINS, value);
        }
        
        public static InFilter In<T>(string fieldId, T[] values)
        {
            return new InFilter(fieldId, FilterOperator.IN, values.Cast<object>().ToArray());
        }
        
        public static InFilter In(string fieldId, object[] values)
        {
            return new InFilter(fieldId, FilterOperator.IN, values);
        }
        
        public static InFilter NotIn<T>(string fieldId, T[] values)
        {
            return new InFilter(fieldId, FilterOperator.NOT_IN, values.Cast<object>().ToArray());
        }
        
        public static InFilter NotIn(string fieldId, object[] values)
        {
            return new InFilter(fieldId, FilterOperator.NOT_IN, values);
        }
        
        public static LogicalFilter Or(params Filter[] filters)
        {
            return new LogicalFilter(FilterOperator.OR, filters);
        }
        
        public static LogicalFilter And(params Filter[] filters)
        {
            return new LogicalFilter(FilterOperator.AND, filters);
        }
    }

    public class InFilter : Filter
    {
        protected InFilter()
        {
        }
        
        public InFilter(string fieldId, string @operator, IEnumerable<object> values)
        {
            FieldId = fieldId;
            Operator = @operator;
            Values = values;
        }

        public string Operator { get; protected set; }
        
        public string FieldId { get; protected set; }
        
        public IEnumerable<object> Values { get; protected set; }
    }

    public class LogicalFilter : Filter
    {
        protected LogicalFilter()
        {
        }
        
        public LogicalFilter(string @operator, Filter[] children)
        {
            Operator = @operator;
            Children = children;
        }

        public string Operator { get; protected set; }
        
        public Filter[] Children { get; protected set; }
    }

    public class ValueFilter : Filter
    {
        protected ValueFilter()
        {
        }
        
        public ValueFilter(string fieldId, string @operator, object value)
        {
            FieldId = fieldId;
            Operator = @operator;
            Value = value;
        }

        public string FieldId { get; protected set; }
        
        public string Operator { get; protected set; }
        
        public object Value { get; protected set; }
    }
}