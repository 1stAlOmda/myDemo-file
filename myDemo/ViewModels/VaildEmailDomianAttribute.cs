using System;
using System.ComponentModel.DataAnnotations;

namespace myDemo.ViewModels
{
    public  class VaildEmailDomianAttribute : ValidationAttribute
    {
        private readonly string validEmailDomain;

        public VaildEmailDomianAttribute(string ValidEmailDomain)
        {
            validEmailDomain = ValidEmailDomain;
        }

        public override bool IsValid(object value)
        {
            string[] res = value.ToString().Split("@");
            return res[1].ToUpper() == validEmailDomain.ToUpper();
        }
    }
}