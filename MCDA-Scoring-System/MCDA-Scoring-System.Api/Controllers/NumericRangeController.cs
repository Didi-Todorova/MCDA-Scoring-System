using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericRange;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NumericRangeController : ControllerBase
    {
        private readonly INumericRangeService _numericRangeService;

        public NumericRangeController(INumericRangeService numericRangeService)
        {
            _numericRangeService = numericRangeService;
        }

        [HttpPost]
        public async Task<ActionResult<IntervalRangeDto>> Create([FromBody] CreateIntervalRangeDto dto)
        {
            var numericRange = await _numericRangeService.CreateNumericRangeAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = numericRange.Id }, numericRange);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IntervalRangeDto>> GetById(int id)
        {
            var numericRange = await _numericRangeService.GetNumericRangeByIdAsync(id);
            return numericRange == null ? NotFound(new { Error = $"Numeric Range with ID {id} not found" }) : Ok(numericRange);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var numericRanges = await _numericRangeService.GetAllNumericRangesAsync();
            return numericRanges == null ? NotFound(new { Error = "No Numeric Ranges found" }) : Ok(numericRanges);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateIntervalRangeDto dto)
        {
            try
            {
                await _numericRangeService.UpdateNumericRangeAsync(id, dto);
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
                await _numericRangeService.PatchNumericRangeAsync(id, dto);
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
            var result = await _numericRangeService.DeleteNumericRangeAsync(id);
            return result ? NoContent() : NotFound(new { Error = $"Numeric Range with ID {id} not found" });
        }
    }
}
