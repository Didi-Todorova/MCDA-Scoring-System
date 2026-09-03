using Microsoft.AspNetCore.Mvc;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Decision;

[Route("api/[controller]")]
[ApiController]
public class DecisionsController : ControllerBase
{
    private readonly IDecisionService _decisionService;

    public DecisionsController(IDecisionService decisionService)
    {
        _decisionService = decisionService;
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

        if(!result)
            return NotFound(new { Error = $"Decision with ID {id} not found" });

        return NoContent();
    }
}
