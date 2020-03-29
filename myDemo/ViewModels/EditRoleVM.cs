using Microsoft.AspNetCore.Http;
using ORMEFCoreDA.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace myDemo.ViewModels
{

    public class EditRoleVM
    {
        public EditRoleVM()
        {
            //    RoleUsers = new List<string>();

        }
        public string id { get; set; }
        public string RoleName { get; set; }
        public List<string> RoleUsers { get; set; } = new List<string>();
    }
}