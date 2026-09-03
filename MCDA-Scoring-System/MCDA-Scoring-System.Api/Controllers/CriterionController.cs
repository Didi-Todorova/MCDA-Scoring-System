using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Criterion;
using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult<CriterionDto>> Create([FromBody] CreateCriterionDto dto)
        {
            var criterion = await _criterionService.CreateCriterionAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = criterion.Id }, criterion);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CriterionDto>> GetById(int id)
        {
            var criterion = await _criterionService.GetCriterionByIdAsync(id);
            return criterion is not null ? Ok(criterion) : NotFound(new { Error = $"Criterion with ID {id} not found" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var criteria = await _criterionService.GetAllCriteriaAsync();
            return criteria is not null ? Ok(criteria) : NotFound(new { Error = "No criteria found" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCriterionDto dto)
        {
            await _criterionService.UpdateCriterionAsync(id, dto);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] PatchCriterionDto dto)
        {
            await _criterionService.PatchCriterionAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _criterionService.DeleteCriterionAsync(id);
            return result ? NoContent() : NotFound(new { Error = $"Criterion with ID {id} not found" });
        }
    }
}
