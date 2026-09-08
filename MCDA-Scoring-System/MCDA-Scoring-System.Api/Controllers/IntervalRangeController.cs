using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.IntervalRange;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntervalRangeController : ControllerBase
    {
        private readonly IIntervalRangeService _numericRangeService;

        public IntervalRangeController(IIntervalRangeService numericRangeService)
        {
            _numericRangeService = numericRangeService;
        }

        [HttpPost]
        public async Task<ActionResult<IntervalRangeDto>> Create([FromBody] CreateIntervalRangeDto dto)
        {
            var numericRange = await _numericRangeService.CreateIntervalRangeAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = numericRange.Id }, numericRange);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IntervalRangeDto>> GetById(int id)
        {
            var numericRange = await _numericRangeService.GetIntervalRangeByIdAsync(id);
            return numericRange == null ? NotFound(new { Error = $"Interval Range with ID {id} not found" }) : Ok(numericRange);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var numericRanges = await _numericRangeService.GetAllIntervalRangesAsync();
            return numericRanges == null ? NotFound(new { Error = "No Interval Ranges found" }) : Ok(numericRanges);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateIntervalRangeDto dto)
        {
            try
            {
                await _numericRangeService.UpdateIntervalRangeAsync(id, dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] PatchIntervalRangeDto dto)
        {
            try
            {
                await _numericRangeService.PatchIntervalRangeAsync(id, dto);
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
            var result = await _numericRangeService.DeleteIntervalRangeAsync(id);
            return result ? NoContent() : NotFound(new { Error = $"Interval Range with ID {id} not found" });
        }
    }
}
