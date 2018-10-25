namespace IRunesWebApp.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    using IRunesWebApp.Contracts;

    public class HashService : IHashService
    {
        public string Hash(string stringToHash)
        {
            stringToHash += "myAppSalt2546595614845156459#";

            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));

                var hash = BitConverter.ToString(hashedBytes).Replace("-", string.Empty).ToLower();

                return hash;
            }
        }
    }
}
