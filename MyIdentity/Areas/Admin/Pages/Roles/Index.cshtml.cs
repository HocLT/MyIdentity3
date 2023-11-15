using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MyIdentity.Areas.Admin.Pages.Roles
{
    [Authorize("Admin")]
    public class IndexModel : PageModel
    {
        readonly RoleManager<IdentityRole> roleManager;

        public IndexModel(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public List<IdentityRole> Roles { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Roles = await roleManager.Roles.ToListAsync();
            return Page();
        }
    }
}
