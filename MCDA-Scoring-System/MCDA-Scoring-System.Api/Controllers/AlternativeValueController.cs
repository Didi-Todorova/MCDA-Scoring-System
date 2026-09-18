using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.AlternativeValue;
using Microsoft.AspNetCore.Mvc;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlternativeValueController : ControllerBase
    {
        private readonly IAlternativeValueService _alternativeValueService;

        public AlternativeValueController(
            IAlternativeValueService alternativeValueService)
        {
            _alternativeValueService = alternativeValueService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
    [FromBody] CreateAlternativeValueDto dto)
        {
            try
            {
                var alternativeValue =
                    await _alternativeValueService
                        .CreateAlternativeValueAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = alternativeValue.Id },
                    alternativeValue);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var alternativeValue =
                await _alternativeValueService
                    .GetAlternativeValueByIdAsync(id);

            if (alternativeValue == null)
            {
                return NotFound(new
                {
                    Error = $"Alternative value with ID {id} not found."
                });
            }

            return Ok(alternativeValue);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var alternativeValues =
                await _alternativeValueService
                    .GetAllAlternativeValuesAsync();

            return Ok(alternativeValues);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateAlternativeValueDto dto)
        {
            try
            {
                await _alternativeValueService
                    .UpdateAlternativeValueAsync(id, dto);

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
        public async Task<IActionResult> Patch(
            int id,
            [FromBody] PatchAlternativeValueDto dto)
        {
            try
            {
                await _alternativeValueService
                    .PatchAlternativeValueAsync(id, dto);

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
                    await _alternativeValueService
                        .DeleteAlternativeValueAsync(id);

                if (!result)
                {
                    return NotFound(new
                    {
                        Error =
                            $"Alternative value with ID {id} not found."
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
    }
}