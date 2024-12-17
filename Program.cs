using PAMTestFlow;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PAMContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
 
// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddSignalR();  // Register SignalR services

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
 
app.UseHttpsRedirection();
app.UseStaticFiles();
 
app.UseRouting();
 
app.UseAuthorization();
app.UseAuthentication(); // Ensure authentication is enabled
app.UseAntiforgery(); // Antiforgery Middleware

app.MapRazorPages();
app.MapHub<ProgressHub>("/progressHub"); // Map the SignalR hub

// API endpoint to get NoRows for a TypeBlock
app.MapGet("/api/typeblocks/{typeID}/norows", async (int typeID, PAMContext context) =>
{
    var typeBlock = await context.TypeBlocks.FindAsync(typeID);
    if (typeBlock == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new { noRows = typeBlock.NoRows });
});

app.Run();