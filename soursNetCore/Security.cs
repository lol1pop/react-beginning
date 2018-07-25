using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DeskAlerts
{
    public static class Security  
    {
        public static string ComputeHash(string login, string password) {
            var algorithm = new SHA256CryptoServiceProvider();
            var reversLogin = new string(login.ToCharArray().Reverse().ToArray());
            var pwd = password + reversLogin;
            var inputBytes = Encoding.Unicode.GetBytes(password + reversLogin);
            var hashedBytes = algorithm.ComputeHash(inputBytes);
            return BitConverter.ToString(hashedBytes).ToLower().Replace("-", "");
        }
    }
}