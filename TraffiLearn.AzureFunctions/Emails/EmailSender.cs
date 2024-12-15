using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Polly;

namespace TraffiLearn.AzureFunctions.Emails
{
    internal sealed class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly SmtpClientSettings _emailSettings;
        private readonly AsyncPolicy _retryPolicy;

        public EmailSender(
            ILogger<EmailSender> logger,
            IOptions<SmtpClientSettings> options)
        {
            _logger = logger;
            _emailSettings = options.Value;

            _retryPolicy = Policy
                .Handle<Exception>(ex => ex is SmtpCommandException || ex is SmtpProtocolException || ex is TimeoutException)
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(
                            "Retry {RetryCount} after {TimeSpan} due to exception: {Exception}", retryCount, timeSpan, exception.Message);
                    });
        }

        public async Task SendEmailAsync(SendEmailRequest request)
        {
            var email = CreateEmailMessage(request);

            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    using var smtp = new SmtpClient();

                    await ConnectAndAuthenticateAsync(smtp);
                    await SendEmailAsync(smtp, email, request.RecipientEmail);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", request.RecipientEmail);
                throw;
            }
        }

        private MimeMessage CreateEmailMessage(SendEmailRequest request)
        {
            var email = new MimeMessage
            {
                From = { new MailboxAddress(nameof(TraffiLearn), _emailSettings.Username) },
                To = { new MailboxAddress(string.Empty, request.RecipientEmail) },
                Subject = request.Subject,
                Body = new TextPart("html") 
                {
                    Text = request.HtmlBody 
                }
            };

            return email;
        }

        private async Task ConnectAndAuthenticateAsync(SmtpClient smtp)
        {
            try
            {
                var secureSocketOptions = _emailSettings.EnableSsl 
                    ? SecureSocketOptions.StartTls 
                    : SecureSocketOptions.None;

                await smtp.ConnectAsync(
                    host: _emailSettings.Host, 
                    port: _emailSettings.Port, 
                    options: secureSocketOptions);

                await smtp.AuthenticateAsync(
                    userName: _emailSettings.Username, 
                    password: _emailSettings.Password);

                _logger.LogInformation("Successfully connected and authenticated to SMTP server.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect or authenticate to SMTP server.");

                throw;
            }
        }

        private async Task SendEmailAsync(
            SmtpClient smtp, 
            MimeMessage email, 
            string recipientEmail)
        {
            try
            {
                _logger.LogInformation("Sending email to {To}", recipientEmail);

                await smtp.SendAsync(email);

                _logger.LogInformation("Email sent successfully to {To}", recipientEmail);

                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", recipientEmail);

                throw;
            }
        }
    }
}
