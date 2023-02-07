using NomadicBook.Models.db;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NomadicBook.Utils
{
    public class Security
    {
        private static byte[] key = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /// <summary>
        /// 傳入字串做md5加密再傳出
        /// </summary>
        /// <param name="value">傳入字串</param>
        /// <returns>加密後字串(會有32個字元)</returns>
        public static string Encryption(string value)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(Encoding.Default.GetBytes(value));
            StringBuilder connectionPassword = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                connectionPassword.Append(data[i].ToString("x2"));
            }
            return connectionPassword.ToString();
        }
        /// <summary>
        /// AES加密演算法
        /// </summary>
        /// <param name="plainText">明文字串</param>
        /// <param name="strKey">金鑰</param>
        /// <returns>返回加密後的密文位元組陣列</returns>
        public static string AESEncrypt(string plainText, string strKey)
        {
            //分組加密演算法
            SymmetricAlgorithm des = Rijndael.Create();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(plainText);//得到需要加密的位元組陣列
            //設定金鑰及金鑰向量
            des.Key = Encoding.UTF8.GetBytes(strKey);
            des.IV = key;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            byte[] cipherBytes = ms.ToArray();//得到加密後的位元組陣列
            cs.Close();
            ms.Close();
            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="cipher">密文位元組陣列</param>
        /// <param name="strKey">金鑰</param>
        /// <returns>返回解密後的字串</returns>
        public static string AESDecrypt(string cipher, string strKey)
        {
            byte[] cipherText = Convert.FromBase64String(cipher);
            SymmetricAlgorithm des = Rijndael.Create();
            des.Key = Encoding.UTF8.GetBytes(strKey);
            des.IV = key;
            byte[] decryptBytes = new byte[cipherText.Length];
            MemoryStream ms = new MemoryStream(cipherText);
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read);
            cs.Read(decryptBytes, 0, decryptBytes.Length);
            cs.Close();
            ms.Close();
            return Encoding.ASCII.GetString(decryptBytes);
        }
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="input">要加密的字串</param>
        /// <param name="encode">編碼</param>
        /// <returns></returns>
        public static string Base64Encrypt(string input, Encoding encode)
        {
            return Convert.ToBase64String(encode.GetBytes(input));
        }
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="input">要解密的字串</param>
        /// <param name="encode">編碼</param>
        /// <returns></returns>
        public static string Base64Decrypt(string input, Encoding encode)
        {
            return encode.GetString(Convert.FromBase64String(input));
        }
    }
}
