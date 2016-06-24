using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace aZaaS.KStar.Web.Helper
{
    public class EncryptHelper
    {
        /// <summary>  
        /// IV  初始化向量 
        /// </summary>  
        private static readonly byte[] IV = { 0xAB, 0xCD, 0xEF, 0x00, 0x78, 0x56, 0x34, 0x12 };
        /// <summary>
        /// 定义加密key常量
        /// </summary>
        private const string encryptKey="4LEhvagYtE+oWL7hzUc2wtYq7gOV7ouRYvQKzuinPPwDj4FqfJFCEdoafcoyfhrtYhZu26Q6EqTQly/4aOPCD7VDQF8jxK04tztdQpH2jFg4RfRVZ3iEWjeTe2JyLKzucif7LBkb3zIJ4Yg5dGBftN28qJjDw1I0NSoTFTe2fPk=";
        /// <summary>  
        /// 加密字符串  
        /// </summary>  
        /// <returns>返回密文</returns>  
        public static String EncryptString(String str, String key = encryptKey)
        {
            byte[] data = Encoding.Default.GetBytes(str);
            byte[] result = EncryptData(data, key);
            if (result != null)
                return Convert.ToBase64String(result, 0, result.Length);
            else
                return "";
        }
        /// <summary>  
        /// 加密二进制数据  
        /// </summary>  
        /// <returns>返回二进制密文</returns>  
        public static byte[] EncryptData(byte[] data, String key = encryptKey)
        {
            byte[] bKey = Encoding.Default.GetBytes(key.Substring(0, 8));
            byte[] bIV = IV;
            try
            {
                DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
                desc.Mode = CipherMode.ECB;
                desc.Padding = PaddingMode.Zeros;
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, desc.CreateEncryptor(bKey, bIV), CryptoStreamMode.Write);
                cStream.Write(data, 0, data.Length);
                cStream.FlushFinalBlock();
                return mStream.ToArray();
            }
            catch
            {
                return null;
            }
        }
        /// <summary>  
        /// 把密文解密为明文  
        /// </summary>  
        /// <returns>返回明文</returns>  
        public static String DecryptString(String decryptStr, String key = encryptKey)
        {
            byte[] data = Convert.FromBase64String(decryptStr);
            byte[] result = DecryptData(data, key);
            if (result != null)
                return Encoding.Default.GetString(result, 0, result.Length);
            else
                return "";
        }

        /// <summary>  
        /// 把二进制密文解密为明文二进制  
        /// </summary>  
        /// <returns>返回明文二进制</returns>  
        public static byte[] DecryptData(byte[] data, String key = encryptKey)
        {
            try
            {
                byte[] bKey = Encoding.Default.GetBytes(key.Substring(0, 8));
                byte[] bIV = IV;
                DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
                desc.Mode = CipherMode.ECB;
                desc.Padding = PaddingMode.Zeros;
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, desc.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write);
                cStream.Write(data, 0, data.Length);
                cStream.FlushFinalBlock();
                return mStream.ToArray();
            }
            catch
            {
                return null;
            }
        }  
    }
}