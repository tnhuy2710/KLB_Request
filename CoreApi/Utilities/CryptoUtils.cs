using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using CoreApi.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
#pragma warning disable 649

// ReSharper disable InconsistentNaming

namespace CoreApi.Utilities
{
    public static class CryptoUtils
    {
        /// <summary>
        /// Convert Bit to Hex String.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] hex)
        {
            if (hex == null) return null;
            if (hex.Length == 0) return string.Empty;

            var s = new StringBuilder();
            foreach (byte b in hex)
            {
                s.Append(b.ToString("x2"));
            }
            return s.ToString();
        }

        public class MD5
        {
            /// <summary>
            /// Encrypt plain text as MD5 Hashed.
            /// </summary>
            /// <param name="plainText"></param>
            /// <returns></returns>
            public static string Encrypt(string plainText)
            {
                var alg = System.Security.Cryptography.MD5.Create();
                alg.ComputeHash(Encoding.UTF8.GetBytes(plainText.Trim()));
                return ToHexString(alg.Hash);
            }

            /// <summary>
            /// Encrypt plain text as MD5 Hashed with ASCII Key.
            /// </summary>
            /// <param name="plainText"></param>
            /// <param name="key">ASCII Key only</param>
            /// <returns></returns>
            public static string Encrypt(string plainText, string key)
            {
                var alg = HMAC.Create();
                alg.HashName = "md5";
                alg.Key = Encoding.ASCII.GetBytes(key.Trim());
                alg.ComputeHash(Encoding.UTF8.GetBytes(plainText.Trim()));
                return ToHexString(alg.Hash);
            }
        }

        public class SHA
        {
            /// <summary>
            /// Encryption plain text to SHA Hashed as Hex String.
            /// </summary>
            /// <param name="plainText"></param>
            /// <param name="size">SHA Size</param>
            /// <returns></returns>
            public static string Encrypt(string plainText, Size size = Size.SHA256)
            {
                return ToHexString(EncryptContent(plainText, size));
            }

            /// <summary>
            /// Encryption plain text to SHA Hashed as Hex String.
            /// </summary>
            /// <param name="plainText"></param>
            /// <param name="key">ASCII Key only</param>
            /// <param name="size">SHA Size</param>
            /// <returns></returns>
            public static string Encrypt(string plainText, string key, Size size = Size.SHA256)
            {
                return ToHexString(EncryptContentHMAC(plainText, key, size));
            }

            /// <summary>
            /// Encryption plain text to SHA Hashed as Base64 String.
            /// </summary>
            /// <param name="plainText">Content to encrypt</param>
            /// <param name="size">ASCII Key only</param>
            /// <returns></returns>
            public static string EncryptToBase64(string plainText, Size size = Size.SHA256)
            {
                return Convert.ToBase64String(EncryptContent(plainText, size));
            }

            /// <summary>
            /// Encryption plain text to SHA Hashed as Base64 String.
            /// </summary>
            /// <param name="plainText">Content to encrypt</param>
            /// <param name="key">ASCII Key only</param>
            /// <param name="size">Size</param>
            /// <returns></returns>
            public static string EncryptToBase64(string plainText, string key, Size size = Size.SHA256)
            {
                return Convert.ToBase64String(EncryptContentHMAC(plainText, key, size));
            }

            private static byte[] EncryptContent(string plainText, Size size = Size.SHA256)
            {
                plainText = plainText.Trim();

                switch (size)
                {
                    case Size.SHA1:
                        var algSHA1 = SHA1.Create();
                        algSHA1.ComputeHash(Encoding.UTF8.GetBytes(plainText));
                        return algSHA1.Hash;

                    case Size.SHA512:
                        var algSha512 = SHA512.Create();
                        algSha512.ComputeHash(Encoding.UTF8.GetBytes(plainText));
                        return algSha512.Hash;

                    case Size.SHA384:
                        var algSha384 = SHA384.Create();
                        algSha384.ComputeHash(Encoding.UTF8.GetBytes(plainText));
                        return algSha384.Hash;

                    default:
                        var algSha256 = SHA256.Create();
                        algSha256.ComputeHash(Encoding.UTF8.GetBytes(plainText));
                        return algSha256.Hash;
                }
            }

