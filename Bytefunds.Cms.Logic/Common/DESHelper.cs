using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bytefunds.Cms.Logic.Common
{
    public class DESHelper
    {

        private static string Key = "supwin88";
        #region DES加密解密

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="data">待加密的字符串</param>
        /// <param name="iv">对称,要求为8位</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static byte[] EncryptDES(byte[] data, byte[] iv, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = iv;
                byte[] inputByteArray = data;
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(inputByteArray, 0, inputByteArray.Length);
                        cStream.FlushFinalBlock();
                        return mStream.ToArray();
                    }
                }
            }
            catch
            {
                return data;
            }
        }



        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="data">待解密的字符串</param>
        /// <param name="iv">对称解密密钥,要求为8位,和加密密钥相同</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回加密后的字符串，失败返回源串</returns>
        public static byte[] DecryptDES(byte[] data, byte[] iv, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
                byte[] rgbIV = iv;
                byte[] inputByteArray = data;
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(inputByteArray, 0, inputByteArray.Length);
                        cStream.FlushFinalBlock();
                        return mStream.ToArray();
                    }
                }
            }
            catch
            {
                //return decryptString;
                return data;
            }
        }

        /// <summary>
        /// 解密字符串，使用本类自定义的解密key
        /// </summary>
        /// <param name="data">解密的字符串</param>
        /// <returns>解密成功返回加密后的字符串，失败返回源串</returns>
        public static string DecryptDES(string data)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(Key.Substring(0, 8));
                byte[] rgbIV = Encoding.UTF8.GetBytes(Key.Substring(0, 8));
                byte[] inputByteArray = Convert.FromBase64String(data);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();

                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(inputByteArray, 0, inputByteArray.Length);
                        cStream.FlushFinalBlock();
                        return Encoding.UTF8.GetString(mStream.ToArray());
                    }
                }
            }
            catch
            {
                //return decryptString;
                return data;
            }
        }
        /// <summary>
        /// 加密字符串，使用本类自定义的解密key
        /// </summary>
        /// <param name="data">加密的字符串</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string data)
        {
            try
            {
                //string;
                byte[] rgbKey = Encoding.UTF8.GetBytes(Key.Substring(0, 8));
                byte[] rgbIV = Encoding.UTF8.GetBytes(Key.Substring(0, 8));
                byte[] inputByteArray = Encoding.UTF8.GetBytes(data);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(inputByteArray, 0, inputByteArray.Length);
                        cStream.FlushFinalBlock();
                        return Convert.ToBase64String(mStream.ToArray());
                    }
                }
            }
            catch
            {
                return data;
            }
        }

        #endregion
    }
}
