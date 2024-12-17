using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PAMTestFlow;

namespace PAMTestFlow.Pages.BloxBuilder
{
    public class EditModel : PageModel
    {
        private readonly PAMTestFlow.PAMContext _context;

        public EditModel(PAMTestFlow.PAMContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BuildingBlock BuildingBlock { get; set; } = default!;

        [BindProperty]
        public List<UserInputs> UserInputs { get; set; } = new List<UserInputs>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buildingblock = await _context.BuildingBlocks
                .Include(bb => bb.UserInputs) // Include UserInputs
                .FirstOrDefaultAsync(m => m.BlockID == id);
            if (buildingblock == null)
            {
                return NotFound();
            }

            BuildingBlock = buildingblock;

            // Load associated UserInputs for the current BuildingBlock
            UserInputs = await _context.UserInputs
                .Where(ui => ui.BlockID == BuildingBlock.BlockID)
                .ToListAsync();

            // Populate the drop-downs with templates and block types
            ViewData["TemplateID"] = new SelectList(_context.Templates, "TemplateID", "TemplateName", BuildingBlock.TemplateID);
            ViewData["TypeID"] = new SelectList(_context.TypeBlocks, "TypeID", "TypeName", BuildingBlock.TypeID);

            // Set the NoRows based on selected TypeID
            var typeBlock = await _context.TypeBlocks
                .FirstOrDefaultAsync(t => t.TypeID == BuildingBlock.TypeID);
            ViewData["NoRows"] = typeBlock?.NoRows ?? 0;

            return Page();
        }



        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Update the BuildingBlock
            _context.BuildingBlocks.Update(BuildingBlock);
            await _context.SaveChangesAsync();

            // Remove existing UserInputs first, if required
            var existingUserInputs = await _context.UserInputs
                .Where(ui => ui.BlockID == BuildingBlock.BlockID)
                .ToListAsync();

            _context.UserInputs.RemoveRange(existingUserInputs);
            await _context.SaveChangesAsync();

            // Save new or updated UserInputs
            foreach (var input in UserInputs)
            {
                input.BlockID = BuildingBlock.BlockID; // Associate UserInput with BuildingBlock
                if (input.InputID == 0) // New input
                {
                    _context.UserInputs.Add(input);
                }
                else // Existing input, update it
                {
                    _context.UserInputs.Update(input);
                }
            }

            await _context.SaveChangesAsync();
            await _context.Database.ExecuteSqlRawAsync("EXEC dbo.UpdateUpdatedDate @TemplateID = {0}", BuildingBlock.TemplateID);

            return RedirectToPage("./Index", new { templateId = BuildingBlock.TemplateID });
        }


        // This is the API endpoint to return the NoRows for a selected TypeBlock
        public IActionResult OnGetNoRows(int id)
        {
            var typeBlock = _context.TypeBlocks.FirstOrDefault(t => t.TypeID == id);

            if (typeBlock == null)
            {
                // Log to check if TypeBlock is found
                Console.WriteLine($"TypeBlock with ID {id} not found.");
                return NotFound(); // Responds with a 404 if TypeBlock is not found
            }

            // Log to check if data is returned
            Console.WriteLine($"Found TypeBlock with NoRows: {typeBlock.NoRows}");

            return new JsonResult(new { noRows = typeBlock.NoRows ?? 0 });
        }
    }
}