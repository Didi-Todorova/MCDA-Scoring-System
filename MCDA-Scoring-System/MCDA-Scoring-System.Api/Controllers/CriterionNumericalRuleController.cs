using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericalCriterionRule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CriterionNumericalRuleController : ControllerBase
    {
        private readonly INumericalCriterionRuleService _numericalCriterionRuleService;

        public CriterionNumericalRuleController(INumericalCriterionRuleService numericalCriterionRuleService)
        {
            _numericalCriterionRuleService = numericalCriterionRuleService;
        }

        [HttpPost]
        public async Task<ActionResult<CriterionNumericalRuleDto>> Create([FromBody] CreateCriterionNumericalRuleDto dto)
        {
            var rule = await _numericalCriterionRuleService.CreateNumericalCriterionRuleAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = rule.Id }, rule);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CriterionNumericalRuleDto>> GetById(int id)
        {
            var rule = await _numericalCriterionRuleService.GetNumericalCriterionRuleByIdAsync(id);
            return rule == null ? NotFound(new { Error = $"Numerical Criterion Rule with ID {id} not found" }) : Ok(rule);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCriterionNumericalRuleDto dto)
        {
            try
            {
                await _numericalCriterionRuleService.UpdateNumericalCriterionRuleAsync(id, dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] PatchCriterionNumericalDto dto)
        {
            try
            {
                await _numericalCriterionRuleService.PatchNumericalCriterionRuleAsync(id, dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _numericalCriterionRuleService.DeleteNumericalCriterionRuleAsync(id);
            return result ? NoContent() : NotFound(new { Error = $"Numerical Criterion Rule with ID {id} not found" });
        }
    }
}
