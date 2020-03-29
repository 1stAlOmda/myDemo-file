using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ORMEFCoreDA.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ORMEFCoreDA.Models
{
  public  class AppDbContext :IdentityDbContext<AppUser> /*DbContext*/
    {
        public AppDbContext(DbContextOptions<AppDbContext> Options):base(Options)
        { }

        public DbSet<Employee> Employee { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.seed();
        }
    }
}
