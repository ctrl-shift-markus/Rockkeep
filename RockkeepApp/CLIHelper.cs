using System;
using System.Text;
using System.Security.Cryptography;

namespace RockkeepApp;

/// <summary>
/// Helpers for CLIFunctions so that Rockkeep looks more clean from a CLI perspective.
/// </summary>
internal class CLIHelper
{
    /// <summary>
    /// Change the console text color to green, print the message and reset the color back to normal.
    /// </summary>
    /// <param name="message">The message to be printed.</param>
    internal static void WriteSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Writes a warning message to the console in yellow text.
    /// </summary>
    /// <param name="message">The warning message to display. If null or empty, no output is written.</param>
    internal static void WriteWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{message}");
        Console.ResetColor();
    }

    internal static void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{message}");
        Console.ResetColor();
    }

    /// <summary>
    /// A safer Console.ReadLine() for passwords that masks the input with asterisks.
    /// </summary>
    /// <param name="passwordPrompt">The prompt displayed when entering your password.</param>
    /// <returns>The password inputted.</returns>
    /// <exception cref="ArgumentException">Raise an exception if any of the parameters are blank.</exception>
    public static string ReadPassword(string prompt = "Enter password: ")
    {
        if (string.IsNullOrEmpty(prompt) || string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("prompt cannot be empty");
        }

        // Prompt the user for their password
        Console.Write(prompt);

        StringBuilder password = new StringBuilder();
        ConsoleKeyInfo key;

        // Reads each key inputted by the user and checks if the password is null, empty or whitespace when Enter is pressed. If it isn't, the password is returned just like Console.ReadLine().
        while (true)
        {
            key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password.Length--;
                    Console.Write("\b \b");
                }
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine();
                return string.Empty;
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password.Append(key.KeyChar);
                Console.Write('*');
            }
        }

        if (String.IsNullOrEmpty(password.ToString()) || String.IsNullOrWhiteSpace(password.ToString()))
        {
            throw new ArgumentException("password cannot be empty");
        }
        return password.ToString();
    }

    /// <summary>
    /// ReadPassword() but with validation.
    /// </summary>
    /// <param name="prompt">The prompt displayed when entering your password.</param>
    /// <param name="reprompt">The prompt displayed when re-entering your password.</param>
    /// <returns>The password inputted.</returns>
    /// <exception cref="ArgumentException">Raise an exception if any of the parameters are blank.</exception>
    public static string ReadPasswordWithValidation(string prompt = "Enter password: ", string reprompt = "Re-enter password: ")
    {

        if (string.IsNullOrEmpty(prompt) || string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("prompt cannot be empty");
        }

        // Loop until the passwords match
        while (true)
        {
            string password;
            string confirmation;

            try
            {
                // Read the password and confirmation. If they match, return the password.
                password = ReadPassword(prompt);
                confirmation = ReadPassword(reprompt);
                if (password == confirmation)
                {
                    return password;
                }
            }
            catch (ArgumentException)
            {
                WriteWarning("Password cannot be empty. Pick another password.");
                continue;
            }
            WriteError("Passwords do not match. Please try again.");
        }
    }
}