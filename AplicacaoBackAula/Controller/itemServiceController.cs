using AplicacaoBackAula.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacaoBackAula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemServiceController : ControllerBase
    {
        private readonly BdAulaBackendContext _context;

        public ItemServiceController(BdAulaBackendContext context)
        {
            _context = context;
        }

        // GET: api/Categorium
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItens()
        {
            return await _context.Items.ToListAsync();
        }


        // GET: api/Categorium/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Iditem == id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }


        // POST: api/Categorium
        [HttpPost]
        public async Task<ActionResult<Item>> PostCategoria(Item item)
        {
            _context.Items.Add(item);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetItem),
                new { id = item.Iditem },
               item
            );
        }


        // PUT: api/Categorium/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(
            int id,
            Item item)
        {
            if (id != item.Iditem)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExiste(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }


        // DELETE: api/Categorium/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Iditem == id);

            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);

            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool ItemExiste(int id)
        {
            return _context.Items
                .Any(e => e.Iditem == id);
        }
    }
}

