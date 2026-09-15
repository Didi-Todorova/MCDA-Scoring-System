using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Criterion;
using Microsoft.AspNetCore.Mvc;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CriterionController : ControllerBase
    {
        private readonly ICriterionService _criterionService;

        public CriterionController(ICriterionService criterionService)
        {
            _criterionService = criterionService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateCriterionDto dto)
        {
            try
            {
                var criterion =
                    await _criterionService.CreateCriterionAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = criterion.Id },
                    criterion);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var criterion =
                await _criterionService.GetCriterionByIdAsync(id);

            if (criterion == null)
            {
                return NotFound(new
                {
                    Error = $"Criterion with ID {id} not found."
                });
            }

            return Ok(criterion);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var criteria =
                await _criterionService.GetAllCriteriaAsync();

            return Ok(criteria);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateCriterionDto dto)
        {
            try
            {
                await _criterionService
                    .UpdateCriterionAsync(id, dto);

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
        public async Task<IActionResult> Patch(
            int id,
            [FromBody] PatchCriterionDto dto)
        {
            try
            {
                await _criterionService
                    .PatchCriterionAsync(id, dto);

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
        public async Task<IActionResult> Delete(int id)
        {
            var result =
                await _criterionService.DeleteCriterionAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    Error = $"Criterion with ID {id} not found."
                });
            }

            return NoContent();
        }
    }
}