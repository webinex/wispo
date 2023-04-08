using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Wispo
{
    public enum SortDir
    {
        Asc = 1,
        Desc = 0,
    }
    
    public class SortRule
    {
        public SortRule([NotNull] string fieldId, SortDir dir)
        {
            if (!Enum.IsDefined(typeof(SortDir), dir))
            {
                throw new ArgumentException($"{dir} is not defined", nameof(dir));
            }
            
            FieldId = fieldId ?? throw new ArgumentNullException(nameof(fieldId));
            Dir = dir;
        }

        [NotNull]
        public string FieldId { get; }
        
        public SortDir Dir { get; }

        public static SortRule Asc(string fieldId)
        {
            return new SortRule(fieldId, SortDir.Asc);
        }

        public static SortRule Desc(string fieldId)
        {
            return new SortRule(fieldId, SortDir.Desc);
        }
    }
}