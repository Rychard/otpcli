using System;
using System.IO;
using OtpNet;

namespace otpcli
{
    class Program
    {
        private static String FileName = ".otpcli";

        static Int32 Main(string[] args)
        {
            String homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            String filePath = Path.Combine(homePath, FileName);

            if(!File.Exists(filePath))
            {
                Console.Error.WriteLine($"Configuration file does not exist: {filePath}");
                return 1;
            }

            String base64Secret;
            try
            {
                base64Secret = File.ReadAllText(filePath).Trim();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unable to read data from configuration file: {filePath}");
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }

            if (String.IsNullOrWhiteSpace(base64Secret))
            {
                Console.Error.WriteLine($"Configuration file does not contain an encoded secret: {filePath}");
                return 1;
            }

            Byte[] secretKey;
            try
            {
                 secretKey = Convert.FromBase64String(base64Secret);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while decoding the secret key in the configuration file: {filePath}");
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }

            Totp totp;
            try
            {
                totp = new Totp(secretKey);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while initializing the OTP library.");
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }

            try
            {
                String result = totp.ComputeTotp();
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while generating the TOTP code.");
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }

            return 0;
        }
    }
}
