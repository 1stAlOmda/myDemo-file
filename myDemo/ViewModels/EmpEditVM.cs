using Microsoft.AspNetCore.Http;
using ORMEFCoreDA.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace myDemo.ViewModels
{
    public class EmpEditVM  : EmpVM
    {
        public int Id { get; set; }
        public string ExistingPhoto { get; set; }


    }
}
