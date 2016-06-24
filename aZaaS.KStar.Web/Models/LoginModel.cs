using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models
{
    public class LoginModel
    {
        
        [Required(ErrorMessage = "Account is requirement.")]
        public string Account { get; set; }

        [Required(ErrorMessage = "Password is requirement.")]
        public string Password { get; set; }
         
        public string code { get; set; }
        public AuthenticationMethod AuthMethod { get; set; }

        public string Integrated { get; set; }

        public string ReturnUrl { get; set; }

    }

    public enum AuthenticationMethod
    {
        SQLForms = 1,
        MutilAD = 2        
    }
}