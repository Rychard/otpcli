using System;
using System.IO;
using OtpNet;

namespace otpcli
{
    class Program
    {
        private static String FileName = ".otpcli";

        static void Main(string[] args)
        {
            String homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            String filePath = Path.Combine(homePath, FileName);

            String base64Secret = String.Empty;
            if(File.Exists(filePath))
            {
                base64Secret = File.ReadAllText(filePath);
            }

            Byte[] secretKey = Base64Decode(base64Secret);
            var totp = new Totp(secretKey);
            Console.WriteLine(totp.ComputeTotp());
        }

        public static Byte[] Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return base64EncodedBytes;
        }
    }
}
