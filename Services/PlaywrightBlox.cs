using System;
using System.Threading.Tasks;
using Azure;
using Microsoft.Playwright;
using PAMTestFlow.Pages.BloxBuilder;

namespace PAMTestFlow.Services
{
    public class PlaywrightBlox
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IPage? _page;
        private readonly PAMContext _context;
        private readonly IndexModel _indexModel;

        public PlaywrightBlox(PAMContext context, IndexModel indexModel)
        {
            _context = context;
            _indexModel = indexModel;
            InitializePlaywright().Wait();
        }

        private async Task InitializePlaywright()
        {
            _playwright = await Playwright.CreateAsync();
        }

        public async Task OpenWebsite(string url, string device)
        {
            try
            {
                if (_playwright == null)
                {
                    await InitializePlaywright();
                }
                // Launch the browser
                if (_playwright != null)
                {
                    _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false }); // Set Headless to false to see the browser
                }
                else
                {
                    throw new InvalidOperationException("Playwright is not initialized.");
                }
                Console.WriteLine($"Emulating device: {device}");
                if (device != "Desktop")
                {
                    var emulation =_playwright.Devices[device];
                    var context = await _browser.NewContextAsync(new BrowserNewContextOptions 
                    { ViewportSize = emulation.ViewportSize, 
                    UserAgent = emulation.UserAgent,
                    IsMobile = true,});
                    _page = await context.NewPageAsync();

                }
                else 
                {
                    _page = await _browser.NewPageAsync();
                }
                // Navigate to the URL
                await _page.GotoAsync(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening website: {ex.Message}");
            }
        }
        public async Task Click(string selector, int waitinterval, int blockId)
        {
            try
            {
                if (_page != null)
                {
                    await _page.ClickAsync(selector);
                    if (waitinterval > 0)
                    {
                        await Task.Delay(waitinterval * 1000);
                    }
                    await _indexModel.UpdateBlockStatusAsync(blockId, 1); // Success
                }
                else
                {
                    Console.WriteLine("Error: Page is not initialised.");
                    await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clicking element: {ex.Message}");
                await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
            }
        }
        public async Task TypeText (string selector, string text, int waitinterval, int blockId)
        {
            try
            {
                // Type text in an element identified by the selector
                if (_page != null)
                {
                    await _page.FillAsync(selector, text);
                    if (waitinterval > 0)
                    {
                        await Task.Delay(waitinterval*1000);
                    }
                    await _indexModel.UpdateBlockStatusAsync(blockId, 1); // Success
                }
                else
                {
                    Console.WriteLine("Error: Page is not initialised.");
                    await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error typing text: {ex.Message}");
                await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
            }
        }
        public async Task PressKey (string key, int waitinterval, int blockId)
        {
            try
            {
                // Press a key
                if (_page != null)
                {
                    await _page.Keyboard.PressAsync(key);
                    if (waitinterval > 0)
                    {
                        await Task.Delay(waitinterval*1000);
                    }
                    await _indexModel.UpdateBlockStatusAsync(blockId, 1); // Success
                }
                else
                {
                    Console.WriteLine("Error: Page is not initialised.");
                    await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error pressing key: {ex.Message}");
                await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
            }
        }
        public async Task Hover(string selector, int waitinterval, int blockId)
        {
            try
            {
                // Hover over an element identified by the selector
                if (_page != null)
                {
                    await _page.HoverAsync(selector);
                    if (waitinterval > 0)
                    {
                        await Task.Delay(waitinterval*1000);
                    }
                    await _indexModel.UpdateBlockStatusAsync(blockId, 1); // Success
                }
                else
                {
                    Console.WriteLine("Error: Page is not initialised.");
                    await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error hovering over element: {ex.Message}");
                await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
            }
        }
        public async Task NavigateToURL(string url, int waitinterval, int blockId)
        {
            try
            {
                // Navigate to a URL
                if (_page != null)
                {
                    await _page.GotoAsync(url);
                    if (waitinterval > 0)
                    {
                        await Task.Delay(waitinterval*1000);
                    }
                    await _indexModel.UpdateBlockStatusAsync(blockId, 1); // Success
                }
                else
                {
                    Console.WriteLine("Error: Page is not initialised.");
                    await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error navigating to URL: {ex.Message}");
                await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
            }
        }
        public async Task CheckTextContent(string selector, string text, int waitinterval, int blockId)
        {
            try
            {
                // Check if an element identified by the selector contains the text
                if (_page != null)
                {
                    var element = await _page.QuerySelectorAsync(selector);
                    if (element != null)
                    {
                        var content = await element.TextContentAsync();
                        if (content != null && content.Contains(text))
                        {
                            Console.WriteLine($"Text '{text}' found in element '{selector}'");
                            await _indexModel.UpdateBlockStatusAsync(blockId, 1); // Success
                        }
                        else
                        {
                            Console.WriteLine($"Text '{text}' not found in element '{selector}'");
                        }
                        if (waitinterval > 0)
                        {
                            await Task.Delay(waitinterval*1000);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Element '{selector}' not found.");
                        await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
                    }
                }
                else
                {
                    Console.WriteLine("Error: Page is not initialised.");
                    await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking text content: {ex.Message}");
                await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
            }
        }
        public async Task WaitForSelector(string selector, int waitinterval, int blockId)
        {
            try
            {
                // Wait for an element identified by the selector to appear in the page
                if (_page != null)
                {
                    await _page.WaitForSelectorAsync(selector);
                    if (waitinterval > 0)
                    {
                        await Task.Delay(waitinterval*1000);
                    }
                    await _indexModel.UpdateBlockStatusAsync(blockId, 1); // Success
                }
                else
                {
                    Console.WriteLine("Error: Page is not initialised.");
                    await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error waiting for selector: {ex.Message}");
                await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
            }
        }
        public async Task Checkbox(string selector, int waitinterval, int blockId)
        {
            try
            {
                // Check a checkbox element identified by the selector
                if (_page != null)
                {
                    await _page.CheckAsync(selector);
                    if (waitinterval > 0)
                    {
                        await Task.Delay(waitinterval*1000);
                    }
                    await _indexModel.UpdateBlockStatusAsync(blockId, 1); // Success
                }
                else
                {
                    Console.WriteLine("Error: Page is not initialised.");
                    await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking checkbox: {ex.Message}");
                await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
            }
        }
        public async Task DropdownOptions(string selector, string option, int waitinterval, int blockId)
        {
            try
            {
                // Select an option from a dropdown element identified by the selector
                if (_page != null)
                {
                    await _page.SelectOptionAsync(selector, option);
                    if (waitinterval > 0)
                    {
                        await Task.Delay(waitinterval*1000);
                    }
                    await _indexModel.UpdateBlockStatusAsync(blockId, 1); // Success
                }
                else
                {
                    Console.WriteLine("Error: Page is not initialised.");
                    await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting dropdown option: {ex.Message}");
                await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
            }
        }
        public async Task FileUpload(string selector, string filePath, int waitinterval, int blockId)
        {
            try
            {
                // Upload a file to an input element identified by the selector
                if (_page != null)
                {
                    await _page.SetInputFilesAsync(selector, filePath);
                    if (waitinterval > 0)
                    {
                        await Task.Delay(waitinterval*1000);
                    }
                    await _indexModel.UpdateBlockStatusAsync(blockId, 1); // Success
                }
                else
                {
                    Console.WriteLine("Error: Page is not initialised.");
                    await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
            }
        }

        public async Task Screenshot(string screenshotname, int waitinterval, int blockId)
        {
            try
            {
                // Take a screenshot of the page
                if (_page != null)
                {
                    await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotname });
                    if (waitinterval > 0)
                    {
                        await Task.Delay(waitinterval*1000);
                    }
                    await _indexModel.UpdateBlockStatusAsync(blockId, 1); // Success
                }
                else
                {
                    Console.WriteLine("Error: Page is not initialised.");
                    await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error taking screenshot: {ex.Message}");
                await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
            } 
        }
        public async Task GeneratePDF(string pdfname, int waitinterval, int blockId)
        {
            try
            {
                // Generate a PDF of the page
                if (_page != null)
                {
                    await _page.PdfAsync(new PagePdfOptions { Path = pdfname });
                    if (waitinterval > 0)
                    {
                        await Task.Delay(waitinterval*1000);
                    }
                    await _indexModel.UpdateBlockStatusAsync(blockId, 1); // Success
                }
                else
                {
                    Console.WriteLine("Error: Page is not initialised.");
                    await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating PDF: {ex.Message}");
                await _indexModel.UpdateBlockStatusAsync(blockId, 2); // Failure
            }
        }
        
        public async Task CloseBrowser()
        {
            if (_browser != null)
            {
                await _browser.CloseAsync();
            }
        }
    }
}