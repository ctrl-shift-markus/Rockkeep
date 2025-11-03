using System.Security.Cryptography;
using System.Text.Json;

namespace RockkeepLibrary
{
    /// <summary>
    /// Provides methods for storing, retrieving, deleting, purging and generating passwords.
    /// </summary>
    public class PasswordHandler
    {
        /// <summary>
        /// The file path for storing passwords.
        /// </summary>
        public static readonly string rockkeepFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Rockkeep", "passwords.json");
        /// <summary>
        /// The directory path for Rockkeep's config.
        /// </summary>
        public static readonly string rockkeepDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Rockkeep");

        /// <summary>
        /// Encrypts a password using the provided master password, creates a dictionary with the new encrypted password and service as the key, and stores it in passwords.json or creates passwords.json if it doesn't exist.
        /// </summary>
        /// <param name="masterPassword">The master password to encrypt the password with.</param>
        /// <param name="password">A password to be encrypted and stored in passwords.json.</param>
        /// <param name="service">A service the password is used for.</param>
        /// <exception cref="ArgumentException">Raise an exception if any of the parameters are blank.</exception>
        /// <exception cref="IOException">Raise an exception if there was an I/O error.</exception>
        /// <exception cref="Exception">Raise a general exception if an error occured during storing the password.</exception>

        public static void StorePassword(string masterPassword, string password, string service)
        {

            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(masterPassword) || string.IsNullOrWhiteSpace(service))
                throw new ArgumentException("password cannot be null or whitespace.");

