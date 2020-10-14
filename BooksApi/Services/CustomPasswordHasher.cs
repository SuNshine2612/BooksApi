using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Services
{
    /// <summary>
    /// hash password chế biến by cuongnq
    /// </summary>
    public static class CustomPasswordHasher
    {
        /// <summary>
        /// Mã hóa password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                password = string.Empty;

            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            //"Salt: {Convert.ToBase64String(salt)}"

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            //"Hashed: {hashed}"

            string outSalt = Convert.ToBase64String(salt);
            hashed = outSalt + MD5Hash(hashed);

            return hashed;
        }

        /// <summary>
        /// Kiểm tra so sánh password đúng hay sai so với password cũ
        /// </summary>
        /// <param name="hashedPassword"></param>
        /// <param name="clearPassword"></param>
        /// <returns></returns>
        public static bool VerifyPassword(string hashedPassword, string clearPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword))
                return false;
            if (string.IsNullOrEmpty(clearPassword))
                clearPassword = string.Empty;
            //split salt and pass (MD5 has 32 character only)
            string hashed = hashedPassword.Substring(hashedPassword.Length - 32);
            byte[] salt = Convert.FromBase64String(hashedPassword.Substring(0, hashedPassword.Length - 32));
            //hash clear password with salt
            string hashedClearPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
               password: clearPassword,
               salt: salt,
               prf: KeyDerivationPrf.HMACSHA1,
               iterationCount: 10000,
               numBytesRequested: 256 / 8));
            //MD5 hashedClearPassword
            hashedClearPassword = MD5Hash(hashedClearPassword);
            //compare hashed and hashedClearPassword
            if (hashed.Equals(hashedClearPassword, StringComparison.Ordinal))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Mã hóa MD5 password đã được hash, thêm 1 lớp bảo vệ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
