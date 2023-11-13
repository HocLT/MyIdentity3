using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MyIdentity.Data
{
    public class AppUser: IdentityUser
    {
        [PersonalData]
        [MaxLength(100)]
        public string? Name { get; set; }
        [PersonalData]
        public DateTime? Birthday { get; set; }
    }
}
