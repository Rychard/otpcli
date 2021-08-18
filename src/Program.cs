using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using OtpNet;

namespace otpcli
{
    class Program
    {
        static Int32 Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    options => RunOptions(options), 
                    errors => HandleParseError(errors)
                );
        }

        static String ResolveFile(String filename, Boolean verbose = false)
        {
            if(String.IsNullOrWhiteSpace(filename))
            {
                filename = ".otpcli";
            }

            if(Path.IsPathFullyQualified(filename) && File.Exists(filename))
            {
                if (verbose) { Console.WriteLine($"Using fully qualified path: {filename}"); }
                return filename;
            }

            String current;

            // Search for the file in the current working directory
            String currentWorkingDirectory = Directory.GetCurrentDirectory();
            current = Path.Combine(currentWorkingDirectory, filename);
            if (verbose) { Console.WriteLine($"Checking in current working directory: {current}"); }
            if (File.Exists(current)) { return current; }
            if (verbose) { Console.WriteLine($"File does not exist: {current}"); }

            // Search for the file in the user's home directory
            String homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            current = Path.Combine(homePath, filename);
            if (verbose) { Console.WriteLine($"Checking in user's home directory: {current}"); }
            if (File.Exists(current)) { return current; }
            if (verbose) { Console.WriteLine($"File does not exist: {current}"); }

            // If the file is not found, return nothing.
            if (verbose) { Console.WriteLine($"Configuration file not found!"); }
            return String.Empty;
        }

        static Byte[] DecodeSecretKey(String input, bool verbose)
        {
            Byte[] decodedBytes = null;
            try
            {
                var base32Bytes = Base32Encoding.ToBytes(input);
                decodedBytes = base32Bytes;
            }
            catch (Exception) 
            {
                if (verbose) { Console.WriteLine($"Configuration file does not contain a valid Base32 value."); }
            }

            try
            {
                var base64Bytes = Convert.FromBase64String(input);
                decodedBytes = base64Bytes;
            }
            catch (Exception) 
            {
                if (verbose) { Console.WriteLine($"Configuration file does not contain a valid Base64 value."); }
            }

            if(decodedBytes == null)
            {
                throw new InvalidDataException("The specified value is not an encoded Base32/Base64 string!");
            }

            return decodedBytes;
        }

        static Int32 RunOptions(Options options)
        {
            String configurationFile = ResolveFile(options.ConfigurationFile, verbose: options.Verbose);

            String encodedSecret;
            try
            {
                encodedSecret = File.ReadAllText(configurationFile).Trim();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unable to read data from configuration file: {configurationFile}");
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }

            Byte[] decodedSecret;
            try
            {
                decodedSecret = DecodeSecretKey(encodedSecret, verbose: options.Verbose);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while decoding the secret key in the configuration file: {configurationFile}");
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }

            Totp totp;
            try
            {
                totp = new Totp(decodedSecret);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while initializing the OTP library.");
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }

            try
            {
                String token = totp.ComputeTotp();
                if (options.Wait) 
                {
                    if (options.Verbose)
                    {
                        Console.WriteLine($"Current token: {token}");
                    }

                    // If the user wants to ensure a fresh token is returned, we must wait for the next code.
                    Int32 remainingSeconds = totp.RemainingSeconds();
                    token = totp.ComputeTotp(DateTime.Now.AddSeconds(remainingSeconds));

                    if (options.Verbose)
                    {
                        Console.WriteLine($"The next token will become valid in {remainingSeconds} seconds");
                        Console.WriteLine($"Next token: {token}");
                    }
                    System.Threading.Thread.Sleep(remainingSeconds * 1000);
                }
                Console.WriteLine(token);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while generating the TOTP code.");
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }

            return 0;
        }

        static Int32 HandleParseError(IEnumerable<Error> errs)
        {
            errs.Output();
            return 1;
        }
    }
}
