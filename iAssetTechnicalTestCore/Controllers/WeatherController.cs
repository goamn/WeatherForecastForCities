using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iAssetTechnicalTestCore.Logic;
using Microsoft.AspNetCore.Mvc;

namespace iAssetTechnicalTestCore.Controllers
{
    public class WeatherController : Controller
    {
        private ILocationLogic _locationLogic;
        private IForecastLogic _forecastLogic;

        public WeatherController(ILocationLogic locationLogic,
             IForecastLogic forecastLogic)
        {
            _locationLogic = locationLogic;
            _forecastLogic = forecastLogic;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RegionsByCountry(string countryCode)
        {
            var regions = await _locationLogic.GetRegionsByCountry(countryCode);
            return Json(regions);
        }

        [HttpGet]
        public async Task<IActionResult> CitiesByRegion(string countryCode, string regionName)
        {
            var cities = await _locationLogic.GetCitiesByRegion(countryCode, regionName);
            return Json(cities);
        }

        [HttpGet]
        public async Task<IActionResult> ForecastCity(string countryCode, string cityName)
        {
            try
            {
                var forecast = await _forecastLogic.GetForecastForCity(countryCode, cityName);
                var localTime = await _locationLogic.GetLocalDateTime(forecast.coord.lat, forecast.coord.lon, DateTime.UtcNow);
                forecast.localTime = localTime.HasValue ? localTime.Value.ToString() : "";
                return Json(forecast);
            }
            catch (KeyNotFoundException)
            {
                return Json(new { result = "", error = "City name not found" });
            }
        }
    }
}