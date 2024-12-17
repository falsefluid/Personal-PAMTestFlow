using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PAMTestFlow;

namespace PAMTestFlow.Pages.BloxBuilder
{
    public class DetailsModel : PageModel
    {
        private readonly PAMTestFlow.PAMContext _context;

        public DetailsModel(PAMTestFlow.PAMContext context)
        {
            _context = context;
        }

        public BuildingBlock BuildingBlock { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buildingblock = await _context.BuildingBlocks
                .Include(bb => bb.TypeBlock)
                .Include(bb => bb.Template)
                .Include(bb => bb.UserInputs) // Include UserInputs as well
                .FirstOrDefaultAsync(m => m.BlockID == id);


            if (buildingblock is not null)
            {
                BuildingBlock = buildingblock;

                return Page();
            }

            return NotFound();
        }
    }
}
