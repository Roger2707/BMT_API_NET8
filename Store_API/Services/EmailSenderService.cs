using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;
namespace Store_API.Services
{
    public class EmailSenderService : IEmailSender
    {
        private readonly ILogger<EmailSenderService> _logger;
        private readonly SmtpClient _smtpClient;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailSenderService(ILogger<EmailSenderService> logger, IConfiguration configuration)
        {
            _logger = logger;
            try
            {
                _fromEmail = configuration["Email:FromEmail"] ?? "huynglesongtan8889@gmail.com";
                _fromName = configuration["Email:FromName"] ?? "ROGER BMT Store";
                var emailPassword = configuration["Email:Password"] ?? "qyra jbjz gxyj xihj";

                _smtpClient = new SmtpClient
                {
                    Port = 587,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_fromEmail, emailPassword),
                    Timeout = 10000 // 10 seconds timeout
                };

                _logger.LogInformation("SmtpClient initialized successfully with email: {Email}", _fromEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize SmtpClient");
                throw;
            }
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                _logger.LogInformation("Starting email send process to {Email}", email);

                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogError("Email address is null or empty");
                    return;
                }

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                _logger.LogInformation("Attempting to send email via SMTP");
                await _smtpClient.SendMailAsync(mailMessage);
                
                _logger.LogInformation("Email sent successfully to {Email}", email);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP error sending email to {Email}. Error message: {Message}", 
                    email, ex.Message);
                
                // Check for specific SMTP errors based on the error message
                if (ex.Message.Contains("authentication"))
                {
                    _logger.LogError("SMTP authentication failed. Please check your Gmail account settings:");
                    _logger.LogError("1. Make sure 2-Step Verification is enabled");
                    _logger.LogError("2. Generate an App Password for this application");
                    _logger.LogError("3. Update the password in appsettings.json");
                }
                else if (ex.Message.Contains("mailbox unavailable"))
                {
                    _logger.LogWarning("Mailbox unavailable for {Email}", email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending email to {Email}. Error type: {ErrorType}", 
                    email, ex.GetType().Name);
            }
            finally
            {
                _logger.LogInformation("Email send process completed for {Email}", email);
            }
        }
    }
}
