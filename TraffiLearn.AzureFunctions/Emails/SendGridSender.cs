using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace TraffiLearn.AzureFunctions.Emails
{
    internal sealed class SendGridSender : IEmailSender
    {
        private readonly ILogger<SendGridSender> _logger;
        private readonly EmailSettings _emailSettings;

        public SendGridSender(
            ILogger<SendGridSender> logger,
            IOptions<EmailSettings> options)
        {
            _logger = logger;
            _emailSettings = options.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var client = new SendGridClient(_emailSettings.ApiKey);

            var from = new EmailAddress(
                _emailSettings.FromEmail,
                _emailSettings.FromName);

            var toEmail = new EmailAddress(to);

            var msg = MailHelper.CreateSingleEmail(
                from, toEmail, subject, body, body);

            try
            {
                var response = await client.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email sent successfully.");
                }
                else
                {
                    _logger.LogError($"Failed to send email: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
            }
        }
    }
}