            try
            {
                ConfirmDirectoryExists();

                string encryptedPassword = Encryption.EncryptPassword(password, masterPassword);

                // Load passwords.json as a dictionary and add the new encrypted password with the service as the key
                Dictionary<string, string> loadedPasswords = LoadPasswords();
                loadedPasswords.Add(service, encryptedPassword);

                // Convert the dictionary back to JSON (with indented text) and write the new dictonary to passwords.json
                string jsonPasswords = JsonSerializer.Serialize(loadedPasswords);
                File.WriteAllText(rockkeepFilePath, jsonPasswords);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message.ToLower());
            }
            catch (IOException ex)
            {
                throw new IOException(ex.Message.ToLower());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToLower());
            }
        }

        /// <summary>
        /// Looks for a password stored with the provided service in passwords.json, decrypts it using the provided master password and returns it.
        /// </summary>
        /// <param name="masterPassword">Master password to decrypt retrieved password.</param>
        /// <param name="service">The service to retrieve the password from.</param>
        /// <returns>The password for the provided service.</returns>
        /// <exception cref="ArgumentException">Raise an exception if any of the parameters are blank.</exception>
        /// <exception cref="IOException">Raise an exception if there was an I/O error.</exception>
        /// <exception cref="KeyNotFoundException">Raise an exception if the key requested can't be found.</exception>
        /// <exception cref="Exception">Raise a general exception if an error occured during retrieving the password.</exception>

        public static string RetrievePassword(string masterPassword, string service)
        {
            if (string.IsNullOrWhiteSpace(masterPassword) || string.IsNullOrWhiteSpace(service))
                throw new ArgumentException("master password cannot be null or whitespace.");

            try
            {
                // Load the encrypted passwords from passwords.json as a dictionary
                var passwords = LoadPasswords();

                // Throw an exception if the service doesn't exist in the dictionary
                if (!passwords.ContainsKey(service))
                    throw new KeyNotFoundException($"service '{service}' not found in stored passwords.");

                // Decrypt the password and return it
                string encryptedPassword = passwords[service];
                string decryptedPassword = Encryption.DecryptPassword(encryptedPassword, masterPassword);

                return decryptedPassword;
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message.ToLower());
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message.ToLower());
            }
            catch (IOException ex)
            {
                throw new IOException(ex.Message.ToLower());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToLower());
            }
        }

        /// <summary>
        /// Looks for a password stored with the provided service in passwords.json and deletes it.
        /// </summary>
        /// <param name="service">The service to delete the password of.</param>
        /// <exception cref="ArgumentException">Raise an exception if any of the parameters are null or just spaces.</exception>
        /// <exception cref="IOException">Raise an exception if there was an I/O error.</exception>
        /// <exception cref="KeyNotFoundException">Raise an exception if the key requested can't be found.</exception>
        /// <exception cref="Exception">Raise a general exception if an error occured during deleting the password.</exception>

        public static void DeletePassword(string service)
        {
            if (string.IsNullOrEmpty(service) || string.IsNullOrWhiteSpace(service))
                throw new ArgumentException("service cannot be null or whitespace");

            try
            {
                // Load the encrypted passwords from passwords.json as a dictionary
                var passwords = LoadPasswords();

                // Throw an exception if the service doesn't exist in the dictionary
                if (!passwords.ContainsKey(service))
                    throw new KeyNotFoundException($"service '{service}' not found in stored passwords.");

                // Remove the password from the dictionary
                passwords.Remove(service);

                // Convert the dictionary back to JSON with indented text and write the new dictonary to passwords.json
                string json = JsonSerializer.Serialize(passwords, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(rockkeepFilePath, json);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message.ToLower());
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message.ToLower());
            }
            catch (IOException ex)
            {
                throw new IOException(ex.Message.ToLower());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToLower());
            }
        }

        /// <summary>
        ///Confirm Rockkeep's config directory exists, and create it if it doesn't.
        /// </summary>
        /// <exception cref="IOException">Raise an exception if there was an I/O error.</exception>
        /// <exception cref="Exception">Raise a general exception if an error occured during confirming the directory exists.</exception>
        private static void ConfirmDirectoryExists()
        {
            try
            {
                // Make the Rockkeep folder in AppData (Windows) or ~/.config (Linux) if it doesn't exist
                if (rockkeepDirectoryPath != null && !Directory.Exists(rockkeepDirectoryPath))
                {
                    Directory.CreateDirectory(rockkeepDirectoryPath);
                }
            }
            catch (IOException ex)
            {
                throw new IOException(ex.Message.ToLower());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToLower());
            }
        }

        /// <summary>
        /// Delete Rockkeep's config directory.
        /// </summary>
        /// <exception cref="IOException">Raise an exception if there was an I/O error.</exception>
        /// <exception cref="Exception">Raise a general exception if an error occured during purging the passwords.</exception>
        public static void PurgePasswords()
        {

            // Delete Rockkeep's config directory
            try
            {
                Directory.Delete(rockkeepDirectoryPath, true);
            }
            catch (IOException ex)
            {
                throw new IOException(ex.Message.ToLower().ToLower());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToLower().ToLower());
            }
        }

        /// <summary>
        /// Loads the passwords from passwords.json as a dictionary. If passwords.json doesn't exist or is empty, return an empty dictionary.
        /// </summary>
        /// <returns>A dictionary containing all the user's encrypted passwords.</returns>
        /// <exception cref="IOException">Raise an exception if there was an I/O error.</exception>
        /// <exception cref="Exception">Raise a general exception if an error occured during loading the passwords.</exception>
        private static Dictionary<string, string> LoadPasswords()
        {
            try
            {
                // Return an empty dictionary if passwords.json doesn't exist
                if (!File.Exists(rockkeepFilePath))
                    return new Dictionary<string, string>();

                // Read passwords.json
                string jsonFile = File.ReadAllText(rockkeepFilePath);

                // If passwords.json is empty, return an empty dictionary
                if (string.IsNullOrWhiteSpace(jsonFile))
                    return new Dictionary<string, string>();

                // Convert the JSON to a dictionary and return it. If jsonFile is null, empty or invalid, return an empty dictionary
                return JsonSerializer.Deserialize<Dictionary<string, string>>(jsonFile) ?? new Dictionary<string, string>();
            }
            catch (IOException ex)
            {
                throw new IOException(ex.Message.ToLower());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToLower());
            }
        }
        /// <summary>
        /// Generates a password of the requested length and includes special characters if requested.
        /// </summary>
        /// <param name="length">The length of the password.</param>
        /// <param name="includeSpecialChars">Whether or not to include special characters (e.g. !@#?).</param>
        /// <returns>A randomly generated, shuffled, secure password.</returns>
        public static string GeneratePassword(int length, bool includeSpecialChars)
        {
            // Create a character set for each character type
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string special = "!@#$%^&*()-_=+[]{}|;:,.<>?";

            // Create a full character and include special characters if requested
            string charset = lowercase + uppercase + digits;
            if (includeSpecialChars)
                charset += special;

            // Create a charater array to hold the randomly picked password characters
            var password = new char[length];

            // Retrieve one character from each character set so the password contains a mix of character types
            password[0] = lowercase[RandomNumberGenerator.GetInt32(lowercase.Length)];
            password[1] = uppercase[RandomNumberGenerator.GetInt32(uppercase.Length)];
            password[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];

            // If special characters are included, retrieve one from that set as well
            int startIndex = 3;
            if (includeSpecialChars)
            {
                password[3] = special[RandomNumberGenerator.GetInt32(special.Length)];
                startIndex = 4;
            }

            // Fill the rest of the password with random characters from the full character set up until the requested length is reached
            for (int i = startIndex; i < length; i++)
            {
                password[i] = charset[RandomNumberGenerator.GetInt32(charset.Length)];
            }

            // Shuffle characters using Fisher-Yates algorithm as a cherry on top to ensure true randomness
            for (int i = length - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (password[i], password[j]) = (password[j], password[i]);
            }

            return new string(password);
        }
    }
}