using System.Security.Cryptography;
using System.Text;

namespace RockkeepLibrary
{
    /// <summary>
    /// Provides methods for encrypting and decrypting passwords using a master password.
    /// </summary>
    public class Encryption
    {
        // Prepare all the variables needed for encryption/decryption
        private const int KeySize = 32;
        private const int NonceSize = 12;
        private const int TagSize = 16;
        private const int SaltSize = 16;
        private const int Iterations = 600000;

        /// <summary>
        /// Encrypts a password using the provided master password.
        /// </summary>
        /// <param name="decryptedPassword">A decrypted password to be encrypted</param>
        /// <param name="masterPassword">A master password to encrypt the password with.</param>
        /// <returns>A securely encrypted password in base 64.</returns>
        /// <exception cref="CryptographicException">Raise an exception if a cryptographic-related error occured during encryption.</exception>
        /// <exception cref="Exception">Raise a general exception if an error occured during encryption.</exception>"
        public static string EncryptPassword(string decryptedPassword, string masterPassword)
        {
            try
            {
                // Variables for encryption
                byte[] salt = RandomNumberGenerator.GetBytes(SaltSize); // Generate a new random salt
                byte[] key = DeriveKey(masterPassword, salt); // Derive an encryption key from the master password and salt
                byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize); // Generate a new random nonce
                byte[] plaintext = Encoding.UTF8.GetBytes(decryptedPassword); // Convert the decrypted password to bytes
                byte[] ciphertext = new byte[plaintext.Length]; // Prepare a byte array to hold the ciphertext
                byte[] tag = new byte[TagSize]; // Prepare a byte array to hold the authentication tag

                // Encrypt the plaintext using AES-GCM
                using (var aesGcm = new AesGcm(key, TagSize))
                {
                    aesGcm.Encrypt(nonce, plaintext, ciphertext, tag);
                }

                // Zero out unneeded sensitive data in memory
                CryptographicOperations.ZeroMemory(key);
                CryptographicOperations.ZeroMemory(plaintext);

                // Combine the salt, nonce, ciphertext, and tag all into a single byte array
                byte[] result = new byte[SaltSize + NonceSize + ciphertext.Length + TagSize];
                Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
                Buffer.BlockCopy(nonce, 0, result, SaltSize, NonceSize);
                Buffer.BlockCopy(ciphertext, 0, result, SaltSize + NonceSize, ciphertext.Length);
                Buffer.BlockCopy(tag, 0, result, SaltSize + NonceSize + ciphertext.Length, TagSize);

                // Zero out unneeded sensitive data in memory
                CryptographicOperations.ZeroMemory(salt);
                CryptographicOperations.ZeroMemory(nonce);
                CryptographicOperations.ZeroMemory(ciphertext);
                CryptographicOperations.ZeroMemory(tag);

                // Return the encrypted password as a base 64 string so that it can easily be stored
                return Convert.ToBase64String(result);
            }
            catch (CryptographicException ex)
            {
                throw new CryptographicException(ex.Message.ToLower());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToLower());
            }
        }

        /// <summary>
        /// Decrypts an encrypted password using the provided master password.
        /// </summary>
        /// <param name="encryptedPassword">An encrypted password to be decrypted.</param>
        /// <param name="masterPassword">Master password to decrypt the password with.</param>
        /// <returns>The decrypted password.</returns>
        /// <exception cref="CryptographicException">Raise a general exception if an error occured during decryption.</exception>
        public static string DecryptPassword(string encryptedPassword, string masterPassword)
        {
            // Decode the base 64 string to get the byte array
            byte[] data = Convert.FromBase64String(encryptedPassword);

            // Raise an exception if the data isn't in the correct format
            if (data.Length < SaltSize + NonceSize + TagSize)
                throw new CryptographicException("invalid encrypted data format.");

            // Extract the salt, nonce, ciphertext, and tag out the byte array
            byte[] salt = new byte[SaltSize];
            byte[] nonce = new byte[NonceSize];
            byte[] ciphertext = new byte[data.Length - SaltSize - NonceSize - TagSize];
            byte[] tag = new byte[TagSize];

            // Unpack the data
            Buffer.BlockCopy(data, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(data, SaltSize, nonce, 0, NonceSize);
            Buffer.BlockCopy(data, SaltSize + NonceSize, ciphertext, 0, ciphertext.Length);
            Buffer.BlockCopy(data, SaltSize + NonceSize + ciphertext.Length, tag, 0, TagSize);

            // Derive the key from the master password and salt
            byte[] key = DeriveKey(masterPassword, salt);
            byte[] plaintext = new byte[ciphertext.Length];

            try
            {
                // Decrypt the ciphertext using AES-GCM
                using (var aesGcm = new AesGcm(key, TagSize))
                {
                    aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
                }

                // Return the decrypted password as a regular string
                return Encoding.UTF8.GetString(plaintext);
            }
            catch (CryptographicException)
            {
                throw new CryptographicException("invalid master password.");
            }
            finally
            {
                // Zero out unneeded sensitive data in memory
                CryptographicOperations.ZeroMemory(key);
                CryptographicOperations.ZeroMemory(plaintext);
                CryptographicOperations.ZeroMemory(salt);
                CryptographicOperations.ZeroMemory(nonce);
                CryptographicOperations.ZeroMemory(ciphertext);
                CryptographicOperations.ZeroMemory(tag);
            }
        }

        /// <summary>
        /// Derives an encryption key using the master password and salt using PBKDF2.
        /// </summary>
        /// <param name="masterPassword">A master password to derive the encryption key from.</param>
        /// <param name="salt">A salt to make sure the same encryption key is never generated twice.</param>
        /// <returns>An encryption key to encrypt passwords with.</returns>
        private static byte[] DeriveKey(string masterPassword, byte[] salt)
        {
            // Use PBKDF2 alongside SHA-256 to derive a secure encryption key
            using (var pbkdf2 = new Rfc2898DeriveBytes(masterPassword, salt, Iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(KeySize);
            }
        }
    }
}