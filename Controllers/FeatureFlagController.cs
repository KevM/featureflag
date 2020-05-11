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

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] FeatureFlag input)
        {
            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            var flagById= await _context.FeatureFlags.FindAsync(input.FeatureFlagId);
            if (flagById != null) return new ConflictObjectResult($"Feature flag with id {input.FeatureFlagId} already exists.");

            var flagByName = await _context.FeatureFlags.FirstOrDefaultAsync(x => x.Name == input.Name);
            if (flagByName != null) return new ConflictObjectResult($"Feature flag with name {input.Name} already exists.");

            await _context.FeatureFlags.AddAsync(input);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = input.FeatureFlagId }, input);
        }
    }
}
