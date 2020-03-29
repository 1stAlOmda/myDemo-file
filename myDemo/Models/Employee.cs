using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace myDemo.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
      
        [Required]
        public string Email { get; set; }
      
        [Required]
        public Dep? Department { get; set; }

     


    }
}
