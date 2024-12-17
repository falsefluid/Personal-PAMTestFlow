using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace PAMTestFlow.Pages;

public class KeyInfoModel : PageModel
{
    private readonly ILogger<KeyInfoModel> _logger;
    private readonly PAMContext _context;

    public KeyInfoModel(ILogger<KeyInfoModel> logger, PAMContext context)
    {
        _logger = logger;
        _context = context;
    }

    public List<TypeBlock> TypeBlocks { get; set; } = new();

    public async Task OnGetAsync()
    {
        TypeBlocks = await _context.TypeBlocks.ToListAsync();
    }
}

