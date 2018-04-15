

$(document).ready(function () {
    var koModel = new WeatherLocationModel();
    ko.applyBindings(koModel);

    initialiseAjaxCalls(koModel);

    koModel.forecastLoaded.extend({
        notify: 'always' 
    });
    koModel.forecastLoaded.subscribe(function () {
        koModel.errorMessage('');
    });
});

function WeatherLocationModel() {
    var self = this;
    self.country = ko.observable('');
    self.region = ko.observable('');
    self.city = ko.observable('');
    self.regionsList = ko.observable('');
    self.citiesList = ko.observable('');

    self.forecastLoaded = ko.observable(false);
    self.errorMessage = ko.observable('');
    self.WeatherForecast = new WeatherForecast();
    return self;
}
function WeatherForecast() {
    var self = this;
    self.LocalTime = ko.observable('');
    self.long = ko.observable('');
    self.lati = ko.observable('');
    self.weatherMain = ko.observable('');
    self.weatherDesc = ko.observable('');
    self.weatherIcon = ko.observable('');
    self.temp = ko.observable('');
    self.humidity = ko.observable('');
    self.pressure = ko.observable('');
    self.visibility = ko.observable('');
    self.windSpeed = ko.observable('');
    self.windDirection = ko.observable('');
    return self;
}

function initialiseAjaxCalls(koModel) {
    $('#countryDropdown').change(function () {
        if (!$(this).val() || $(this).val() === '') {
            return;
        }
        koModel.forecastLoaded(false);
        $.ajax({
            url: 'Weather/RegionsByCountry',
            type: 'GET',
            data: { countryCode: koModel.country() },
            success: function (serverData) {
                if (serverData) {
                    koModel.city('');
                    koModel.region('');
                    koModel.citiesList('');
                    koModel.regionsList(serverData);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                //set knockout error variable
                console.log("Status: " + textStatus + ". Error: " + errorThrown);
            }
        });
    });


    $('#regionDropdown').change(function () {
        if (!$(this).val() || $(this).val() === '') {
            return;
        }
        koModel.forecastLoaded(false);
        $.ajax({
            url: 'Weather/CitiesByRegion',
            type: 'GET',
            data: {
                countryCode: koModel.country(),
                regionName: koModel.region()
            },
            success: function (serverData) {
                if (serverData) {
                    koModel.city('');
                    koModel.citiesList(serverData);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                //set knockout error variable
                console.log("Status: " + textStatus + ". Error: " + errorThrown);
            }
        });
    });


    $('#cityDropdown').change(function () {
        if (!$(this).val() || $(this).val() === '') {
            return;
        }
        koModel.forecastLoaded(false);
        $.ajax({
            url: 'Weather/ForecastCity',
            type: 'GET',
            data: {
                countryCode: koModel.country(),
                cityName: koModel.city()
            },
            success: function (serverData) {
                if (serverData) {
                    if (!serverData.error) {
                        koModel.forecastLoaded(true);
                        LoadWeatherForecast(serverData);
                    } else {
                        koModel.forecastLoaded(false);
                        koModel.errorMessage(serverData.error);
                    }
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                //set knockout error variable
                console.log("Status: " + textStatus + ". Error: " + errorThrown);
            }
        });
    });
    function LoadWeatherForecast(serverData) {
        koModel.WeatherForecast.LocalTime(serverData.localTime);
        koModel.WeatherForecast.long(serverData.coord.lon);
        koModel.WeatherForecast.lati(serverData.coord.lat);
        koModel.WeatherForecast.weatherMain(serverData.weather[0].main);
        koModel.WeatherForecast.weatherDesc(serverData.weather[0].description);
        koModel.WeatherForecast.weatherIcon(serverData.weather[0].icon);
        var temperatureInCelsius = parseFloat(serverData.main.temp - 273.15).toFixed(2);
        koModel.WeatherForecast.temp(temperatureInCelsius);
        koModel.WeatherForecast.humidity(serverData.main.humidity);
        koModel.WeatherForecast.pressure(serverData.main.pressure);
        koModel.WeatherForecast.visibility(serverData.visibility);
        koModel.WeatherForecast.windSpeed(serverData.wind.speed);
        koModel.WeatherForecast.windDirection(serverData.wind.deg);
    };
    $.ajaxSetup({
        beforeSend: function () {
            $.LoadingOverlay("show");
        },
        complete: function () {
            $.LoadingOverlay("hide");
        }
    });
}