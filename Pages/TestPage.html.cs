using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using PAMTestFlow;
 
 namespace PAMTestFlow.Pages;
public class TestPageModel(IHubContext<ProgressHub> hubContext, IAntiforgery antiforgery) : PageModel
{
    private readonly IHubContext<ProgressHub> _hubContext = hubContext;
    private readonly IAntiforgery _antiforgery = antiforgery;
 
    public string? AntiForgeryToken { get; private set; }

    // GET method to render the page and generate the anti-forgery token
    public void OnGet()
    {
        AntiForgeryToken = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
        ViewData["AntiForgeryToken"] = AntiForgeryToken;
    }
 
    public async Task<IActionResult> OnPostRunTest()
    {
        Console.WriteLine("OnPostRunTest method called");
        // Run the Playwright test in the backend and notify frontend via SignalR
        await RunPlaywrightTest();
        return new JsonResult("Test started.");
    }
 
    // Method to run Playwright Test
    private async Task RunPlaywrightTest()
    {
        Console.WriteLine("RunPlaywrightTest method called");
        var playwright = await Playwright.CreateAsync();
        var iPhone = playwright.Devices["iPhone 11"];
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync(iPhone);
        var page = await browser.NewPageAsync();
 
        try
        {
            await page.GotoAsync("https://selfbill.aws.ohiosystems.co.uk/");
            await SendProgressUpdate("Navigated to the website");
            await Task.Delay(1000);
 
            await page.FillAsync("input[name='ctl00$ucNavbar$ucLogin$LoginUserRelease$UserName']", "admin@pam.co.uk");
            await page.FillAsync("input[name='ctl00$ucNavbar$ucLogin$LoginUserRelease$Password']", "Ohioadmin123");
            await SendProgressUpdate("Filled login form");
            await Task.Delay(1000);
 
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = "test_screenshot.png" });
            await SendProgressUpdate("Test completed and screenshot taken");
            await Task.Delay(1000);
 
            await browser.CloseAsync();
        }
        catch (Exception ex)
        {
            await SendProgressUpdate($"Error: {ex.Message}");
        }
    }
 
    // Helper method to send real-time progress updates to frontend via SignalR
    private async Task SendProgressUpdate(string message)
    {
        Console.WriteLine($"Sending progress update: {message}");
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
    }
}