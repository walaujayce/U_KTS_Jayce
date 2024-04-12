using ControlzEx.Standard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kartos.Models.Auth
{
    class Certificate
    {

        /// <summary>
        /// AES 編碼
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static byte[] Encrypt(string plainText)
        {
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.GenerateKey();
                aesAlg.GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        /// <summary>
        /// AES解碼
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string Decrypt(byte[] cipherText)
        {
           try
    {
        using (AesManaged aesAlg = new AesManaged())
        {
            aesAlg.GenerateIV();
            aesAlg.GenerateKey();

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
    catch (CryptographicException ex)
    {
        // Handle decryption errors
        Console.WriteLine("Decryption error: " + ex.Message);
        return null; // or throw exception or handle in your specific way
    }
        }

        /// <summary>
        /// 取得本地端電腦的裝置識別碼
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_ComputerSystemProduct");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
               return mo["UUID"].ToString();
            }

            return string.Empty;
        }


    }
}
