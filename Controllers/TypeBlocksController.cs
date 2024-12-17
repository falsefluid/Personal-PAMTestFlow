
using Microsoft.AspNetCore.Mvc;
using PAMTestFlow;
using System.Linq;

namespace PAMTestFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeBlocksController : ControllerBase
    {
        private readonly PAMContext _context;

        public TypeBlocksController(PAMContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/norows")]
        public IActionResult GetNoRows(int id)
        {
            var typeBlock = _context.TypeBlocks.FirstOrDefault(t => t.TypeID == id);
            if (typeBlock == null)
            {
                return NotFound();
            }

            return Ok(new { noRows = typeBlock.NoRows ?? 0 });
        }
    }
}