using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyIdentity.Data;
using System.ComponentModel.DataAnnotations;

namespace MyIdentity.Areas.Admin.Pages.Roles
{
    public class AddUserRoleModel : PageModel
    {
        readonly UserManager<AppUser> userManager;
        readonly RoleManager<IdentityRole> roleManager;

        public AddUserRoleModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string? Id { set; get; }
            public string? Name { set; get; }

            public string[]? RoleName { set; get; }
        }

        [BindProperty]
        public InputModel Input { set; get; }

        [BindProperty]
        public bool IsConfirm { set; get; }

        //public List<string> Roles { set; get; } = new List<string>();
        public SelectList Roles { set; get; }

        public IActionResult OnGet() => NotFound("Page not found.");

        public async Task<IActionResult> OnPost()
        {
            var user = await userManager.FindByIdAsync(Input.Id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // gán Name
            Input.Name = user.UserName; 

            // đọc roles của user
            var uRs = await userManager.GetRolesAsync(user);

            // tất cả role trong db, select name
            var aRr = await roleManager.Roles
                .Select(r=>r.Name)
                .ToListAsync();
            Roles = new SelectList(aRr);
            
            if (!IsConfirm)
            {
                Input.RoleName = uRs.ToArray();
                IsConfirm = true;
                StatusMessage = "";
                ModelState.Clear();
            }
            else
            {
                StatusMessage = "Updated.";
                if (Input.RoleName == null)
                {
                    Input.RoleName = new string[] { };
                }

                // thêm role mới
                foreach (var role in Input.RoleName)
                {
                    if (!uRs.Contains(role))
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
                // xóa role không được chọn
                foreach(var role in uRs) 
                {
                    if (!Input.RoleName.Contains(role))
                    {
                        await userManager.RemoveFromRoleAsync(user, role);
                    }
                }
            }

            return Page();
        }
    }
}
