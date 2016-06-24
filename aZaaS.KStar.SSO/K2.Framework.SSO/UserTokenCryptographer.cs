using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2.Framework.SSO
{
    public class UserTokenCryptographer
    { 

        /// <summary>
        /// 加密键（字节格式）
        /// <param name="crypter">加密调用者类别：用来判断如何获得加密键</param> 
        /// </summary>
        private static byte[] GetKeyBytes(CrypterKind crypter)
        {
            string skey = "";
             
            switch (crypter)
            {
                case CrypterKind.SSO:
                    skey =SSOConst.SSOUserTokenKey; //CryptKeyManager.GetCorrectKey();
                    break; 

            }
            return System.Text.UTF8Encoding.UTF8.GetBytes(skey);
        }

  
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="contentString">要加密的字符串内容</param>
        /// <param name="crypter">加密调用者类别：用来判断如何获得加密键</param> 
        /// <returns>返回加密结果（base64)</returns>
        public static string Encrypt(string contentString, CrypterKind crypter)
        {
            string rtValue = "";
            byte[] contentBytes = System.Text.UTF8Encoding.UTF8.GetBytes(contentString);
            byte[] keys = GetKeyBytes(crypter);
            try
            {

                byte[] ret = SymmetricCryptographer.Encrypt(keys, keys, contentBytes);
                rtValue = Convert.ToBase64String(ret);
            }
            catch (Exception ex)
            {
                throw new CustomSSOException("加密异常", ex, SSOError.SymmetricCryptographerExceptionID);
            }

            return rtValue;
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="secretBase64">密文(base64)</param>
        /// <param name="crypter">加密调用者类别：用来判断如何获得加密键</param> 
        /// <returns>原文字符串</returns> 
        public static string Decrypt(string secretBase64, CrypterKind crypter)
        {
            string rtValue = "";
            byte[] secretBytes;
            byte[] keys = GetKeyBytes(crypter);

            try
            {
                secretBytes = Convert.FromBase64String(secretBase64);
            }
            catch (Exception ex)
            {
                throw new CustomSSOException("解密前把base64“" + secretBase64 + "”转为字节数组异常", ex, SSOError.ConvertExceptionID);
            }


            try
            {
                byte[] rt = SymmetricCryptographer.Decrypt(keys, keys, secretBytes);
                rtValue = System.Text.UTF8Encoding.UTF8.GetString(rt);
            }
            catch (Exception ex)
            {

                //throw   new  CustomSSOException ( "单点登录解密异常" , ex , SSOError.ExceptionSymmetricCryptographer ) ;                      
                rtValue = "";
            }

            return rtValue;

        }


    }
}
