using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.Services.Email
{
    public interface IEmailSenderService
    {
        Task<Result<Nothing>> SendEmailAsync(string toEmail, string subject, string body);
        Task<Result<Nothing>> SendRegistrationVerificationEmailAsync(string toEmail, string fullName, string verificationCode);
        Task<Result<Nothing>> SendResetPasswordEmailAsync(string toEmail, string resetCode);
    }
}
