using PostmarkDotNet;
using Scriban;
using Serilog;
using WebApp.Entreo.Configuration;
using File = System.IO.File;

namespace Entreo.Services.Services
{

    public class EmailService
    {
        public string GetEmailContent(
            string emailName,
            params (string name, string value)[] items)
        {
            var dictionary = items.ToDictionary(
                i => i.name,
                i => i.value);

            return GetEmailContent(emailName, dictionary);
        }

        public string GetEmailContentScriban(string emailName, object model)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"Mails\\{emailName}.html");
            var text = File.ReadAllText(path);

            var template = Template.Parse(text);
            return template.Render(model);
        }

        public string GetEmailContent(
            string emailName,
            IDictionary<string, string>? items = null)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"Mails\\{emailName}.html");
            var text = File.ReadAllText(path);

            if (items != null)
            {
                foreach (var item in items)
                {
                    text = text.Replace($"{{{item.Key}}}", item.Value);
                }
            }

            return text;
        }

        public async Task SendEmailAsync(EmailOptions emailOptions, string? unSubscribeLink = null, string? text = null)
        {
            var from = !string.IsNullOrEmpty(emailOptions.From)
                ? emailOptions.From
                : AppSettings.EmailSettings.FromEmail;

            var layoutFile = "layout";

            var dictionary = new Dictionary<string, string> { { "body", emailOptions.Body }, { "footer", "" } };
            if (unSubscribeLink != null)
            {
                var layoutFooter = GetEmailContent("footerWithUnscribeLink", new Dictionary<string, string> { { "linkToUnsubscribe", unSubscribeLink } });
                dictionary = new Dictionary<string, string> { { "body", emailOptions.Body }, { "footer", layoutFooter } };
            }

            var layout = GetEmailContent(layoutFile, dictionary);

            var inlinedCssLayout = PreMailer.Net.PreMailer
                .MoveCssInline(layout, removeStyleElements: true)
                .Html;

            var email = new PostmarkMessage
            {
                From = from,
                To = string.Join(",", emailOptions.To),
                Cc = string.Join(",", emailOptions.Cc),
                Bcc = string.Join(",", emailOptions.Bcc),
                ReplyTo = emailOptions.ReplyTo,
                Subject = emailOptions.Subject,
                HtmlBody = inlinedCssLayout
            };

            if (!AppSettings.EmailSettings.SendEmails)
            {
                return;
            }

            try
            {
                var postmarkClient = new PostmarkClient(AppSettings.EmailSettings.PostmarkServerKey);
                if (!string.IsNullOrEmpty(email.To) && !email.To.Contains("+DoNotSendEmail@")) //Like entreo.test+DoNotSendEmail@gmail.com
                {
                    var result = await postmarkClient.SendMessageAsync(email);
                    if (result.ErrorCode != 0)
                    {
                        throw new Exception("Could not send email");
                    }
                    Log.Information("Send email '{Subject}' to {Email}", emailOptions.Subject, email.To);
                }
            }
            catch (Exception e)
            {
                Log.Error("Sending email '{Subject}' to {Email} failed: {Error}", emailOptions.Subject, email.To, e.Message);
            }
        }

    }

    public class EmailSettings
    {
        public string PostmarkServerKey { get; set; } = string.Empty;
        public bool SendEmails { get; set; }
        public string FromEmail { get; set; } = string.Empty;
    }

    public class EmailOptions
    {
        public string? From { get; set; }
        public string? ReplyTo { get; set; }
        public IList<string> To { get; set; } = new List<string>();
        public IList<string> Cc { get; set; } = new List<string>();
        public IList<string> Bcc { get; set; } = new List<string>();
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? TemplateName { get; set; }
        public string? RelateReference { get; set; }
        public string? RelateId { get; set; }
    }

}
