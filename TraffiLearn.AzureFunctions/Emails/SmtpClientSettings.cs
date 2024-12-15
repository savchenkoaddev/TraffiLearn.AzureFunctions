namespace TraffiLearn.AzureFunctions.Emails
{
    internal sealed class SmtpClientSettings
    {
        public string Host { get; init; }

        public string Username { get; init; }

        public string Password { get; init; }

        public bool EnableSsl { get; init; }

        public int Port { get; init; }
    }
}
