using System;
using static RockkeepApp.CLIFunctions;
using static RockkeepApp.CLIHelper;

namespace RockkeepApp;

internal class Program
{
    private static void Main(string[] args)
    {
        // Show the user how to use Rockkeep if no arguments are provided
        if (args.Length == 0)
        {
            WriteWarning("Invalid argument. Run Rockkeep with '-h' for help");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return;
        }
        // All supported arguments
        switch (args[0])
        {
            // Help message
            case "-h":
                Help();
                break;
            // Version message
            case "-v":
                Version();
                break;
            // Store password
            case "-s":
                if (args.Length == 2)
                {
                    StorePassword(args[1]);
                }
                else
                {
                    WriteError("Invalid argument. Usage: -s [service:string]");
                }
                break;
            // Delete password
            case "-d":
                if (args.Length == 2)
                {
                    DeletePassword(args[1]);
                }
                else
                {
                    WriteError("Invalid argument. Usage: -d [service:string]");
                }
                break;
            // Retrieve password
            case "-r":
                if (args.Length == 2)
                {
                    RetrievePassword(args[1]);
                }
                else
                {
                    WriteError("Invalid argument. Usage: -g [service:string]");
                }
                break;
            // Purge all passwords
            case "-p":
                if (args.Length == 1)
                {
                    PurgePasswords();
                }
                else
                {
                    WriteError("Invalid argument. Usage: -p");
                }
                break;
            // Generate password
            case "-g":
                // There must be 3 arguments: -e, length and includeSpecialChars, length must be an integer and includeSpecialChars must be a boolean
                if (args.Length == 3 && int.TryParse(args[1], out int length) && bool.TryParse(args[2], out bool characters))
                {
                    // Length must be between 8 and 32
                    if (length <= 8 || length >= 32)
                    {
                        WriteError("Error. Cause: password length must not be below or equal to 8 or above and equal to 32");
                        break;
                    }
                    GeneratePassword(length, characters);
                }
                else
                {
                    WriteError("Invalid arguments. Usage: -e [length:int] [includeSpecialChars:bool]");
                }
                break;
            // If none of the supported argumnets are inputted
            default:
                InvalidArgument(args[0]);
                break;
        }
    }
}