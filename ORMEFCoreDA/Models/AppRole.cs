using Microsoft.AspNetCore.Identity;

namespace ORMEFCoreDA.Models
{
    public class AppRole : IdentityRole
    {
        
        public string InsertUser { get; set; }
        public string InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public string UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }

   
}