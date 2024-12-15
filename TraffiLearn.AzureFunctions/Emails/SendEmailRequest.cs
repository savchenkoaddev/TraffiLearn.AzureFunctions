    namespace TraffiLearn.AzureFunctions.Emails
    {
        public sealed class SendEmailRequest
        {
            public string RecipientEmail { get; init; }

            public string Subject { get; init; }

            public string HtmlBody { get; init; }
        }
    }
