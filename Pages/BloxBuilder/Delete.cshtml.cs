using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PAMTestFlow;

namespace PAMTestFlow.Pages.BloxBuilder
{
    public class DeleteModel : PageModel
    {
        private readonly PAMTestFlow.PAMContext _context;

        public DeleteModel(PAMTestFlow.PAMContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BuildingBlock BuildingBlock { get; set; } = default!;

        // GET: /BloxBuilder/Delete/{id}
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buildingblock = await _context.BuildingBlocks
                .Include(bb => bb.TypeBlock)
                .Include(bb => bb.Template)
                .Include(bb => bb.UserInputs)
                .FirstOrDefaultAsync(m => m.BlockID == id);

            if (buildingblock == null)
            {
                return NotFound();
            }

            BuildingBlock = buildingblock;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buildingblock = await _context.BuildingBlocks
                .Include(bb => bb.UserInputs) 
                .FirstOrDefaultAsync(m => m.BlockID == id);

            if (buildingblock == null)
            {
                return NotFound();
            }

            if (buildingblock != null)
            {
                BuildingBlock = buildingblock;

                // Remove the BuildingBlock (and related UserInputs will be deleted due to cascade delete)
                _context.BuildingBlocks.Remove(BuildingBlock);

                // Save changes to apply deletion
                await _context.SaveChangesAsync();

                // Fix the sequence order of the remaining blocks
                var blocksToUpdate = await _context.BuildingBlocks
                    .Where(bb => bb.TemplateID == BuildingBlock.TemplateID && bb.SequenceOrder > BuildingBlock.SequenceOrder)
                    .ToListAsync();

                foreach (var block in blocksToUpdate)
                {
                    block.SequenceOrder--;
                }

                await _context.SaveChangesAsync();
                await _context.Database.ExecuteSqlRawAsync("EXEC dbo.UpdateUpdatedDate @TemplateID = {0}", BuildingBlock.TemplateID);
            }

            // Redirect to the Index page after deletion
            return RedirectToPage("./Index", new { templateId = BuildingBlock.TemplateID });
        }
    }
}