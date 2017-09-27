using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AspNetGizmos.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AspNetGizmos.Data;

namespace AspNetGizmos.Controllers
{
  [Route("api/[controller]")]
  public class GizmosController : Controller
  {
    private readonly GizmoContext _context;

    public GizmosController(GizmoContext context)
    {
      _context = context;

      if (!_context.Gizmos.Any())
      {
        _context.AddRange(GizmosData.Get());
        _context.SaveChanges();
      }
    }

    // GET api/Gizmos/
    [HttpGet("")]
    public async Task<IEnumerable<Gizmo>> GetAll()
    {
      var ranked = _context.Gizmos.OrderBy(x => x.Rank);

      return await ranked.ToListAsync();
    }
    
    // GET api/Gizmos/5
    [HttpGet("{id}", Name = "GetGizmo")]
    public IActionResult GetById(long id)
    {
      var gizmo = _context.Gizmos.FirstOrDefault(t => t.Id == id);
      if (gizmo == null)
      {
        return NotFound();
      }
      return new ObjectResult(gizmo);
    }

    // POST api/Gizmos
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Gizmo gizmo)
    {
      if (gizmo == null)
      {
        return BadRequest();
      }

      _context.Gizmos.Add(gizmo);

      await _context.SaveChangesAsync();

      return CreatedAtRoute("GetGizmo", new { id = gizmo.Id }, gizmo);
    }

    // PUT api/Gizmos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] Gizmo gizmo)
    {
      if (gizmo == null || gizmo.Id != id)
      {
        return BadRequest();
      }

      var ctxResult = _context.Gizmos.FirstOrDefault(t => t.Id == id);
      if (ctxResult == null)
      {
        return NotFound();
      }

      ctxResult.Name = gizmo.Name;
      ctxResult.Rank = gizmo.Rank;

      _context.Gizmos.Update(ctxResult);

      await _context.SaveChangesAsync();

      return new NoContentResult();
    }

    // DELETE api/Gizmos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
      var gizmo = _context.Gizmos.FirstOrDefault(t => t.Id == id);

      if (gizmo == null)
      {
        return NotFound();
      }

      _context.Gizmos.Remove(gizmo);

      await _context.SaveChangesAsync();

      return new NoContentResult();
    }
  }
}
