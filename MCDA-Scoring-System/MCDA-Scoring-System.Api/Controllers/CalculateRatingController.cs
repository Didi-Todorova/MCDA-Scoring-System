using Microsoft.AspNetCore.Mvc;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class NumericalValuesController : ControllerBase
    {
        private readonly IRatingService _numericalValuesService;

        public NumericalValuesController(
            IRatingService numericalValuesService)
        {
            _numericalValuesService = numericalValuesService;
        }

        [HttpGet("rating")]
        public async Task<IActionResult> CalculateRating(
            [FromQuery] int alternativeId,
            [FromQuery] int criterionId)
        {
            try
            {
                var rating = await _numericalValuesService
                    .CalculateRatingAsync(alternativeId, criterionId);

                return Ok(rating);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
