using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iAssetTechnicalTestCore.Models
{
    public class WeatherForecastModel
    {
        public Coord coord { get; set; }
        public List<Weather> weather { get; set; }
        public MainInformation main { get; set; }
        public string visibility { get; set; }
        public Wind wind { get; set; }
        public string localTime { get; set; }
    }

    public class Coord
    {
        public string lon { get; set; }
        public string lat { get; set; }
    }
    public class Weather
    {
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }
    public class MainInformation
    {
        public string temp { get; set; }
        public string humidity { get; set; }
        public string pressure { get; set; }
    }
    public class Wind
    {
        public string speed { get; set; }
        public string deg { get; set; }
    }
}
