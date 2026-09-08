using Microsoft.AspNetCore.Mvc;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Api.Controllers
{

    namespace MCDA_Scoring_System.MCDA_Scoring_System.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class NumericalValuesController : ControllerBase
        {
            private readonly INumericValuesService _numericalValuesService;

            public NumericalValuesController(
                INumericValuesService numericalValuesService)
            {
                _numericalValuesService = numericalValuesService;
            }

            [HttpGet("rating")]
            public async Task<IActionResult> CalculateRating(
                [FromQuery] int alternativeId,
                [FromQuery] int criterionId)
            {
                var rating = await _numericalValuesService.CalculateRatingAsync(
                    alternativeId,
                    criterionId);

                return Ok(rating);
            }
        }
    }
}
