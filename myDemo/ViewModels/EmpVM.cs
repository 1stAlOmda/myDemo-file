using Microsoft.AspNetCore.Http;
using ORMEFCoreDA.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace myDemo.ViewModels
{
    public class EmpVM 
    {
        
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public Dep? Department { get; set; }

        public IFormFile Photo { get; set; }
    }
}
