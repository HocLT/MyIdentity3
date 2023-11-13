using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyIdentity.Data;

namespace MyIdentity.Areas.Admin.Pages.Roles
{
    public class UserModel : PageModel
    {
        readonly UserManager<AppUser> userManager;
        readonly RoleManager<IdentityRole> roleManager;

        public UserModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public class UserAndRole: AppUser
        {
            public string? ListRoles { set; get; }
        }

        public List<UserAndRole> Users { set; get; }

        public IActionResult OnPost() => NotFound("Page not found.");

        public async Task<IActionResult> OnGet()
        {
            Users = await userManager.Users
                .Select(u => new UserAndRole { 
                    Id = u.Id,
                    UserName = u.UserName,
                })
                .ToListAsync();

            // set role
            foreach (var user in Users)
            {
                var roles = await userManager.GetRolesAsync(user);
                user.ListRoles = string.Join(", ", roles.ToList());
            }

            return Page();
        }
    }
}
