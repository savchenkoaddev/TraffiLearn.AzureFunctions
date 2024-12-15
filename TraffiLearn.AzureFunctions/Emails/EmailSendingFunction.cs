using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TraffiLearn.AzureFunctions.Emails
{
    public sealed class EmailSendingFunction
    {
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;

        public EmailSendingFunction(
            ILoggerFactory loggerFactory,
            IEmailSender emailSender)
        {
            _logger = loggerFactory.CreateLogger<EmailSendingFunction>();
            _emailSender = emailSender;
        }

        [Function(name: "EmailSendingFunction")]
        public async Task Run(
            [RabbitMQTrigger("traffilearn-queue", ConnectionStringSetting = "RabbitMqConnectionString")]
            string myQueueItem)
        {
            _logger.LogDebug($"Processing message from queue.");

            try
            {
                var sendEmailRequest = JsonConvert.DeserializeObject<SendEmailRequest>(myQueueItem);

                await _emailSender.SendEmailAsync(sendEmailRequest);
            }
            catch (JsonSerializationException)
            {
                _logger.LogError("Failed to deserialize the message into SendEmailRequest.");
            }
        }
    }
}
