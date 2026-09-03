using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.CriterionOption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CriterionOptionController : ControllerBase
    {
        private readonly ICriterionOptionService _criterionOptionService;

        public CriterionOptionController(ICriterionOptionService criterionOptionService)
        {
            _criterionOptionService = criterionOptionService;
        }

        [HttpPost]
        public async Task<ActionResult<CriterionOptionDto>> Create([FromBody] CreateCriterionOptionDto dto)
        {
            var criterionOption = await _criterionOptionService.CreateCriterionOptionAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = criterionOption.Id }, criterionOption);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CriterionOptionDto>> GetById(int id)
        {
            var criterionOption = await _criterionOptionService.GetCriterionOptionByIdAsync(id);
            return criterionOption is not null ? Ok(criterionOption) : NotFound(new { Error = $"Criterion option with ID {id} not found" });
        }

        [HttpGet]   
        public async Task<IActionResult> GetAll()
        {
            var criterionOptions = await _criterionOptionService.GetAllCriterionOptionsAsync();
            return criterionOptions is not null ? Ok(criterionOptions) : NotFound(new { Error = "No criterion options found" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCriterionOptionDto dto)
        {
            await _criterionOptionService.UpdateCriterionOptionAsync(id, dto);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] PatchCriterionOptionDto dto)
        {
            await _criterionOptionService.PatchCriterionOptionAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _criterionOptionService.DeleteCriterionOptionAsync(id);
            return result ? NoContent() : NotFound(new { Error = $"Criterion option with ID {id} not found" });
        }
    }
}
