using System;
using System.Security.Cryptography;
using System.Text;

namespace SLII_Web.Classes
{
    public class General
    {
        /// <summary>
        /// Get Decryption value of the encrypted text
        /// </summary>
        public static string GetDecryptedValue(string encryptedValue, string key)
        {
            byte[] inputArray = Convert.FromBase64String(encryptedValue);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();

            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();

            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// Get Encryption value of the plain text
        /// </summary>
        public static string GetEncryptedValue(string decryptedValue, string key)
        {
            byte[] inputArray = Encoding.UTF8.GetBytes(decryptedValue);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();

            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static int GetQueryStringInt(string value)
        {
            int rValue = -1;

            if (value != string.Empty)
            {
                int.TryParse(value, out rValue);
            }

            return rValue;
        }

    }
}