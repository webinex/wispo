using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Webinex.Wispo.Services
{
    internal interface IValuesService
    {
        IDictionary<string, object> Merge(WispoArgs args);
    }

    internal class ValuesService : IValuesService
    {
        public IDictionary<string, object> Merge(WispoArgs args)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            return new Merger(args).Merge();
        }

        private class Merger
        {
            private readonly WispoArgs _args;
            private readonly IDictionary<string, JObject> _groupValuesCache = new Dictionary<string, JObject>();

            private readonly IDictionary<ValueTuple<JObject, JObject, JObject>, JObject> _resultCache =
                new Dictionary<(JObject, JObject, JObject), JObject>();

            public Merger(WispoArgs args)
            {
                _args = args ?? throw new ArgumentNullException(nameof(args));
            }

            public IDictionary<string, object> Merge()
            {
                var values = _args.Values != null ? JObject.FromObject(_args.Values) : null;
                return _args.Recipients.ToDictionary(x => x.Id, x => RecipientMergedValue(values, x));
            }

            private object RecipientMergedValue(JObject values, WispoRecipient recipient)
            {
                var groupValues = RecipientGroupValues(recipient);
                var recipientValues = RecipientValues(recipient);
                return MergeWithCache(values, groupValues, recipientValues);
            }

            private JObject RecipientValues(WispoRecipient recipient)
            {
                return recipient.Values != null ? JObject.FromObject(recipient.Values) : null;
            }

            private JObject RecipientGroupValues(WispoRecipient recipient)
            {
                return recipient.GroupId != null ? GroupValues(recipient.GroupId) : null;
            }

            private JObject GroupValues(string id)
            {
                if (_groupValuesCache.TryGetValue(id, out var value))
                    return value;

                var group = Group(id);
                value = group.Values != null ? JObject.FromObject(group.Values) : null;
                _groupValuesCache[id] = value;
                return value;
            }

            private WispoGroup Group(string id)
            {
                return _args.Groups.First(g => g.Id == id);
            }

            private JObject MergeWithCache(JObject values, JObject groupValues, JObject recipientValues)
            {
                var key = ValueTuple.Create(values, groupValues, recipientValues);
                if (_resultCache.TryGetValue(key, out var result))
                    return result;

                result = Merge(values, groupValues, recipientValues);
                _resultCache[key] = result;
                return result;
            }

            private JObject Merge(JObject values, JObject groupValues, JObject recipientValues)
            {
                JObject result = values;
                result = MergeOrGet(result, groupValues);
                result = MergeOrGet(result, recipientValues);

                return result;
            }

            private JObject MergeOrGet(JObject into, JObject value)
            {
                if (into == null) return value;
                if (value == null) return into;

                into.Merge(value, new JsonMergeSettings
                {
                    MergeNullValueHandling = MergeNullValueHandling.Merge,
                    MergeArrayHandling = MergeArrayHandling.Replace
                });

                return into;
            }
        }
    }
}