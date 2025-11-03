using System;
using static RockkeepApp.CLIHelper;
using RockkeepLibrary;

namespace RockkeepApp;

public class CLIFunctions
{
    // Show help message
    public static void Help()
    {
        Version();
        Console.WriteLine("Usage: netpass [argument] [parameter] [parameter]\n");
        Console.WriteLine("Arguments:");
        Console.WriteLine("-h: Show this help message.");
        Console.WriteLine("-v: Show the current version of Rockkeep.");
        Console.WriteLine("-s [service:string]: Store a password for the service.");
        Console.WriteLine("-d [service:string]: Delete the password for the service.");
        Console.WriteLine("-g [service:string]: Get the password for the service.");
        Console.WriteLine("-p: Purge all stored passwords.");
        Console.WriteLine("-e [length:int] [includeSpecialChars:bool]: Generate a random password with the length and whether or not to include special characters (true/false). The length must be between 8 (too small) and 32 (too massive).\n");
        Console.WriteLine("All passwords are randomly encrypted, hashed and salted, stay on your computer and are in your control for what you want to do with them. Rockkeep is open-source, meaning you can review the code at any time. Passwords are saved in your local config folder/Rockkeep/passwords.json.");
        Console.WriteLine("GitHub repository: https://github.com/ctrl-shift-markus/Rockkeep.");
    }

    // Show current version
    public static void Version()
    {
        Console.WriteLine("Rockkeep version v1.0.0");
    }

    // Show invalid argument message
    public static void InvalidArgument(string argument)
    {
        WriteError($"'{argument}' is not a valid argument. Use '-h' to show you the list of available arguments.");
    }

    // Read and store password for a service
    public static void StorePassword(string service)
    {
        try
        {
            if (string.IsNullOrEmpty(service) || string.IsNullOrWhiteSpace(service))
            {
                throw new ArgumentException("service cannot be empty.");
            }
            // Retrieve password and master password
            string password = ReadPasswordWithValidation("Enter password: ", "Re-enter password: ");
            string masterPassword = ReadPasswordWithValidation("Enter master password: ", "Re-enter master password: ");

            // Store the password
            PasswordHandler.StorePassword(masterPassword, password, service);
            WriteSuccess($"Password for service '{service}' stored successfully!");
        }
        catch (Exception ex)
        {
            WriteError($"Error. Cause: {ex.Message.ToLower()}");
        }
    }

    // Delete password for a service
    public static void DeletePassword(string service)
    {
        try
        {
            if (string.IsNullOrEmpty(service) || string.IsNullOrWhiteSpace(service))
            {
                throw new ArgumentException("service cannot be empty.");
            }
            PasswordHandler.DeletePassword(service);
            WriteSuccess($"Password for service '{service}' deleted successfully!");
        }
        catch (Exception ex)
        {
            WriteError($"Error. Cause: {ex.Message.ToLower()}");
        }
    }

    // Retrieve password for a service
    public static void RetrievePassword(string service)
    {
        try
        {
            string masterPassword = ReadPassword("Enter master password: ");
            if (string.IsNullOrEmpty(masterPassword) || string.IsNullOrWhiteSpace(masterPassword))
            {
                throw new ArgumentException("master password cannot be empty.");
            }
            string password = PasswordHandler.RetrievePassword(masterPassword, service);
            WriteSuccess($"Password: {password}");
        }
        catch (Exception ex)
        {
            WriteError($"Error. Cause: {ex.Message.ToLower()}");
        }
    }

    // Purge all stored passwords
    public static void PurgePasswords()
    {
        try
        {
            PasswordHandler.PurgePasswords();
            WriteSuccess("All stored passwords have been purged successfully!");
        }
        catch (Exception ex)
        {
            WriteError($"Error. Cause: {ex.Message.ToLower()}");
        }
    }

    // Generate a random password
    public static void GeneratePassword(int length, bool specialCharacter)
    {
        try
        {
            string password = PasswordHandler.GeneratePassword(length, specialCharacter);
            WriteSuccess($"Generated password: {password}");
        }
        catch (Exception ex)
        {
            WriteError($"Error. Cause: {ex.Message.ToLower()}");
        }
    }
}