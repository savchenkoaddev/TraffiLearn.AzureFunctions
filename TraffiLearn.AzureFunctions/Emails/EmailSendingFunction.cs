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
            [ServiceBusTrigger(
                queueName: "traffilearn-emails-queue", Connection = "ServiceBusConnectionString")]
            string myQueueItem)
        {
            _logger.LogDebug($"Processing message from queue.");

            try
            {
                var envelope = JsonConvert.DeserializeObject<MessageEnvelope<SendEmailRequest>>(myQueueItem);

                var sendEmailRequest = envelope?.Message;

                if (sendEmailRequest is null)
                {
                    _logger.LogError("Message deserialization failed. The envelope does not contain a valid message.");

                    return;
                }

                await _emailSender.SendEmailAsync(sendEmailRequest);
            }
            catch (JsonSerializationException)
            {
                _logger.LogError("Failed to deserialize the message into SendEmailRequest.");
            }
        }
    }
}
