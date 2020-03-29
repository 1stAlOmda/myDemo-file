using Microsoft.AspNetCore.Identity;

namespace ORMEFCoreDA.Models
{
    public class AppUser : IdentityUser
    {
        public City? city { get; set; }
        public string InsertUser { get; set; }
        public string InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public string UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }

    public enum City
    {
        london,newYork,HongKong,cairo
    }
}