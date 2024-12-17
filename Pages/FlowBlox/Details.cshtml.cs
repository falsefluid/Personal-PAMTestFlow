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
    public class DetailsModel : PageModel
    {
        private readonly PAMTestFlow.PAMContext _context;

        public DetailsModel(PAMTestFlow.PAMContext context)
        {
            _context = context;
        }

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
    }
}
