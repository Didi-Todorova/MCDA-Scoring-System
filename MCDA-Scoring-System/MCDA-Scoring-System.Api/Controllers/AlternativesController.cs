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
        public async Task<IActionResult> CreateAlternative(
            [FromBody] CreateAlternativeDto dto)
        {
            try
            {
                var createdAlternative =
                    await _alternativeService.CreateAlternativeAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdAlternative.Id },
                    createdAlternative);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var alternative =
                await _alternativeService.GetAlternativeByIdAsync(id);

            if (alternative == null)
            {
                return NotFound(new
                {
                    Error = $"Alternative with ID {id} not found."
                });
            }

            return Ok(alternative);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var alternatives =
                await _alternativeService.GetAllAlternativesAsync();

            return Ok(alternatives);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlternative(
            int id,
            [FromBody] UpdateAlternativeDto dto)
        {
            try
            {
                await _alternativeService
                    .UpdateAlternativeAsync(id, dto);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAlternative(
            int id,
            [FromBody] PatchAlternativeDto dto)
        {
            try
            {
                await _alternativeService
                    .PatchAlternativeAsync(id, dto);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlternative(int id)
        {
            var result =
                await _alternativeService.DeleteAlternativeAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    Error = $"Alternative with ID {id} not found."
                });
            }

            return NoContent();
        }
    }
}