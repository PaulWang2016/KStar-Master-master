using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace K2.Framework.SSO
{
    public class SymmetricCryptographer
    {

        public static DES GetDESCryptoServiceProvider()
        {
            DES cipher = new DESCryptoServiceProvider();
            cipher.Mode = CipherMode.CBC;
            cipher.Padding = PaddingMode.PKCS7;
            return cipher;
        }


        public static byte[] Encrypt(byte[] keyBytes, byte[] ivBytes, byte[] contentBytes)
        {

            DES cipher = GetDESCryptoServiceProvider();
            ICryptoTransform ct = cipher.CreateEncryptor(keyBytes, ivBytes);
            MemoryStream ms = new MemoryStream();
            CryptoStream stream = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            stream.Write(contentBytes, 0, contentBytes.Length);
            stream.Close();
            byte[] encs = ms.ToArray();
            return encs;
        }


        public static byte[] Decrypt(byte[] keyBytes, byte[] ivBytes, byte[] secretBytes)
        {
            DES cipher = GetDESCryptoServiceProvider();
            ICryptoTransform ct = cipher.CreateDecryptor(keyBytes, ivBytes);
            MemoryStream ms = new MemoryStream();
            CryptoStream stream = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            stream.Write(secretBytes, 0, secretBytes.Length);
            stream.Close();
            byte[] encs = ms.ToArray();
            return encs;
        }

    }

}