            private static byte[] EncryptContentHMAC(string plainText, string key, Size size = Size.SHA256)
            {
                var alg = HMAC.Create();

                alg.HashName = size.ToString();
                alg.Key = Encoding.ASCII.GetBytes(key.Trim());
                alg.ComputeHash(Encoding.UTF8.GetBytes(plainText));
                return alg.Hash;
            }

            public enum Size
            {
                SHA1,
                SHA256,
                SHA384,
                SHA512
            }
        }

        public class AES
        {
            // Setups
            private const int KEY_SIZE             = 256;           // Key Size
            private const int BLOCK_SIZE           = 128;           // Block Size
            private const PaddingMode PADDING_MODE = PaddingMode.PKCS7;
            private const CipherMode CIPHER_MODE   = CipherMode.CBC;

            private static IDataProtector protector;
            private static IDataProtector protectorLevel2;

            // Public methods

            public static void InitDataProtect(IDataProtectionProvider dataProtection)
            {
                protector = dataProtection.CreateProtector("Crypto.AES.Algorithm.Key");
                protectorLevel2 = dataProtection.CreateProtector("Crypto.AES.Algorithm.Key.Level2");
            }

            /// <summary>
            /// Encrypt content using DataProtectionProvider
            /// </summary>
            /// <param name="plainText"></param>
            /// <returns></returns>
            public static string Encrypt(string plainText)
            {
                return protector.Protect(plainText);
            }

            /// <summary>
            /// Decrypt content using DataProtectionProvider
            /// </summary>
            /// <param name="plainText"></param>
            /// <returns></returns>
            public static string Decrypt(string plainText)
            {
                try
                {
                    return protector.Unprotect(plainText);
                }
                catch
                {
                    // ignored
                    return string.Empty;
                }
            }

            /// <summary>
            /// Encrypt content with two level protect using DataProtectionProvider
            /// </summary>
            /// <param name="plainText"></param>
            /// <returns></returns>
            public static string EncryptTwoLevel(string plainText)
            {
                var lvl1 = protector.Protect(plainText);
                return protectorLevel2.Protect(lvl1);
            }

            /// <summary>
            /// Decrypt content with two level protect using DataProtectionProvider
            /// </summary>
            /// <param name="plainText"></param>
            /// <returns></returns>
            public static string DecryptTwoLevel(string plainText)
            {
                try
                {
                    var lvl1 = protectorLevel2.Unprotect(plainText);
                    return protector.Unprotect(lvl1);
                }
                catch
                {
                    return string.Empty;
                }
            }

            /// <summary>
            /// Encrypt text to base64 format string.
            /// </summary>
            /// <param name="platinText">Content want to encrypt</param>
            /// <param name="key">Key must have 32 characters</param>
            /// <returns></returns>
            public static string Encrypt(string platinText, string key)
            {
                // Check
                if (StringExtensions.IsEmptyOrNull(platinText, key))
                {
                    throw new Exception("Please fill full value parameters.");
                }

                // Check key length
                if (key.Length != 32)
                    throw new Exception("Key must have length is 32 character.");

                var bytesKey = Encoding.ASCII.GetBytes(key);

                return Convert.ToBase64String(Encrypt(platinText, bytesKey));
            }

            /// <summary>
            /// Decrypt base64 string Encrypt content to Orginal content.
            /// </summary>
            /// <param name="platinText"></param>
            /// <param name="key">Key must have 32 characters</param>
            /// <returns></returns>
            public static string Decrypt(string platinText, string key)
            {
                // Check
                if (StringExtensions.IsEmptyOrNull(platinText, key))
                {
                    throw new Exception("Please fill full value parameters.");
                }

                // Check key length
                if (key.Length != 32)
                    throw new Exception("Key must have length is 32 character.");

                var bytesKey = Encoding.ASCII.GetBytes(key);

                try
                {
                    var content = Convert.FromBase64String(platinText);
                    return Decrypt(content, bytesKey);
                }
                catch
                {
                    return null;
                }
            }


