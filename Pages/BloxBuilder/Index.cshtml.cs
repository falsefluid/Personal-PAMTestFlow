using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PAMTestFlow;
using Microsoft.AspNetCore.SignalR;
using PAMTestFlow.Services;

namespace PAMTestFlow.Pages.BloxBuilder
{
    public class IndexModel : PageModel
    {
        private readonly PAMTestFlow.PAMContext _context;
        private readonly IHubContext<ProgressHub> _hubContext;

        public IndexModel(PAMTestFlow.PAMContext context, IHubContext<ProgressHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public IList<BuildingBlock> BuildingBlock { get; set; } = new List<BuildingBlock>();
        public List<Template> Templates { get; set; } = new List<Template>();
        public int SelectedTemplateId { get; set; }
        public int WaitInterval { get; set; }
        public string? WebsiteURL { get; set; }
        public string EmulationDevice { get; set; }

        public class BlockOrder
        {
            public int BlockId { get; set; }
            public int SequenceOrder { get; set; }
        }

        public async Task OnGetAsync(int? templateId)
        {
            Templates = await _context.Templates.ToListAsync();

            if (templateId.HasValue)
            {
                var template = await _context.Templates.FindAsync(templateId.Value);
            if (template != null)
            {
                WebsiteURL = template.WebsiteURL;
            }
                await OnPostFilterBlocksAsync(templateId.Value);
            }
        }

        public async Task<IActionResult> OnPostFilterBlocksAsync(int templateId)
        {
            SelectedTemplateId = templateId;
            Templates = await _context.Templates.ToListAsync();
            
            var template = await _context.Templates.FindAsync(templateId);
            if (template != null)
            {
                WebsiteURL = template.WebsiteURL;
            }
            
            BuildingBlock = await _context.BuildingBlocks
                .Include(b => b.Template)
                .Include(b => b.TypeBlock)
                .Include(b => b.UserInputs)
                .Where(b => b.TemplateID == templateId)
                .OrderBy(b => b.SequenceOrder)
                .ToListAsync();
                

            return Page();
        }

        public async Task<IActionResult> OnPostTestAsync(int templateId, string websiteUrl, string emulationDevice)
    {
        if (templateId == 0)
        {
            ModelState.AddModelError(string.Empty, "Please select a template.");
            Templates = await _context.Templates.ToListAsync();
            return Page();
        }
        SelectedTemplateId = templateId;
        EmulationDevice = emulationDevice;

        var template = await _context.Templates.FindAsync(templateId);
        if (template != null)
        {
            template.WebsiteURL = websiteUrl;
            await _context.SaveChangesAsync();
        }

        await SetAllStatusesAsync(templateId, 0);

        BuildingBlock = await _context.BuildingBlocks
            .Include(b => b.Template)
            .Include(b => b.TypeBlock)
            .Include(b => b.UserInputs)
            .Where(b => b.TemplateID == templateId)
            .OrderBy(b => b.SequenceOrder)
            .ToListAsync();

        var playwrightBlox = new PlaywrightBlox(_context, this);

            try
            {
                Console.WriteLine($"Redirecting to URL: {websiteUrl}");
                await playwrightBlox.OpenWebsite(websiteUrl, emulationDevice);
                foreach (var block in BuildingBlock)
                {
                    var waitInterval = block.WaitInterval;

                    switch (block.TypeBlock?.TypeName)
                    {
                        case "Click":
                            if (block.UserInputs != null && block.UserInputs.Any())
                            {
                                var userInput = block.UserInputs.First().UserInput;
                                if (!string.IsNullOrEmpty(userInput))
                                {
                                    await playwrightBlox.Click(userInput, waitInterval, block.BlockID);
                                }
                            }
                            break;
                        case "Type Text":
                            if (block.UserInputs != null && block.UserInputs.Count > 1)
                            {
                                var firstInput = block.UserInputs.First().UserInput;
                                var secondInput = block.UserInputs.ElementAt(1).UserInput;
                                if (!string.IsNullOrEmpty(firstInput) && !string.IsNullOrEmpty(secondInput))
                                {
                                    await playwrightBlox.TypeText(firstInput, secondInput, waitInterval, block.BlockID);
                                }
                            }
                            break;
                        case "Press Key":
                            if (block.UserInputs != null && block.UserInputs.Any() && !string.IsNullOrEmpty(block.UserInputs.First().UserInput))
                            {
                                var key = block.UserInputs.First().UserInput;
                                if (!string.IsNullOrEmpty(key))
                                {
                                    await playwrightBlox.PressKey(key, waitInterval, block.BlockID);
                                }
                            }
                            break;
                        case "Hover":
                            if (block.UserInputs != null && block.UserInputs.Any() && !string.IsNullOrEmpty(block.UserInputs.First().UserInput))
                            {
                                var hoverInput = block.UserInputs.First().UserInput;
                                if (!string.IsNullOrEmpty(hoverInput))
                                {
                                    await playwrightBlox.Hover(hoverInput, waitInterval, block.BlockID);
                                }
                            }
                            break;
                        case "Navigate to URL":
                            if (block.UserInputs != null && block.UserInputs.Any())
                            {
                                var url = block.UserInputs.First().UserInput;
                                if (!string.IsNullOrEmpty(url))
                                {
                                    await playwrightBlox.NavigateToURL(url, waitInterval, block.BlockID);
                                }
                            }
                            break;
                        case "Check Text Content":
                            if (block.UserInputs != null && block.UserInputs.Count > 1)
                            {
                                if (block.UserInputs != null && block.UserInputs.Count > 1)
                                {
                                    var firstInput = block.UserInputs.First().UserInput;
                                    var secondInput = block.UserInputs.ElementAt(1).UserInput;
                                    if (!string.IsNullOrEmpty(firstInput) && !string.IsNullOrEmpty(secondInput))
                                    {
                                        await playwrightBlox.CheckTextContent(firstInput, secondInput, waitInterval, block.BlockID);
                                    }
                                }
                            }
                            break;
                        case "Wait for Selector":
                            if (block.UserInputs != null && block.UserInputs.Any())
                            {
                                var selector = block.UserInputs.First().UserInput;
                                if (!string.IsNullOrEmpty(selector))
                                {
                                    await playwrightBlox.WaitForSelector(selector, waitInterval, block.BlockID);
                                }
                            }
                            break;
                        case "Check Checkbox":
                            if (block.UserInputs != null && block.UserInputs.Any())
                            {
                                var checkboxInput = block.UserInputs.First().UserInput;
                                if (!string.IsNullOrEmpty(checkboxInput))
                                {
                                    await playwrightBlox.Checkbox(checkboxInput, waitInterval, block.BlockID);
                                }
                            }
                            break;
                        case "Select Dropdown Option":
                            if (block.UserInputs != null && block.UserInputs.Count > 1)
                            {
                                var firstInput = block.UserInputs.First().UserInput;
                                var secondInput = block.UserInputs.ElementAt(1).UserInput;
                                if (!string.IsNullOrEmpty(firstInput) && !string.IsNullOrEmpty(secondInput))
                                {
                                    await playwrightBlox.DropdownOptions(firstInput, secondInput, waitInterval, block.BlockID);
                                }
                            }
                            break;
                        case "File Upload":
                            if (block.UserInputs != null && block.UserInputs.Count > 1)
                            {
                                var firstInput = block.UserInputs.First().UserInput;
                                var secondInput = block.UserInputs.ElementAt(1).UserInput;
                                if (!string.IsNullOrEmpty(firstInput) && !string.IsNullOrEmpty(secondInput))
                                {
                                    await playwrightBlox.FileUpload(firstInput, secondInput, waitInterval, block.BlockID);
                                }
                            }
                            break;
                        case "Screenshot":
                            if (block.UserInputs != null && block.UserInputs.Any())
                            {
                                var screenshotName = block.UserInputs.First().UserInput;
                                if (!string.IsNullOrEmpty(screenshotName))
                                {
                                    await playwrightBlox.Screenshot(screenshotName, waitInterval, block.BlockID);
                                }
                            }
                            break;
                        case "Generate PDF":
                            if (block.UserInputs != null && block.UserInputs.Any())
                            {
                                var pdfName = block.UserInputs.First().UserInput;
                                if (!string.IsNullOrEmpty(pdfName))
                                {
                                    await playwrightBlox.GeneratePDF(pdfName, waitInterval, block.BlockID);
                                }
                            }
                            break;
                        default:
                            Console.WriteLine($"Unknown TypeBlock: {block.TypeBlock?.TypeName}");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during test execution: {ex.Message}");
            }
            finally
            {
                // Close the browser after the foreach loop
                await Task.Delay(10000);
                await playwrightBlox.CloseBrowser();
            }

            return RedirectToPage("/BloxBuilder/Index", new { templateId = SelectedTemplateId });
        }

        public async Task<IActionResult> OnPostUpdateSequenceOrderAsync([FromBody] List<BlockOrder> blockOrder)
        {
            if (blockOrder == null || !blockOrder.Any())
            {
                return BadRequest("Invalid block order data.");
            }

            foreach (var order in blockOrder)
            {
                var block = await _context.BuildingBlocks.FindAsync(order.BlockId);
                if (block != null)
                {
                    block.SequenceOrder = order.SequenceOrder;
                }
            }

            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnPostSaveWebsiteUrlAsync([FromBody] SaveWebsiteUrlRequest request)
        {
            if (request.TemplateId == 0)
            {
                return BadRequest("Invalid template ID.");
            }

            var template = await _context.Templates.FindAsync(request.TemplateId);
            if (template == null)
            {
                return NotFound("Template not found.");
            }

            template.WebsiteURL = request.WebsiteUrl;
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

        private async Task SetAllStatusesAsync(int templateId, int status)
        {
            var blocks = await _context.BuildingBlocks
                .Where(b => b.TemplateID == templateId)
                .ToListAsync();

            foreach (var block in blocks)
            {
                block.BlockStatus = status;
                await _hubContext.Clients.All.SendAsync("ReceiveStatusUpdate", block.BlockID, status);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateBlockStatusAsync(int blockId, int status)
        {
            var block = await _context.BuildingBlocks.FindAsync(blockId);
            if (block != null)
            {
                block.BlockStatus = status;
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveStatusUpdate", blockId, status);
            }
        }

        public class SaveWebsiteUrlRequest
        {
            public int TemplateId { get; set; }
            public string WebsiteUrl { get; set; }
        }
    }
}