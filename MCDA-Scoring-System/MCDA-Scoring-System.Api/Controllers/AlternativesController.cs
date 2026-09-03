using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Alternative;
using Microsoft.AspNetCore.Mvc;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlternativesController : ControllerBase
    {
        private readonly IAlternativeService _alternativeService;

        public AlternativesController(IAlternativeService alternativeService)
        {
            _alternativeService = alternativeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAlternative([FromBody] CreateAlternativeDto dto)
        {
            var createdAlternative = await _alternativeService.CreateAlternativeAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdAlternative.Id }, createdAlternative);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var alternative = await _alternativeService.GetAlternativeByIdAsync(id);
            return alternative is not null ? Ok(alternative) : NotFound(new { Error = $"Alternative with ID {id} not found" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var alternatives = await _alternativeService.GetAllAlternativesAsync();
            return Ok(alternatives);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateAlternative(int id, [FromBody] UpdateAlternativeDto dto)
        {
            await _alternativeService.UpdateAlternativeAsync(id, dto);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAlternative(int id, [FromBody] PatchAlternativeDto dto)
        {
            await _alternativeService.PatchAlternativeAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlternative(int id)
        {
            var result = await _alternativeService.DeleteAlternativeAsync(id);
            return result ? NoContent() : NotFound(new { Error = $"Alternative with ID {id} not found" });
        }
    }
}
