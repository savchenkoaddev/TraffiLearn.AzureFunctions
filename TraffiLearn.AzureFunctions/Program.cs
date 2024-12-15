using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TraffiLearn.AzureFunctions.Emails;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        config
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.Configure<SmtpClientSettings>(
            configuration.GetSection(nameof(SmtpClientSettings)));

        services.AddSingleton<IEmailSender, EmailSender>();
    })
    .Build();

host.Run();
