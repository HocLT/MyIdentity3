using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace MyIdentity.Areas.Admin.Pages.Roles
{
    public class AddModel : PageModel
    {
        readonly RoleManager<IdentityRole> roleManager;

        public AddModel(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage { get; set; }
        
        public class InputModel
        {
            public string? Id { set; get; }
            [Required(ErrorMessage = "Please input the role name")]
            [StringLength(200, ErrorMessage = "{0} has length {2} to {1}", MinimumLength = 3)]
            public string? Name { set; get; }
        }

        [BindProperty]
        public InputModel Input { set; get; }

        [BindProperty]
        public bool IsUpdate { set; get; }

        public IActionResult OnGet() => NotFound("Page not found.");
        public IActionResult OnPost() => NotFound("Page not found.");

        public IActionResult OnPostStartNewRole()
        {
            StatusMessage = "Please input data to create new Role.";
            IsUpdate = false;
            ModelState.Clear();     // xóa thông báo lỗi => nếu có.
            return Page();
        }

        public async Task<IActionResult> OnPostStartUpdate()
        {
            StatusMessage = "";
            IsUpdate = true;
            if (Input.Id == null)
            {
                StatusMessage = "Role invalid.";
                return Page();
            }

            var rr = await roleManager.FindByIdAsync(Input.Id);
            if (rr == null)
            {
                StatusMessage = "Role not found.";
            }
            else
            {
                ModelState.Clear();     // xóa thông báo lỗi => nếu có.
                Input.Name = rr.Name;
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = "";
                return Page();
            }

            if (IsUpdate)
            {
                if (Input.Id == null)
                {
                    ModelState.Clear();
                    StatusMessage = "Error: Role invalid.";
                    return Page();
                }

                var rr = await roleManager.FindByIdAsync(Input.Id);
                if (rr == null)
                {
                    StatusMessage = "Error: Role not found.";
                }
                else
                {
                    rr.Name = Input.Name;   // update name
                    var rrResult = await roleManager.UpdateAsync(rr);
                    if (rrResult.Succeeded)
                    {
                        StatusMessage = "Update Role successfully.";
                        return RedirectToPage("./Index");
                    }
                    else
                    {
                        StatusMessage = "Error: ";
                        foreach (var err in rrResult.Errors)
                        {
                            StatusMessage += err.Description;
                        }
                    }
                }
            }
            else
            {
                // tạo role
                var newRr = new IdentityRole(Input.Name);
                var rr = await roleManager.CreateAsync(newRr);
                if (rr.Succeeded)
                {
                    StatusMessage = "Create new Role successfully.";
                    return RedirectToPage("./Index");
                }
                else
                {
                    StatusMessage = "Error: ";
                    foreach (var err in rr.Errors)
                    {
                        StatusMessage += err.Description;
                    }
                }
            }

            return Page();
        }
    }
}
