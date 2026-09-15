using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.CriterionOption;
using Microsoft.AspNetCore.Mvc;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CriterionOptionController : ControllerBase
    {
        private readonly ICriterionOptionService _criterionOptionService;

        public CriterionOptionController(
            ICriterionOptionService criterionOptionService)
        {
            _criterionOptionService = criterionOptionService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateCriterionOptionsDto dto)
        {
            try
            {
                var criterionOptions =
                    await _criterionOptionService
                        .CreateCriterionOptionAsync(dto);

                return Ok(criterionOptions);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var criterionOption =
                await _criterionOptionService
                    .GetCriterionOptionByIdAsync(id);

            if (criterionOption == null)
            {
                return NotFound(new
                {
                    Error =
                        $"Criterion option with ID {id} not found."
                });
            }

            return Ok(criterionOption);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var criterionOptions =
                await _criterionOptionService
                    .GetAllCriterionOptionsAsync();

            return Ok(criterionOptions);
        }

        [HttpPut("criterion/{criterionId}")]
        public async Task<IActionResult> Update(
            int criterionId,
            [FromBody] UpdateCriterionOptionsDto dto)
        {
            try
            {
                await _criterionOptionService
                    .UpdateCriterionOptionAsync(
                        criterionId,
                        dto);

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
            [FromBody] PatchCriterionOptionDto dto)
        {
            try
            {
                await _criterionOptionService
                    .PatchCriterionOptionAsync(id, dto);

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
                await _criterionOptionService
                    .DeleteCriterionOptionAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    Error =
                        $"Criterion option with ID {id} not found."
                });
            }

            return NoContent();
        }
    }
}