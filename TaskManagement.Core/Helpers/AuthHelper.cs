using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Shared;
using TaskManagement.Core.DTOs.Users.Controllers;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Helpers
{
    public static class AuthHelper
    {
        public static string Generate6DigitCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public static bool VerifyPassword(string password, string passwordHash)
        {
            return password == EncryptionHelper.Decrypt(passwordHash);
        }


    }
}
