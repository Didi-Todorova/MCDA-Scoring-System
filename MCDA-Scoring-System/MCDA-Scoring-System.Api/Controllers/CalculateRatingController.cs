using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalculateRatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public CalculateRatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpGet("rating")]
        public async Task<IActionResult> CalculateRating(
            [FromQuery] int alternativeId,
            [FromQuery] int criterionId)
        {
            try
            {
                var rating = await _ratingService
                    .CalculateRatingAsync(
                        alternativeId,
                        criterionId);

                return Ok(rating);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { Error = ex.Message });
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