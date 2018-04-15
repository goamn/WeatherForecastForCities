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
    public interface ILocationLogic
    {
        Task<IEnumerable<string>> GetRegionsByCountry(string countryCode);
        Task<IEnumerable<string>> GetCitiesByRegion(string countryCode, string regionName);
        Task<DateTime?> GetLocalDateTime(string lat, string lon, DateTime dateTime);
    }
    public class LocationLogic : ILocationLogic
    {
        private static readonly HttpClient _client = new HttpClient();
        private IConfiguration _config;

        public LocationLogic(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<IEnumerable<string>> GetCitiesByRegion(string countryCode, string regionName)
        {
            var regionNameEncoded = WebUtility.HtmlEncode(regionName);
            string citiesByRegionUrl = _config["iAssetConfig:citiesByRegionUrl"]?.Replace("{COUNTRY}", countryCode).Replace("{REGION}", regionNameEncoded) + _config["iAssetConfig:BattutaKey"];
            var response = await _client.GetStringAsync(citiesByRegionUrl);
            if (!string.IsNullOrWhiteSpace(response))
            {
                var jsonResult = JsonConvert.DeserializeObject<List<CityRegionModel>>(response);
                return jsonResult.Select(s => s.city);
            }
            return null;
        }

        public async Task<DateTime?> GetLocalDateTime(string latitude, string longitude, DateTime utcDate)
        {
            var googleTimeUrl = $"https://maps.googleapis.com/maps/api/timezone/json?location={latitude},{longitude}&timestamp={this.ToTimestamp(utcDate)}&sensor=false";
            var response = await _client.GetStringAsync(googleTimeUrl);
            if (!string.IsNullOrWhiteSpace(response))
            {
                var jsonResult = JsonConvert.DeserializeObject<GoogleTimeZone>(response);
                return utcDate.AddSeconds(jsonResult.rawOffset + jsonResult.dstOffset);
            }
            return null;
        }

        public async Task<IEnumerable<string>> GetRegionsByCountry(string countryCode)
        {
            string regionsByCountryUrl = _config["iAssetConfig:regionsByCountryUrl"]?.Replace("{COUNTRY}", countryCode) + _config["iAssetConfig:BattutaKey"];
            var response = await _client.GetStringAsync(regionsByCountryUrl);
            if (!string.IsNullOrWhiteSpace(response))
            {
                var jsonResult = JsonConvert.DeserializeObject<List<RegionModel>>(response);
                return jsonResult.Select(s => s.region);
            }
            return null;
        }

        public double ToTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}
