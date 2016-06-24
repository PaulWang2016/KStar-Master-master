using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2.Framework.SSO
{
    public class SSOError
    {
        //public  const  int   FailReadCryptedKeyFromDB        = 1 ;	

        public const int SymmetricCryptographerExceptionID = 1;
        public const int ConvertExceptionID = 2;
        public const int CryptKeyReceiverExceptionID = 3;
        public const int CryptKeyManagerExceptionID = 4;



        public const string SymmetricCryptographerExceptionPolicy = "SymmetricCryptographerExceptionPolicy";
        public const string ConvertExceptionPolicy = "ConvertExceptionPolicy";
        public const string CryptKeyReceiverExceptionPolicy = "CryptKeyReceiverExceptionPolicy";
        public const string CryptKeyManagerExceptionPolicy = "CryptKeyManagerExceptionPolicy";
    }
}
