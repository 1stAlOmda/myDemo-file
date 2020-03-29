using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ORMEFCoreDA.Models
{
    public static class ModelBuliderExtention
    {
        public static void seed(this ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, Name = "Ahmed", Email = "EngAmmh86@gmail.com", Department = Dep.IT },
                new Employee { Id = 2, Name = "Mohammed", Email = "EngMohammed@gmail.com", Department = Dep.HR },
                new Employee { Id = 3, Name = "Hesham", Email = "EngHesham@gmail.com", Department = Dep.IT }
                );
        }
    }
}
