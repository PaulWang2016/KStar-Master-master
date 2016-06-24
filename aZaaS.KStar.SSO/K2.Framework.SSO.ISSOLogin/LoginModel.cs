namespace K2.Framework.SSO.ISSOLogin
{
    using System;
    using System.Runtime.CompilerServices;

    public class LoginModel
    {
        public string Account { get; set; }

        public string Code { get; set; }

        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}

