using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scriban;
using Webinex.Wispo.DataAccess;

namespace Webinex.Wispo.Services.Templates
{
    internal class DefaultTemplateService : ITemplateService
    {
        public Task<IDictionary<string, NotificationRow>> RenderAsync(
            WispoArgs args,
            IDictionary<string, object> valuesByRecipientId)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            valuesByRecipientId = valuesByRecipientId ?? throw new ArgumentNullException(nameof(valuesByRecipientId));

            var result = new Renderer(args, valuesByRecipientId).Render();
            return Task.FromResult(result);
        }

        private class Renderer
        {
            private readonly WispoArgs _args;
            private readonly IDictionary<string, object> _valuesByRecipientId;
            private readonly IDictionary<object, TemplateResult> _cache = new Dictionary<object, TemplateResult>();

            private Template _subjectTemplate;
            private Template _bodyTemplate;

            public Renderer(WispoArgs args, IDictionary<string, object> valuesByRecipientId)
            {
                _args = args;
                _valuesByRecipientId = valuesByRecipientId;
            }

            public IDictionary<string, NotificationRow> Render()
            {
                _subjectTemplate = Template.Parse(_args.Subject);
                _bodyTemplate = Template.Parse(_args.Body);

                return _args.Recipients.ToDictionary(x => x.Id, Render);
            }

            private NotificationRow Render(WispoRecipient recipient)
            {
                var templateResult = RenderTemplateWithCache(recipient);
                return new NotificationRow
                {
                    Id = Guid.NewGuid(),
                    Body = templateResult.Body,
                    Subject = templateResult.Subject,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    RecipientId = recipient.Id
                };
            }

            private TemplateResult RenderTemplateWithCache(WispoRecipient recipient)
            {
                var values = _valuesByRecipientId[recipient.Id];
                if (values == null) return new TemplateResult(_args.Subject, _args.Body);
                
                if (_cache.TryGetValue(values, out var result))
                    return result;

                result = RenderTemplate(values);
                _cache[values] = result;
                return result;
            }

            private TemplateResult RenderTemplate(object values)
            {
                var data = new { values };
                var subject = _subjectTemplate.Render(data);
                var body = _bodyTemplate.Render(data);
                return new TemplateResult(subject, body);
            }
        }

        private class TemplateResult
        {
            public TemplateResult(string subject, string body)
            {
                Subject = subject ?? throw new ArgumentNullException(nameof(subject));
                Body = body ?? throw new ArgumentNullException(nameof(body));
            }

            public string Subject { get; }
            public string Body { get; }
        }
    }
}