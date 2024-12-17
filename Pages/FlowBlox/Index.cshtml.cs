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
    public class IndexModel : PageModel
    {
        private readonly PAMTestFlow.PAMContext _context;

        public IndexModel(PAMTestFlow.PAMContext context)
        {
            _context = context;
        }

        public IList<Template> Template { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Template = await _context.Templates.ToListAsync();
        }
    }
}
