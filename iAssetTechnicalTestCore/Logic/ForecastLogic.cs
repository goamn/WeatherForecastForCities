using iAssetTechnicalTestCore.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace iAssetTechnicalTestCore.Logic
{
    public interface IForecastLogic
    {
        Task<WeatherForecastModel> GetForecastForCity(string countryCode, string regionName);
    }
    public class ForecastLogic : IForecastLogic
    {
        private static readonly HttpClient _client = new HttpClient();
        private IConfiguration _config;

        public ForecastLogic(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<WeatherForecastModel> GetForecastForCity(string countryCode, string cityName)
        {
            var cityNameEncoded = WebUtility.HtmlEncode(cityName);
            string cityWeatherForecastUrl = _config["iAssetConfig:cityWeatherForecastUrl"]?.Replace("{COUNTRY}", countryCode).Replace("{CITY}", cityNameEncoded) + _config["iAssetConfig:OpenWeatherMapKey"];
            var response = await _client.GetAsync(cityWeatherForecastUrl);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var weatherForecastModel = JsonConvert.DeserializeObject<WeatherForecastModel>(responseString);
                return weatherForecastModel;
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException();
            }
            return null;
        }
    }
}
