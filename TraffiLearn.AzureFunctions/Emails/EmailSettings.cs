namespace TraffiLearn.AzureFunctions.Emails
{
    internal sealed class EmailSettings
    {
        public string? ApiKey { get; init; }

        public string? FromEmail { get; init; }

        public string? FromName { get; init; }
    }
}
