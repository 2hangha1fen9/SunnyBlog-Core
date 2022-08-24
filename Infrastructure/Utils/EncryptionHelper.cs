using NETCore.Encrypt;
using NETCore.Encrypt.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class EncryptionHelper
    {
        /// <summary>
        /// AES密钥
        /// </summary>
        private static string AES_KEY = "QPz42i1dJ6hkVrRn";

        /// <summary>
        /// 获取随机数序列
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomSequnce(int length)
        {
            StringBuilder result = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                result.Append(random.Next(9));
            }
            return result.ToString();
        }

        /// <summary>
        /// 哈希摘要
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ShaEncrypt(this string data)
        {
            return EncryptProvider.Sha1(data);
        }

        /// <summary>
        /// 验证哈希值
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ShaCheck(this string hash,string data)
        {
            return ShaEncrypt(data).Equals(hash);
        }

        /// <summary>
        /// AES对称加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string AESEncrypt(this string data)
        {
            return EncryptProvider.AESEncrypt(data, AES_KEY);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="encData"></param>
        /// <returns></returns>
        public static string AESDecrypt(this string encData)
        {
            return EncryptProvider.AESDecrypt(encData, AES_KEY);
        }
    }
}
