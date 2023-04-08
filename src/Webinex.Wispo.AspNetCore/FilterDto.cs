using System;
using System.Linq;
using System.Text.Json;

namespace Webinex.Wispo.AspNetCore
{
    public class FilterDto
    {
        public string Operator { get; set; }
        
        public string FieldId { get; set; }
        
        public JsonElement Value { get; set; }
        
        public JsonElement Values { get; set; }
        
        public FilterDto[] Children { get; set; }

        public static FilterDto FromJson(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            
            return JsonSerializer.Deserialize<FilterDto>(value, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        public Filter ToFilter()
        {
            if (FilterOperator.ALL_VALUES.Contains(Operator))
                return ToValueFilter();

            if (FilterOperator.ALL_IN.Contains(Operator))
                return ToInFilter();

            if (FilterOperator.ALL_LOGICAL.Contains(Operator))
                return ToLogicalFilter();

            throw new InvalidOperationException($"Unknown operator {Operator}");
        }

        private ValueFilter ToValueFilter()
        {
            var valueType = NotificationField.Types[FieldId];
            var value = JsonSerializer.Deserialize(Value.GetRawText(), valueType);
            return new ValueFilter(FieldId, Operator, value);
        }

        private InFilter ToInFilter()
        {
            var valueType = NotificationField.Types[FieldId];
            var values = Values.EnumerateArray().Select(x => JsonSerializer.Deserialize(x.GetRawText(), valueType)).ToArray();
            return new InFilter(FieldId, Operator, values);
        }

        private LogicalFilter ToLogicalFilter()
        {
            var children = Children.Select(x => x.ToFilter()).ToArray();
            return new LogicalFilter(Operator, children);
        }
    }
}