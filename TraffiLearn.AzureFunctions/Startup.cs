using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TraffiLearn.AzureFunctions.Emails;

[assembly: FunctionsStartup(typeof(TraffiLearn.AzureFunctions.Startup))]

namespace TraffiLearn.AzureFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;

            builder.Services.Configure<EmailSettings>(
                configuration.GetSection(nameof(EmailSettings)));

            builder.Services.AddSingleton<IEmailSender, SendGridSender>();
        }
    }
}
