namespace Loupedeck.WeatherPlugin.Models
{
    internal class WeatherMetric(String name, String unit, String icon, Boolean forecast = false)
    {
        public readonly String Name = name;
        public readonly String Unit = unit;
        public readonly Boolean Forecast = forecast;
        public readonly String Icon = icon;

        public const String DefaultCurrentMetric = "temp_c";
        public const String DefaultForecastMetric = "day.maxtemp_c";

        public static ICollection<String> Keys => _metrics.Keys;

        public static WeatherMetric Get(String name) => _metrics[name];

        private static readonly IDictionary<String, WeatherMetric> _metrics = new Dictionary<String, WeatherMetric>
        {
            { "temp_c", new WeatherMetric("Temperature (ºC)", "ºC", "temp") },
            { "temp_f", new WeatherMetric("Temperature (ºF)", "ºF", "temp") },
            { "feelslike_c", new WeatherMetric("Feels like temperature (ºC)", "ºC", "feelslike") },
            { "feelslike_f", new WeatherMetric("Feels like temperature (ºF)", "ºF", "feelslike") },
            { "windchill_c", new WeatherMetric("Windchill temperature (ºC)", "ºC", "windchill") },
            { "windchill_f", new WeatherMetric("Windchill temperature (ºF)", "ºF", "windchill") },
            { "heatindex_c", new WeatherMetric("Heat index (ºC)", "ºC", "heatindex") },
            { "heatindex_f", new WeatherMetric("Heat index (ºF)", "ºF", "heatindex") },
            { "dewpoint_c", new WeatherMetric("Dew point (ºC)", "ºC", "dewpoint") },
            { "dewpoint_f", new WeatherMetric("Dew point (ºF)", "ºF", "dewpoint") },
            { "wind_mph", new WeatherMetric("Wind speed (mph)", "mph", "wind") },
            { "wind_kph", new WeatherMetric("Wind speed (km/h)", "km/h", "wind") },
            { "wind_degree", new WeatherMetric("Wind direction", "º", "wind_degree") },
            { "pressure_mb", new WeatherMetric("Pressure (mb)", "mb", "pressure") },
            { "pressure_in", new WeatherMetric("Pressure (in)", "in", "pressure") },
            { "precip_mm", new WeatherMetric("Precipitation (mm)", "mm", "precip") },
            { "precip_in", new WeatherMetric("Precipitation (in)", "in", "precip") },
            { "humidity", new WeatherMetric("Humidity", "%", "humidity") },
            { "cloud", new WeatherMetric("Cloud cover", "%", "cloud") },
            { "vis_km", new WeatherMetric("Visibility (km)", "km", "vis") },
            { "vis_miles", new WeatherMetric("Visibility (mi)", "mi", "vis") },
            { "gust_mph", new WeatherMetric("Wind gust (mph)", "mph", "gust") },
            { "gust_kph", new WeatherMetric("Wind gust (km/h)", "km/h", "gust") },
            { "uv", new WeatherMetric("UV Index", "", "uv") },
            { "air_quality.co", new WeatherMetric("Carbon Monoxide (μg/m3)", "CO", "air_quality") },
            { "air_quality.no2", new WeatherMetric("Nitrogen dioxide (μg/m3)", "NO2", "air_quality") },
            { "air_quality.o3", new WeatherMetric("Ozone (μg/m3)", "O3", "air_quality") },
            { "air_quality.so2", new WeatherMetric("Sulphur dioxide (μg/m3)", "SO2", "air_quality") },
            { "air_quality.pm2_5", new WeatherMetric("PM2.5 (μg/m3)", "PM2.5", "air_quality") },
            { "air_quality.pm10", new WeatherMetric("PM10 (μg/m3)", "PM10", "air_quality") },
            { "day.maxtemp_c", new WeatherMetric("Maximum temperature (ºC)", "ºC", "maxtemp", true) },
            { "day.maxtemp_f", new WeatherMetric("Maximum temperature (ºF)", "ºF", "maxtemp", true) },
            { "day.mintemp_c", new WeatherMetric("Minimum temperature (ºC)", "ºC", "mintemp", true) },
            { "day.mintemp_f", new WeatherMetric("Minimum temperature (ºF)", "ºF", "mintemp", true) },
            { "day.avgtemp_c", new WeatherMetric("Average temperature (ºC)", "ºC", "avgtemp", true) },
            { "day.avgtemp_f", new WeatherMetric("Average temperature (ºF)", "ºF", "avgtemp", true) },
            { "day.maxwind_mph", new WeatherMetric("Maximum wind speed (mph)", "mph", "gust", true) },
            { "day.maxwind_kph", new WeatherMetric("Maximum wind speed (km/h)", "km/h", "gust", true) },
            { "day.totalprecip_mm", new WeatherMetric("Total precipitation (mm)", "mm", "precip", true) },
            { "day.totalprecip_in", new WeatherMetric("Total precipitation (in)", "in", "precip", true) },
            { "day.totalsnow_cm", new WeatherMetric("Total snowfall (cm)", "cm", "snow", true) },
            { "day.avgvis_km", new WeatherMetric("Average visibility (km)", "km", "vis", true) },
            { "day.avgvis_miles", new WeatherMetric("Average visibility (mi)", "mi", "vis", true) },
            { "day.avghumidity", new WeatherMetric("Average humidity", "%", "humidity", true) },
            { "day.daily_chance_of_rain", new WeatherMetric("Chance of rain", "%", "rain", true) },
            { "day.daily_chance_of_snow", new WeatherMetric("Chance of snow", "%", "snow", true) },
            { "day.uv", new WeatherMetric("UV Index", "", "uv", true) },
            { "astro.sunrise", new WeatherMetric("Sunrise time", "", "sunrise", true) },
            { "astro.sunset", new WeatherMetric("Sunset time", "", "sunset", true) },
            { "astro.moonrise", new WeatherMetric("Moonrise time", "", "moonrise", true) },
            { "astro.moonset", new WeatherMetric("Moonset time", "", "moonset", true) },
            { "astro.moon_phase", new WeatherMetric("Moon phase", "", "moon-new-moon", true) },
            { "astro.moon_illumination", new WeatherMetric("Moon illumination", "%", "moon_illumination", true) }
        };
    }
}
