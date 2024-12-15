namespace TraffiLearn.AzureFunctions.Emails
{
    internal interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
