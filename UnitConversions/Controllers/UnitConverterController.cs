using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnitConversions.Services;
using UnitConversions.Models;


namespace UnitConversions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitConverterController : ControllerBase
    {
        private readonly IUnitConverterService _converterService;

        public UnitConverterController(IUnitConverterService converterService)
        {
            _converterService = converterService;
        }

        [HttpPost("convert")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConversionResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ConvertUnit([FromBody] ConversionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = _converterService.Convert(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            return Ok(_converterService.GetSupportedCategories());
        }

        [HttpGet("categories/{category}/units")]
        public IActionResult GetUnits(string category)
        {
            try
            {
                var units = _converterService.GetSupportedUnits(category);
                return Ok(units);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

    }
}
