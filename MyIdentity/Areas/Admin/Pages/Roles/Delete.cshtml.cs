using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace MyIdentity.Areas.Admin.Pages.Roles
{
    public class DeleteModel : PageModel
    {
        readonly RoleManager<IdentityRole> roleManager;

        public DeleteModel(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string? Id { set; get; }
            public string? Name { set; get; }
        }

        [BindProperty]
        public InputModel Input { set; get; }

        [BindProperty]
        public bool IsConfirm { set; get; }

        public IActionResult OnGet() => NotFound("Page not found.");

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return NotFound("Can not delete.");
            }

            var rr = await roleManager.FindByIdAsync(Input.Id);
            if (rr == null)
            {
                return NotFound("Role note found.");
            }

            ModelState.Clear();
            if (IsConfirm)
            {
                // xóa
                await roleManager.DeleteAsync(rr);
                StatusMessage = $"Role {rr.Name} deleted";
            }
            else
            {
                Input.Name = rr.Name;
                IsConfirm = true;
            }

            return Page();
        }
    }
}
