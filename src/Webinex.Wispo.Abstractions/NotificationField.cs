using System;
using System.Collections.Generic;

namespace Webinex.Wispo
{
    public static class NotificationField
    {
        public const string ID = "id";
        public const string RECIPIENT_ID = "recipientId";
        public const string SUBJECT = "subject";
        public const string BODY = "body";
        public const string IS_READ = "isRead";
        public const string CREATED_AT = "createdAt";
        public const string READ_BY_ID = "readById";
        public const string READ_AT = "readAt";

        public static HashSet<string> All => new HashSet<string>
        {
            ID, RECIPIENT_ID, SUBJECT, BODY,
            IS_READ, CREATED_AT, READ_BY_ID, READ_AT,
        };

        public static IDictionary<string, Type> Types = new Dictionary<string, Type>
        {
            [ID] = typeof(Guid),
            [RECIPIENT_ID] = typeof(string),
            [SUBJECT] = typeof(string),
            [BODY] = typeof(string),
            [IS_READ] = typeof(bool),
            [CREATED_AT] = typeof(DateTime),
            [READ_BY_ID] = typeof(string),
            [READ_AT] = typeof(DateTime?),
        };
    }
}