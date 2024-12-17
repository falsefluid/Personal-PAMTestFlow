using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PAMTestFlow;
using Microsoft.EntityFrameworkCore;


namespace PAMTestFlow.Pages.BloxBuilder
{
    public class CreateModel : PageModel
    {
        private readonly PAMTestFlow.PAMContext _context;

        public CreateModel(PAMTestFlow.PAMContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BuildingBlock BuildingBlock { get; set; } = default!;

        [BindProperty]
        public List<UserInputs> UserInputs { get; set; } = new List<UserInputs>();

        public IActionResult OnGet()
        {
            // Initialize BuildingBlock if null
            BuildingBlock ??= new BuildingBlock();

            // Populate the drop-downs with templates and block types
            ViewData["TemplateID"] = new SelectList(_context.Templates, "TemplateID", "TemplateName");
            ViewData["TypeID"] = new SelectList(_context.TypeBlocks, "TypeID", "TypeName");

            // Get the NoRows value from TypeBlock based on selected TypeID
            if (BuildingBlock.TypeID != 0)
            {
                var typeBlock = _context.TypeBlocks.FirstOrDefault(t => t.TypeID == BuildingBlock.TypeID);
                if (typeBlock != null)
                {
                    ViewData["NoRows"] = typeBlock.NoRows ?? 0;
                }
                else
                {
                    ViewData["NoRows"] = 0; // Default value if TypeBlock is not found
                }
            }
            else
            {
                ViewData["NoRows"] = 0; // Default value if no TypeID is selected
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Repopulate the drop-downs with templates and block types
                ViewData["TemplateID"] = new SelectList(_context.Templates, "TemplateID", "TemplateName");
                ViewData["TypeID"] = new SelectList(_context.TypeBlocks, "TypeID", "TypeName");

                // Get the NoRows value from TypeBlock based on selected TypeID
                if (BuildingBlock.TypeID != 0)
                {
                    var typeBlock = _context.TypeBlocks.FirstOrDefault(t => t.TypeID == BuildingBlock.TypeID);
                    if (typeBlock != null)
                    {
                        ViewData["NoRows"] = typeBlock.NoRows ?? 0;
                    }
                    else
                    {
                        ViewData["NoRows"] = 0; // Default value if TypeBlock is not found
                    }
                }
                else
                {
                    ViewData["NoRows"] = 0; // Default value if no TypeID is selected
                }

                return Page();
            }

            // Initialize UserInputs if null
            UserInputs ??= new List<UserInputs>();

            // Set the SequenceOrder to the last number in the sequence for the chosen template
            var lastSequenceOrder = _context.BuildingBlocks
                .Where(bb => bb.TemplateID == BuildingBlock.TemplateID)
                .OrderByDescending(bb => bb.SequenceOrder)
                .Select(bb => bb.SequenceOrder)
                .FirstOrDefault();

            BuildingBlock.SequenceOrder = lastSequenceOrder + 1;

            // Save the BuildingBlock
            _context.BuildingBlocks.Add(BuildingBlock);
            await _context.SaveChangesAsync();

            // Save UserInputs associated with this BuildingBlock
            foreach (var input in UserInputs)
            {
                input.BlockID = BuildingBlock.BlockID; // Associate with the BuildingBlock
                _context.UserInputs.Add(input);
            }

            await _context.SaveChangesAsync();
            await _context.Database.ExecuteSqlRawAsync("EXEC dbo.UpdateUpdatedDate @TemplateID = {0}", BuildingBlock.TemplateID);

            return RedirectToPage("./Index", new { templateId = BuildingBlock.TemplateID });
        }

        // This is the API endpoint to return the NoRows for a selected TypeBlock
        // Returning JSON using JsonResult instead of Ok()
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