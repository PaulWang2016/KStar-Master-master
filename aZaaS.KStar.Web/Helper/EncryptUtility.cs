using System;
using System.Security.Cryptography;
using System.Text;
namespace aZaaS.KStar.Web.Helper
{
	internal class EncryptUtility
	{
		private const int saltLenght = 12;
		internal static string Encrypt(string strPwd)
		{
			MD5 mD = new MD5CryptoServiceProvider();
			byte[] bytes = Encoding.Default.GetBytes(strPwd);
			byte[] array = mD.ComputeHash(bytes);
			mD.Clear();
			string text = "";
			for (int i = 0; i < array.Length - 1; i++)
			{
				text += array[i].ToString("x").PadLeft(2, '0');
			}
			return text;
		}
		internal static string CreateSalt()
		{
			RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
			byte[] array = new byte[12];
			rNGCryptoServiceProvider.GetBytes(array);
			return Convert.ToBase64String(array);
		}
		internal static string GetPasswordHash(string pwd, string salt)
		{
			string strPwd = pwd + salt;
			return EncryptUtility.Encrypt(strPwd);
		}
	}
}
