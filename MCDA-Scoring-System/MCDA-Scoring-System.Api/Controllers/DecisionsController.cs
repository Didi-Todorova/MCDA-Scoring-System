using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Criterion.Weights;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Decision;
using Microsoft.AspNetCore.Mvc;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DecisionsController : ControllerBase
    {
        private readonly IDecisionService _decisionService;
        private readonly IPercentageAllocationService
            _percentageAllocationService;
        private readonly IDirectRankingService _directRankingService;
        private readonly IScoringService _scoringService;

        public DecisionsController(
            IDecisionService decisionService,
            IPercentageAllocationService percentageAllocationService,
            IDirectRankingService directRankingService,
            IScoringService scoringService)
        {
            _decisionService = decisionService;
            _percentageAllocationService =
                percentageAllocationService;
            _directRankingService = directRankingService;
            _scoringService = scoringService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
    [FromBody] CreateDecisionDto dto)
        {
            try
            {
                var decision =
                    await _decisionService.CreateDecisionAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = decision.Id },
                    decision);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var decisions =
                await _decisionService.GetAllDecisionsAsync();

            return Ok(decisions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var decision =
                await _decisionService.GetDecisionByIdAsync(id);

            if (decision == null)
            {
                return NotFound(new
                {
                    Error = $"Decision with ID {id} not found."
                });
            }

            return Ok(decision);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateDecisionDto dto)
        {
            try
            {
                await _decisionService
                    .UpdateDecisionAsync(id, dto);

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
        public async Task<IActionResult> PartialUpdate(
            int id,
            [FromBody] PatchDecisionDto dto)
        {
            try
            {
                await _decisionService
                    .PartialUpdateDecisionAsync(id, dto);

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
            try
            {
                var result =
                    await _decisionService.DeleteDecisionAsync(id);

                if (!result)
                {
                    return NotFound(new
                    {
                        Error = $"Decision with ID {id} not found."
                    });
                }

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Error = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Error = ex.Message
                });
            }
        }

        [HttpPut("{id}/weight")]
        public async Task<IActionResult> SetWeights(
            int id,
            [FromBody] List<CriterionPercentageAllocationDto> weights)
        {
            try
            {
                await _percentageAllocationService
                    .SetWeightsAsync(id, weights);

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

        [HttpPut("{id}/ranking")]
        public async Task<IActionResult> SetRanking(
            int id,
            [FromBody] List<CriterionDirectRankingDto> rankings)
        {
            try
            {
                await _directRankingService
                    .SetWeightsAsync(id, rankings);

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

        [HttpGet("{id}/score")]
        public async Task<IActionResult> GetScore(int id)
        {
            try
            {
                var score =
                    await _scoringService.CalculateScoreAsync(id);

                return Ok(score);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}