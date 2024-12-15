namespace TraffiLearn.AzureFunctions.Emails
{
    public interface IEmailSender
    {
        Task SendEmailAsync(SendEmailRequest request);
    }
}
