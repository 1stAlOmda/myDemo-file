using Microsoft.AspNetCore.Mvc;
using ORMEFCoreDA.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace myDemo.ViewModels
{
    public class RegisterVM
    {

        [Required]
        [EmailAddress]
        [Remote(controller:"Account" , action:"EmailIsInUsed",ErrorMessage ="The Email is Already taken :: from VMClass")]
        [VaildEmailDomian(ValidEmailDomain: "gmail.com", ErrorMessage = "The Email is Not GMAIL")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password",
            ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public City? city { get; set; }
    }
}

