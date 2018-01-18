/*
 * (Licence Notice (LGPLv3))

This file is part of ArkUtility Infrastructure.

ArkUtility Infrastructure is free software: you can redistribute it and / or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ArkUtility Infrastructure is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with ArkUtility Infrastructure.If not, see < http://www.gnu.org/licenses/ >.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ArkUtility.Infrastructure.Extensions;

namespace ArkUtility.Infrastructure
{
    /// <summary>
    /// Class to enable and support encryption Symmetric, Asymmetric, and Hashing.
    /// </summary>
    public class Encryption
    {

        private static readonly string _salt = "SNJRDNRLS^43uygé&%¿bsakeô24ffs";

        #region Symmetric Encryption
        /// <summary>
        /// Returns a base64 encoded, algorithim encrypted string
        /// </summary>
        /// <param name="plainText">Source Text</param>
        /// <param name="algorithm"></param>
        /// <param name="sharedPhrase">Shared Phrase used to generate the encryption Key by combining with salt</param>
        /// <param name="salt">Salt to use for hashing sharePhrase and generate the key. If null or empty defaults to a local constant value.</param>
        /// <param name="encoding">Optional: Default: Encoding.Unicode</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Encrypt(SupportedSymmetricEncryptionAlgorithim algorithm,
            string plainText,  string sharedPhrase,  string salt = null, Encoding encoding = null)
        {
            var key = new byte[0];
            encoding = encoding ?? Encoding.Unicode;
            switch (algorithm)
            {
                case SupportedSymmetricEncryptionAlgorithim.Aes256:
                    var saltedText = GetSaltedText(sharedPhrase, salt);
                    key = GetSha256Hash(saltedText, encoding);
                    break;
                default:
                    throw new NotImplementedException($"Algorithm not implemented in Encrypt: {algorithm}");
            }
            var cipherText = Encrypt(algorithm, plainText, key, encoding);
            return cipherText;
        }
        /// <summary>
        /// Returns a base64 encoded, algorithim encrypted string
        /// </summary>
        /// <param name="plainText">Source Text</param>
        /// <param name="algorithm"></param>
        /// <param name="key">Valid key Length byte array</param>
        /// <param name="encoding">Optional: Default: Encoding.Unicode</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Encrypt(SupportedSymmetricEncryptionAlgorithim algorithm, string plainText,
             byte[] key, Encoding encoding = null)
        {
            var bytes = (encoding ?? Encoding.Unicode).GetBytes(plainText);
            bytes = Encrypt(algorithm, bytes, key);
            var cipherText = Convert.ToBase64String(bytes);
            return cipherText;
        }
        /// <summary>
        /// Returns a base64 encoded, algorithim encrypted string
        /// </summary>
        /// <param name="plainData"></param>
        /// <param name="algorithm"></param>
        /// <param name="key">Valid key Length byte array</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] Encrypt(SupportedSymmetricEncryptionAlgorithim algorithm,
            byte[] plainData, byte[] key)
        {
            var bytes = new byte[0];
            switch (algorithm)
            {
                case SupportedSymmetricEncryptionAlgorithim.Aes256:
                    bytes = AesEncryptBytes(plainData, key);
                    break;
                default:
                    throw new NotImplementedException($"Algorithm not implemented in Encrypt: {algorithm}");
            }
            return bytes;
        }

        /// <summary>
        /// Encrypts a string using AES 256 with a random 128bit initialization vector which is prepended.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key">Valid key Length. ex: 128bit, 192bit, or 256bit value</param>
        /// <param name="encoding">Optional: Default: Encoding.Unicode</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        protected static byte[] Aes256EncryptStringToBytes(string plainText, byte[] key, Encoding encoding = null)
        {
            //Safety Check.
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length == 0)
                throw new ArgumentNullException(nameof(key));
            encoding = encoding ?? Encoding.Unicode;
            byte[] plainTextBytes = encoding.GetBytes(plainText);
            var encryptedBytes = AesEncryptBytes(plainTextBytes, key);
            return encryptedBytes;
        }

        /// <summary>
        /// Encrypts a byte[] using AES 256 with a random 128bit initialization vector which is prepended to the Array.
        /// </summary>
        /// <param name="bytesToEncrypt"></param>
        /// <param name="key">Valid key Length. ex: 128bit, 192bit, or 256bit value</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        protected static byte[] AesEncryptBytes(byte[] bytesToEncrypt, byte[] key)
        {
            //Safety Check.
            if (bytesToEncrypt == null || bytesToEncrypt.Length == 0)
                throw new ArgumentNullException(nameof(bytesToEncrypt));
            if (key == null || key.Length == 0)
                throw new ArgumentNullException(nameof(key));
            byte[] encrypted;
            using (var aes = new AesCryptoServiceProvider())
            {
                //Configure the AES settings
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = key.Length * 8;
                aes.Key = key;
                aes.GenerateIV();
                using (MemoryStream ms = new MemoryStream())
                {
                    //Write the initialization vector to the start of the stream
                    ms.Write(aes.IV, 0, 16);
                    using (var encryptor = aes.CreateEncryptor())
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
                    }
                    encrypted = ms.ToArray();
                }
            }
            return encrypted;
        }

        #endregion Symmetric Encryption

        #region Symmetric Decryption
        /// <summary>
        /// Returns decrypted and decoded string. Generates the key using the salted sharedPhrase. Initial Bytes of ciphertext provide the initialization vector.
        /// </summary>
        /// <param name="cipherText">Encrypted text</param>
        /// <param name="algorithm"></param>
        /// <param name="sharedPhrase">Shareed Phrase used to generate the encryption Key</param>
        /// <param name="salt">Salt to use for hashing sharePhrase and generate the key. If null or empty defaults to a local constant value.</param>
        /// <param name="encoding">Optional: Default: Encoding.Unicode</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Decrypt(SupportedSymmetricEncryptionAlgorithim algorithm, string cipherText, string sharedPhrase,
            string salt = null, Encoding encoding = null)
        {
            byte[] key = new byte[0];
            encoding = encoding ?? Encoding.Unicode;
            //Build Key of proper size based on the algorithm
            switch (algorithm)
            {
                case SupportedSymmetricEncryptionAlgorithim.Aes256:
                    var saltedText = GetSaltedText(sharedPhrase, salt);
                    key = GetSha256Hash(saltedText, encoding);
                    break;
                default:
                    throw new NotImplementedException($"Algorithm not implemented in Decrypt: {algorithm}");
            }
            var plainText = Decrypt(algorithm, cipherText, key, encoding);
            return plainText;
        }

        /// <summary>
        /// Returns decrypted and decoded string. Initial Bytes of ciphertext provide the initialization vector.
        /// </summary>
        /// <param name="cipherText">Encrypted text</param>
        /// <param name="algorithm"></param>
        /// <param name="key">Valid key Length. ex: 128bit, 192bit, or 256bit value</param>
        /// <param name="encoding">Optional: Default: Encoding.Unicode</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Decrypt(SupportedSymmetricEncryptionAlgorithim algorithm, string cipherText, byte[] key, Encoding encoding = null)
        {
            string plainText = null;
            byte[] cipherData;
            switch (algorithm)
            {
                case SupportedSymmetricEncryptionAlgorithim.Aes256:
                    cipherData = Convert.FromBase64String(cipherText);
                    break;
                default:
                    throw new NotImplementedException($"Algorithm not implemented in Decrypt: {algorithm}");
            }
            var plainTextBytes = Decrypt(algorithm, cipherData, key);
            plainText = (encoding ?? Encoding.Unicode).GetString(plainTextBytes);
            return plainText;
        }
        /// <summary>
        /// Returns decrypted byte array. 
        /// </summary>
        /// <param name="cipherData">Encrypted bytes</param>
        /// <param name="algorithm"></param>
        /// <param name="key">Valid key Length. ex: 128bit, 192bit, or 256bit value</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] Decrypt(SupportedSymmetricEncryptionAlgorithim algorithm, byte[] cipherData, byte[] key)
        {
            byte[] plainData = null;
            switch (algorithm)
            {
                case SupportedSymmetricEncryptionAlgorithim.Aes256:
                    plainData = AesDecryptBytes(cipherData, key);
                    break;
                default:
                    throw new NotImplementedException($"Algorithm not implemented in GetHashString: {algorithm}");
            }
            return plainData;
        }

        /// <summary>
        /// Decrypt an AES encrypted byte array.
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key">Valid key Length. ex: 128bit, 192bit, or 256bit value</param>
        /// <param name="encoding">Optional: Default: Encoding.Unicode</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        protected static string AesDecryptStringFromBytes(byte[] cipherText, byte[] key, Encoding encoding = null)
        {
            //Safety Check.
            if (cipherText == null || cipherText.Length == 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (cipherText.Length <= 16)
                throw new ArgumentException($"Argument [{nameof(cipherText)}] does not contain \"Initialization Vector\" information. Array length {cipherText?.Length ?? 0}");
            if (key == null || key.Length == 0)
                throw new ArgumentNullException(nameof(key));
            encoding = encoding ?? Encoding.Unicode;
            var plainTextBytes = AesDecryptBytes(cipherText, key);
            var plainText = encoding.GetString(plainTextBytes);
            return plainText;
        }
        /// <summary>
        /// Decrypt an AES encrypted byte array.
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        protected static byte[] AesDecryptBytes(byte[] cipherText, byte[] key)
        {
            //Safety Check.
            if (cipherText == null || cipherText.Length == 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (cipherText.Length <= 16)
                throw new ArgumentException($"Argument [{nameof(cipherText)}] does not contain \"Initialization Vector\" information. Array length {cipherText?.Length ?? 0}");
            if (key == null || key.Length == 0)
                throw new ArgumentNullException(nameof(key));
            byte[] plainTextBytes;
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = key.Length * 8;
                aes.Key = key;
                var vectorFromCipher = cipherText.Take(16).ToArray();
                aes.IV = vectorFromCipher;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (var decryptor = aes.CreateDecryptor())
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(cipherText, 16, cipherText.Length - 16);
                    }
                    plainTextBytes = ms.ToArray();
                }
            }
            return plainTextBytes;
        }

        #endregion Symmetric Decryption

        /// <summary>
        /// Returns a Base64 encoded Hash of the text.
        /// </summary>
        /// <param name="algoritm">Valid supported algorithm</param>
        /// <param name="text">Source Text</param>
        /// <param name="salt">Salt to use for hashing. If null or empty defaults to a local constant value.</param>
        /// <param name="encoding">Optional: Default: Encoding.Unicode</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetHashString(SupportedHashGenerationAlgorithm algoritm, string text, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));
            var enc = encoding ?? Encoding.Unicode;
            byte[] hashBytes = new byte[0];
            switch (algoritm)
            {
                case SupportedHashGenerationAlgorithm.Sha256:
                    hashBytes = GetSha256Hash(text, enc);
                    break;
                case SupportedHashGenerationAlgorithm.Sha512:
                    hashBytes = GetSha512Hash(text, enc);
                    break;
                default:
                    throw new NotImplementedException($"Algorithm not implemented in GetHashString: {algoritm}");
            }
            return Convert.ToBase64String(hashBytes);
        }
        /// <summary>
        /// Returns a Base64 encoded Hash of the byte array.
        /// </summary>
        /// <param name="algoritm">Valid supported algorithm</param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetHashString(SupportedHashGenerationAlgorithm algoritm, byte[] bytes)
        {
            byte[] hashBytes = new byte[0];
            switch (algoritm)
            {
                case SupportedHashGenerationAlgorithm.Sha256:
                    hashBytes = GetSha256Hash(bytes);
                    break;
                case SupportedHashGenerationAlgorithm.Sha512:
                    hashBytes = GetSha512Hash(bytes);
                    break;
                default:
                    throw new NotImplementedException($"Algorithm not implemented in GetHashString: {algoritm}");
            }
            return hashBytes.Base64Encode();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">Source Text</param>
        /// <param name="salt">Salt to use for hashing. If null or empty defaults to a local constant value.</param>
        /// <param name="encoding">Optional: Default: Encoding.Unicode</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] GetSha256Hash(string text, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));

            var bytes = (encoding ?? Encoding.Unicode).GetBytes(text);
            return GetSha256Hash(bytes);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] GetSha256Hash(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                throw new ArgumentNullException(nameof(bytes));
            byte[] result;
            using (var sha256 = new SHA256CryptoServiceProvider())
            {
                result = sha256.ComputeHash(bytes);
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">Source Text</param>
        /// <param name="salt">Salt to use for hashing. If null or empty defaults to a local constant value.</param>
        /// <param name="encoding">Optional: Default: Encoding.Unicode</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] GetSha512Hash(string text, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));
            var bytes = (encoding ?? Encoding.Unicode).GetBytes(text);
            return GetSha512Hash(bytes);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] GetSha512Hash(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                throw new ArgumentNullException(nameof(bytes));
            byte[] result;
            using (var sha512 = new SHA512CryptoServiceProvider())
            {
                result = sha512.ComputeHash(bytes);
            }
            return result;
        }
        /// <summary>
        /// Salts the text by appending the salt.
        /// </summary>
        /// <param name="text">Source Text</param>
        /// <param name="salt">Salt to use for hashing. If null or empty defaults to a local constant value.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetSaltedText(string text, string salt)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrEmpty(salt))
                salt = _salt;
            return $"{text}{salt}";
        }
    }

    /// <summary>
    /// Symmetric Encryption Algorithms which are supported by ArkUtility.Infrastructure.Encryption methods.
    /// </summary>
    public enum SupportedSymmetricEncryptionAlgorithim
    {
        None = 0,
        /// <summary>
        /// AES-256 algorithm. FIPS compliant.
        /// </summary>
        Aes256 = 2,
    }
    /// <summary>
    /// Hash Algorithms which are supported by ArkUtility.Infrastructure.Encryption methods.
    /// </summary>
    public enum SupportedHashGenerationAlgorithm
    {
        None = 0,
        /// <summary>
        /// SHA-256 algorithm. FIPS compliant.
        /// </summary>
        Sha256 = 3,
        /// <summary>
        /// SHA-512 algorithm.
        /// </summary>
        Sha512 = 5
    }
}
