using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.IntervalRange;
using Microsoft.AspNetCore.Mvc;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntervalRangeController : ControllerBase
    {
        private readonly IIntervalRangeService _intervalRangeService;

        public IntervalRangeController(
            IIntervalRangeService intervalRangeService)
        {
            _intervalRangeService = intervalRangeService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateIntervalRangesDto dto)
        {
            try
            {
                var ranges =
                    await _intervalRangeService
                        .CreateIntervalRangesAsync(dto);

                return Ok(ranges);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var range =
                await _intervalRangeService
                    .GetIntervalRangeByIdAsync(id);

            if (range == null)
            {
                return NotFound(new
                {
                    Error = $"Interval Range with ID {id} not found."
                });
            }

            return Ok(range);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ranges =
                await _intervalRangeService
                    .GetAllIntervalRangesAsync();

            return Ok(ranges);
        }

        [HttpGet("rule/{criterionNumericalRuleId}")]
        public async Task<IActionResult> GetByRuleId(
            int criterionNumericalRuleId)
        {
            var ranges =
                await _intervalRangeService
                    .GetIntervalRangesByRuleIdAsync(
                        criterionNumericalRuleId);

            return Ok(ranges);
        }

        [HttpPut("rule/{criterionNumericalRuleId}")]
        public async Task<IActionResult> Update(
            int criterionNumericalRuleId,
            [FromBody] UpdateIntervalRangesDto dto)
        {
            try
            {
                await _intervalRangeService
                    .UpdateIntervalRangesAsync(
                        criterionNumericalRuleId,
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result =
                await _intervalRangeService
                    .DeleteIntervalRangeAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    Error = $"Interval Range with ID {id} not found."
                });
            }

            return NoContent();
        }
    }
}