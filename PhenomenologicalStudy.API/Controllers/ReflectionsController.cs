using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhenomenologicalStudy.API.Data;
using PhenomenologicalStudy.API.Models;

namespace PhenomenologicalStudy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Add header "Authorization": "Bearer {jwt}" where jwt from login.
    public class ReflectionsController : ControllerBase
    {
        private readonly PhenomenologicalStudyContext _context;

        public ReflectionsController(PhenomenologicalStudyContext context)
        {
            _context = context;
        }

        // GET: api/Reflections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reflection>>> GetReflections()
        {
            return await _context.Reflections.ToListAsync();
        }

        // GET: api/Reflections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reflection>> GetReflection(Guid id)
        {
            var reflection = await _context.Reflections.FindAsync(id);

            if (reflection == null)
            {
                return NotFound();
            }

            return reflection;
        }

        // PUT: api/Reflections/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReflection(Guid id, Reflection reflection)
        {
            if (id != reflection.Id)
            {
                return BadRequest();
            }

            _context.Entry(reflection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReflectionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Reflections
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Reflection>> PostReflection(Reflection reflection)
        {
            _context.Reflections.Add(reflection);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReflection", new { id = reflection.Id }, reflection);
        }

        // DELETE: api/Reflections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReflection(Guid id)
        {
            var reflection = await _context.Reflections.FindAsync(id);
            if (reflection == null)
            {
                return NotFound();
            }

            _context.Reflections.Remove(reflection);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReflectionExists(Guid id)
        {
            return _context.Reflections.Any(e => e.Id == id);
        }
    }
}