            // Private methods
            static byte[] Encrypt(string plainText, byte[] Key)
            {
                // Check arguments.
                if (plainText == null || plainText.Length <= 0)
                    throw new ArgumentNullException("plainText");
                if (Key == null || Key.Length <= 0)
                    throw new ArgumentNullException("Key");

                byte[] encrypted;
                byte[] iv;

                // Create an Aes object
                // with the specified key and IV.
                using (var aesAlg = Aes.Create())
                {
                    // Setup
                    aesAlg.KeySize = KEY_SIZE;
                    aesAlg.BlockSize = BLOCK_SIZE;
                    aesAlg.Mode = CIPHER_MODE;
                    aesAlg.Padding = PADDING_MODE;

                    aesAlg.Key = Key;
                    aesAlg.GenerateIV();

                    iv = aesAlg.IV;

                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(plainText);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }

                var combinedIvCt = new byte[iv.Length + encrypted.Length];
                Array.Copy(iv, 0, combinedIvCt, 0, iv.Length);
                Array.Copy(encrypted, 0, combinedIvCt, iv.Length, encrypted.Length);

                // Return the encrypted bytes from the memory stream.
                return combinedIvCt;
            }

            static string Decrypt(byte[] cipherTextCombined, byte[] Key)
            {
                // Check arguments.
                if (cipherTextCombined == null || cipherTextCombined.Length <= 0)
                    throw new ArgumentNullException("cipherText");
                if (Key == null || Key.Length <= 0)
                    throw new ArgumentNullException("Key");

                // Declare the string used to hold 
                // the decrypted text. 
                string plaintext = null;

                // Create an Aes object 
                // with the specified key and IV. 
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;

                    byte[] IV = new byte[aesAlg.BlockSize / 8];
                    byte[] cipherText = new byte[cipherTextCombined.Length - IV.Length];

                    Array.Copy(cipherTextCombined, IV, IV.Length);
                    Array.Copy(cipherTextCombined, IV.Length, cipherText, 0, cipherText.Length);

                    aesAlg.IV = IV;

                    aesAlg.Mode = CipherMode.CBC;

                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption. 
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {

                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                }

                return plaintext;

            }
        }

        public class RSA
        {
            private static X509Certificate2 certificate;
            private static RSAEncryptionPadding encryptionPadding;

            /// <summary>
            /// This method will init certificate from file.
            /// </summary>
            /// <param name="environment"></param>
            /// <param name="password"></param>
            public static void InitFromFile(IHostingEnvironment environment, string password)
            {
                // Init
                var certFilePath = Path.Combine(environment.ContentRootPath, "Certificates/Certificate.pfx");
                certificate = new X509Certificate2(certFilePath, password, X509KeyStorageFlags.UserKeySet);
                encryptionPadding = RSAEncryptionPadding.OaepSHA512;
            }

            /// <summary>
            /// This method will get Certificate from Computer. So you much read Readme for more information
            /// </summary>
            /// <param name="certificateThumbprint">Certificate Thumbprint</param>
            public static void Init(string certificateThumbprint)
            {
                var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadWrite);

                // Make Up Thumbprint
                certificateThumbprint = Regex.Replace(certificateThumbprint, @"[^0-9a-zA-Z]+", "");

                var certificates = store.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    certificateThumbprint,
                    true);
                if (certificates.Count == 1)
                {
                    encryptionPadding = RSAEncryptionPadding.OaepSHA1;
                    certificate = certificates[0];
                }
                else
                {
                    // Certificate not installed
                    throw new Exception("Certificate not installed. Please check Readme for more information.");
                }
                store.Close();
            }


            /// <summary>
            /// Encrypt content using Certificate. You must Initialize certificate before using.
            /// </summary>
            /// <param name="content"></param>
            /// <returns></returns>
            public static string Encrypt(string content)
            {
                using (var rsa = certificate.GetRSAPublicKey())
                    return Convert.ToBase64String(
                        rsa.Encrypt(
                            Encoding.UTF8.GetBytes(content), 
                            encryptionPadding));
            }

            /// <summary>
            /// Decrypt content using Certificate. You must Initialize certificate before using.
            /// </summary>
            /// <param name="content"></param>
            /// <returns></returns>
            public static string Decrypt(string content)
            {
                try
                {
                    using (var rsa = certificate.GetRSAPrivateKey())
                        return Encoding.UTF8.GetString(
                            rsa.Decrypt(Convert.FromBase64String(content), encryptionPadding)
                        );
                }
                catch
                {
                    //ignore
                    return string.Empty;
                }
            }
        }
    }
}
