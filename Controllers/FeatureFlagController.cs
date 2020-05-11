using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace featureflags.Controllers
{
    [ApiController]
    [Route("/featureflag")]
    public class FeatureFlagController : ControllerBase
    {
        private readonly FeatureFlagContext _context;
        private readonly ILogger<FeatureFlagController> _logger;

        public FeatureFlagController(
            FeatureFlagContext context,
            ILogger<FeatureFlagController> logger
            )
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<FeatureFlag>> Get()
        {
            return await _context.FeatureFlags.ToListAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<FeatureFlag>> GetById(Guid id)
        {
            var flagById = await _context.FeatureFlags.FindAsync(id);
            if (flagById == null) return NotFound("Flag does not exist");

            return flagById;
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Post([FromBody] FeatureFlag input)
        {
            var flagById= await _context.FeatureFlags.FindAsync(input.FeatureFlagId);
            if (flagById != null) return new ConflictObjectResult($"Feature flag with id {input.FeatureFlagId} already exists.");

            var flagByName = await _context.FeatureFlags.FirstOrDefaultAsync(x => x.Name == input.Name);
            if (flagByName != null) return new ConflictObjectResult($"Feature flag with name {input.Name} already exists.");

            await _context.FeatureFlags.AddAsync(input);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = input.FeatureFlagId }, input);
        }

        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Put(Guid id, [FromBody] FeatureFlag input)
        {
            if (id != input.FeatureFlagId) return BadRequest("You canot change the Flag identifier");

            var flagById = await _context.FeatureFlags.FindAsync(id);
            if (flagById.Name != input.Name)
            {
                var flagByNewName = await _context.FeatureFlags.FirstOrDefaultAsync(x => x.Name == input.Name);
                if (flagByNewName != null) return new ConflictObjectResult($"Cannot rename Feature flag with to name which already exists.");
            }

            _context.Update(input);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
