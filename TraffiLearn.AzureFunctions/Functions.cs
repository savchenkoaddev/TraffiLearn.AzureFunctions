using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace TraffiLearn.AzureFunctions
{
    public sealed class Functions
    {
        private readonly ILogger _logger;

        public Functions(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Functions>();
        }

        [Function(name: "EmailSendingFunction")]
        public void Run(
            [RabbitMQTrigger("traffilearn-queue", ConnectionStringSetting = "RabbitMqConnectionString")] 
            string myQueueItem)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
