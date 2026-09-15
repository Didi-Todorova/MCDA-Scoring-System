using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.CriterionNumericalRule;
using Microsoft.AspNetCore.Mvc;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CriterionNumericalRuleController : ControllerBase
    {
        private readonly ICriterionNumericalRuleService
            _numericalCriterionRuleService;

        public CriterionNumericalRuleController(
            ICriterionNumericalRuleService numericalCriterionRuleService)
        {
            _numericalCriterionRuleService =
                numericalCriterionRuleService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateCriterionNumericalRuleDto dto)
        {
            try
            {
                var rule =
                    await _numericalCriterionRuleService
                        .CreateCriterionNumericalRuleAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = rule.Id },
                    rule);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rule =
                await _numericalCriterionRuleService
                    .GetCriterionNumericalRuleByIdAsync(id);

            if (rule == null)
            {
                return NotFound(new
                {
                    Error =
                        $"Criterion Numerical Rule with ID {id} not found."
                });
            }

            return Ok(rule);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rules =
                await _numericalCriterionRuleService
                    .GetAllCriterionNumericalRulesAsync();

            return Ok(rules);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateCriterionNumericalRuleDto dto)
        {
            try
            {
                await _numericalCriterionRuleService
                    .UpdateCriterionNumericalRuleAsync(id, dto);

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
            [FromBody] PatchCriterionNumericalDto dto)
        {
            try
            {
                await _numericalCriterionRuleService
                    .PatchCriterionNumericalRuleAsync(id, dto);

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
                await _numericalCriterionRuleService
                    .DeleteCriterionNumericalRuleAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    Error =
                        $"Criterion Numerical Rule with ID {id} not found."
                });
            }

            return NoContent();
        }
    }
}