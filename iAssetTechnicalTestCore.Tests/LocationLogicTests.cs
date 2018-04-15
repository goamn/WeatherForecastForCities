using iAssetTechnicalTestCore.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace iAssetTechnicalTestCore.Tests
{

    public class LocationLogicTests
    {
        private static readonly HttpClient _client = new HttpClient();
        public IConfigurationRoot Configuration { get; }
        private readonly string battutaKey;
        private readonly string regionsByCountryUrl;
        private readonly string citiesByRegionUrl;
        private readonly string openWeatherMapKey;
        private readonly string cityWeatherForecastUrl;

        public LocationLogicTests()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            battutaKey = Configuration["iAssetConfig:BattutaKey"];
            regionsByCountryUrl = Configuration["iAssetConfig:regionsByCountryUrl"];
            citiesByRegionUrl = Configuration["iAssetConfig:citiesByRegionUrl"];

            openWeatherMapKey = Configuration["iAssetConfig:OpenWeatherMapKey"];
            cityWeatherForecastUrl = Configuration["iAssetConfig:cityWeatherForecastUrl"];
        }

        [Fact]
        public void TestBattutaConfigExists()
        {
            Assert.False(string.IsNullOrWhiteSpace(battutaKey));
            Assert.False(string.IsNullOrWhiteSpace(regionsByCountryUrl));
            Assert.False(string.IsNullOrWhiteSpace(citiesByRegionUrl));
        }

        [Fact]
        public async Task TestBattutaRegionsByCountry()
        {
            string completeUrl = $"{regionsByCountryUrl.Replace("{COUNTRY}", "AU")}{battutaKey}";
            var response = await _client.GetStringAsync(completeUrl);
            if (!string.IsNullOrWhiteSpace(response))
            {
                var jsonResult = JsonConvert.DeserializeObject<List<RegionModel>>(response);
                Assert.Equal(8, jsonResult.Count);
                Assert.Contains("State of New South Wales", string.Join(',', jsonResult.Select(s=>s.region)));
            }
            else
            {
                Assert.True(false, "response returned was null or empty");
            }
        }

        [Fact]
        public async Task TestBattutaCitiesByRegion()
        {
            string completeUrl = $"{citiesByRegionUrl.Replace("{COUNTRY}", "AU").Replace("{REGION}", "Northern Territory")}{battutaKey}";
            var response = await _client.GetStringAsync(completeUrl);
            if (!string.IsNullOrWhiteSpace(response))
            {
                var jsonResult = JsonConvert.DeserializeObject<List<CityRegionModel>>(response);
                Assert.Equal(16, jsonResult.Count);
                Assert.Contains("Darwin", string.Join(',', jsonResult.Select(s=>s.city)));
            }
            else
            {
                Assert.True(false, "response returned was null or empty");
            }
        }

        [Fact]
        public void TestOpenWeatherConfigExists()
        {
            Assert.False(string.IsNullOrWhiteSpace(openWeatherMapKey));
            Assert.False(string.IsNullOrWhiteSpace(cityWeatherForecastUrl));
        }

        [Fact]
        public async Task TestOpenWeatherForecast()
        {
            string completeUrl = $"{cityWeatherForecastUrl.Replace("{COUNTRY}", "AU").Replace("{CITY}", "Cairns")}{openWeatherMapKey}";
            var response = await _client.GetStringAsync(completeUrl);
            if (!string.IsNullOrWhiteSpace(response))
            {
                var jsonResult = JsonConvert.DeserializeObject<WeatherForecastModel>(response);
                Assert.Single(jsonResult.weather);
                Assert.NotEmpty(jsonResult.weather.FirstOrDefault()?.main);
                Assert.Equal("-16.92,145.77", $"{jsonResult.coord.lat},{jsonResult.coord.lon}");
            }
            else
            {
                Assert.True(false, "response returned was null or empty");
            }
        }
    }
}
