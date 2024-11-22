using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using TaskManagement.Core.Helpers;
using static TaskManagement.Core.Helpers.Error;
using TaskManagement.Core.DTOs.Projects.Repository;


namespace TaskManagement.Core.Services.Email
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailSettings _emailSettings;

        public EmailSenderService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }


        public async Task<Result<Nothing>> SendRegistrationVerificationEmailAsync(string toEmail, string fullName, string verificationCode)
        {
            try
            {
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "RegistrationVerification.html");
                var placeholders = new Dictionary<string, string>
                {
                    { "FullName", fullName },
                    { "VerificationCode", verificationCode },
                };

                var emailBodyResult = GetEmailBody(templatePath, placeholders);
                if (!emailBodyResult.IsSuccessful)
                    return Result<Nothing>.Failure(emailBodyResult.Message, emailBodyResult.Error);

                var sendEmailResult = await SendEmailAsync(toEmail, "Welcome to Task Management", emailBodyResult.Value);
                if (!sendEmailResult.IsSuccessful)
                    return Result<Nothing>.Failure(sendEmailResult.Message, sendEmailResult.Error);

                return Result<Nothing>.Success(emailBodyResult.Message);
            }
            catch (Exception ex)
            {
                return Result<Nothing>.Failure($"An unexpected error occurred: {ex.Message}", Errors.ServerError.InternalServerError);
            }

        }

        public async Task<Result<Nothing>> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
                {
                    Port = _emailSettings.Port,
                    Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                    EnableSsl = _emailSettings.EnableSsl,
                    
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(new MailAddress(toEmail));

                await smtpClient.SendMailAsync(mailMessage);
                return Result<Nothing>.Success($"An email was just sent to: {toEmail}");
            }
            catch (SmtpException smtpEx)
            {
                if (smtpEx.InnerException != null)
                    return Result<Nothing>.Failure($"Inner Exception: {smtpEx.InnerException.Message}", Errors.ServerError.InternalServerError);

                return Result<Nothing>.Failure($"SMTP error: {smtpEx.Message}", Errors.ServerError.InternalServerError);
            }
            catch (Exception ex)
            {
                return Result<Nothing>.Failure($"An unexpected error occurred: {ex.Message}", Errors.ServerError.UnhandledException);
            }

        }


        private Result<string> GetEmailBody(string templatePath, Dictionary<string, string> placeholders)
        {
            try
            {
                var template = File.ReadAllText(templatePath);
                foreach (var placeholder in placeholders)
                {
                    template = template.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
                }

                if (string.IsNullOrEmpty(template))
                    return Result<string>.Failure("Email template not found", Errors.ServerError.InternalServerError);

                return Result<string>.Success("Email body retrieved successfully", template);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"An error occurred: {ex.Message}", Errors.ServerError.InternalServerError);

            }

        }
    }
}
