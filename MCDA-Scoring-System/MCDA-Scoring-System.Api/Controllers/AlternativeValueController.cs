using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.AlternativeValue;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlternativeValueController : ControllerBase
    {
        private readonly IAlternativeValueService _alternativeValueService;

        public AlternativeValueController(IAlternativeValueService alternativeValueService)
        {
            _alternativeValueService = alternativeValueService;
        }

        [HttpPost]
        public async Task<ActionResult<AlternativeValueDto>> Create([FromBody] CreateAlternativeValueDto dto)
        {
            var alternativeValue = await _alternativeValueService.CreateAlternativeValueAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = alternativeValue.Id }, alternativeValue);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AlternativeValueDto>> GetById(int id)
        {
            var alternativeValue = await _alternativeValueService.GetAlternativeValueByIdAsync(id);
            return alternativeValue is not null ? Ok(alternativeValue) : NotFound(new { Error = $"Alternative value with ID {id} not found" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var alternativeValues = await _alternativeValueService.GetAllAlternativeValuesAsync();
            return alternativeValues is not null ? Ok(alternativeValues) : NotFound(new { Error = "No alternative values found" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAlternativeValueDto dto)
        {
            await _alternativeValueService.UpdateAlternativeValueAsync(id, dto);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] PatchAlternativeValueDto dto)
        {
            await _alternativeValueService.PatchAlternativeValueAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _alternativeValueService.DeleteAlternativeValueAsync(id);
            return result ? NoContent() : NotFound(new { Error = $"Alternative value with ID {id} not found" });
        }
    }
}
