using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PAMTestFlow;

namespace PAMTestFlow.Pages.FlowBlox
{
    public class CreateModel : PageModel
    {
        private readonly PAMTestFlow.PAMContext _context;

        public CreateModel(PAMTestFlow.PAMContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Template Template { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Add the new Template to the database
            _context.Templates.Add(Template);
            await _context.SaveChangesAsync();

            // Retrieve the TemplateID of the newly added Template
            int newTemplateID = Template.TemplateID; // Adjust according to your model's primary key name

            // Call the stored procedure to set the CreatedDate
            await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.UpdateCreatedDate @TemplateID = {newTemplateID}");

            return RedirectToPage("./Index");
        }
    }
}