using Microsoft.AspNetCore.Mvc;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Decision;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Criterion.Weights;

[Route("api/[controller]")]
[ApiController]
public class DecisionsController : ControllerBase
{
    private readonly IDecisionService _decisionService;
    private readonly IPercentageAllocationService _percentageAllocationService;
    private readonly IDirectRankingService _directRankingService;

    public DecisionsController(IDecisionService decisionService, IPercentageAllocationService percentageAllocationService, IDirectRankingService directRankingService)
    {
        _decisionService = decisionService;
        _percentageAllocationService = percentageAllocationService;
        _directRankingService = directRankingService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDecisionDto dto)
    {
        var decision = await _decisionService.CreateDecisionAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = decision.Id }, decision);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var decisions = await _decisionService.GetAllDecisionsAsync();
        return Ok(decisions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DecisionDto>> GetById(int id)
    {
        var decision = await _decisionService.GetDecisionByIdAsync(id);
        return decision is not null ? Ok(decision) : NotFound(new { Error = $"Decision with ID {id} not found" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateDecisionDto dto)
    {
        await _decisionService.UpdateDecisionAsync(id, dto);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PartialUpdate(int id, PatchDecisionDto dto)
    {
        await _decisionService.PartialUpdateDecisionAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _decisionService.DeleteDecisionAsync(id);

        if (!result)
            return NotFound(new { Error = $"Decision with ID {id} not found" });

        return NoContent();
    }

    [HttpPut("{id}/weight")]
    public async Task<IActionResult> SetWeights(int id, [FromBody] List<CriterionPercentageAllocationDto> weights)
    {
        await _percentageAllocationService.SetWeightsAsync(id, weights);
       
        return NoContent();
    }

    [HttpPut("{id}/ranking")]
    public async Task<IActionResult> SetWeights(int id, [FromBody] List<CriterionDirectRankingDto> rankings)
    {
        await _directRankingService.SetWeightsAsync(id, rankings);

        return NoContent();
    }
}

