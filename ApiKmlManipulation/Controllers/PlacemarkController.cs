using System;
using Microsoft.AspNetCore.Mvc;
using ApiKMLManipulation.Services;

namespace ApiKMLManipulation.Controllers
{
    [ApiController]
    [Route("api/placemarks")]
    public class PlacemarkController : ControllerBase
    {
        private readonly PlacemarkService _placemarkService;

        public PlacemarkController(PlacemarkService placemarkService)
        {
            _placemarkService = placemarkService;
        }

        /// <summary>
        /// Obt�m os valores �nicos de CLIENTE, SITUA��O e BAIRRO para filtragem.
        /// </summary>
        /// <returns>Objeto JSON com listas de valores �nicos.</returns>
        [HttpGet("filters")]
        public IActionResult GetUniqueFilterValues()
        {
            try
            {
                var filterValues = _placemarkService.GetUniqueFilterValues();

                return Ok(filterValues);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
