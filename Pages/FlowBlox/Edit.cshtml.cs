using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PAMTestFlow;

namespace PAMTestFlow.Pages.FlowBlox
{
    public class EditModel : PageModel
    {
        private readonly PAMTestFlow.PAMContext _context;

        public EditModel(PAMTestFlow.PAMContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Template Template { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var template =  await _context.Templates.FirstOrDefaultAsync(m => m.TemplateID == id);
            if (template == null)
            {
                return NotFound();
            }
            Template = template;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
{
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Template).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();

            // Call the stored procedure to update the UpdatedDate
            await _context.Database.ExecuteSqlRawAsync("EXEC dbo.UpdateUpdatedDate @TemplateID = {0}", Template.TemplateID);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TemplateExists(Template.TemplateID))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

        private bool TemplateExists(int id)
        {
            return _context.Templates.Any(e => e.TemplateID == id);
        }
    }
}
