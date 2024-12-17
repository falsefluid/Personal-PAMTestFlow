using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PAMTestFlow;

namespace PAMTestFlow.Pages.FlowBlox
{
    public class DeleteModel : PageModel
    {
        private readonly PAMTestFlow.PAMContext _context;

        public DeleteModel(PAMTestFlow.PAMContext context)
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

            var template = await _context.Templates.FirstOrDefaultAsync(m => m.TemplateID == id);

            if (template is not null)
            {
                Template = template;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var template = await _context.Templates.FindAsync(id);
            if (template != null)
            {
                Template = template;

                // Delete all user inputs associated with the blocks
                var blocks = await _context.BuildingBlocks
                    .Where(bb => bb.TemplateID == template.TemplateID)
                    .ToListAsync();

                var userInputs = await _context.UserInputs
                    .Where(ui => blocks.Select(bb => bb.BlockID).Contains(ui.BlockID))
                    .ToListAsync();

                _context.UserInputs.RemoveRange(userInputs);

                // Save changes to delete the user inputs
                await _context.SaveChangesAsync();

                // Delete all blocks associated with the template
                _context.BuildingBlocks.RemoveRange(blocks);

                // Save changes to delete the blocks
                await _context.SaveChangesAsync();

                // Delete the template
                _context.Templates.Remove(Template);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}