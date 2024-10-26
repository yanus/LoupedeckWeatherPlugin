namespace Loupedeck.WeatherPlugin.Models
{
    using System.Text.Json.Nodes;
    using System.Timers;
    using System.Web;

    using Loupedeck.WeatherPlugin.Settings;

    // Class for storing a location's weather data
    internal class WeatherData
    {
        public String Name = "Loading...";

        // The first dictionary in the list contains metrics for the current weather, the second and third contain the forecasts for today and tomorrow
        public IList<IDictionary<String, String>> Forecasts = [];
        public IList<String> WeatherIcons = [];

        public Boolean IsValid = false;
        public Boolean IsLoading = false;

        // Generate image to show for the location
        public BitmapImage GetImage(CommandSettings settings, Int32 imageWidth, Int32 imageHeight)
        {
            // Create a new image with a black background
            var img = new BitmapBuilder(imageWidth, imageHeight);
            img.Clear(BitmapColor.Black);

            if (this.IsValid == false)
            {
                // Only display the title in case the data are still loading or there was an error
                img.DrawText(this.Name);
            }
            else
            {
                var value = this.Forecasts[settings.Forecast][settings.Metric];
                var name = this.Name;
                var metric = WeatherMetric.Get(settings.Metric).Name;
                var valueWithUnit = $"{value} {WeatherMetric.Get(settings.Metric).Unit}";
                var text = settings.ShowLocation ? settings.ShowMetric ? settings.ShowValue ? $"{name}\n{metric}\n{valueWithUnit}" : $"{name}\n\u00A0\n{metric}"
                                                 : settings.ShowValue ? $"{name}\n\u00A0\n{valueWithUnit}" : name
                                                 : settings.ShowMetric ? settings.ShowValue ? $"{metric}\n\u00A0\n{valueWithUnit}" : metric
                                                 : settings.ShowValue ? $"{valueWithUnit}" : "";

                if (settings.Icon.Equals("condition"))
                {
                    // Get cached image for the condition
                    img.DrawImage(_imagesCache[this.WeatherIcons[settings.Forecast]]);
                }
                else if (settings.Icon.Equals("metric"))
                {
                    BitmapImage bgSVG = null;

                    // Get special icons for wind direction and moon phase
                    if (settings.Metric.Equals("wind_degree"))
                    {
                        try
                        {
                            var wind_deg = _plugin.Assembly.ReadTextFile("Loupedeck.WeatherPlugin.Resources.wind_degree.svg");
                            wind_deg = wind_deg.Replace("rotate(0)", $"rotate({value})");
                            bgSVG = BitmapImage.FromSvg(wind_deg);
                        }
                        catch { }
                    }
                    else if (settings.Metric.Equals("astro.moon_phase"))
                    {
                        var filename = value.ToLower().Replace(' ', '-');
                        bgSVG = BitmapImage.FromResource(_plugin.Assembly, "Loupedeck.WeatherPlugin.Resources.moon-" + filename + ".svg");
                    }

                    // Get regular icon
                    bgSVG ??= BitmapImage.FromResource(_plugin.Assembly, "Loupedeck.WeatherPlugin.Resources." + WeatherMetric.Get(settings.Metric).Icon + ".svg") ?? BitmapImage.FromResource(_plugin.Assembly, "Loupedeck.WeatherPlugin.Resources.unknown.svg");

                    img.DrawImage(bgSVG.VectorToBitmap(imageWidth, imageHeight, BitmapColor.White));
                }

                img.FillRectangle(0, 0, img.Width, img.Height, new BitmapColor(0, 0, 0, 128));
                img.DrawText(text);
            }

            return img.ToImage();
        }

        private static readonly Dictionary<String, WeatherData> _data = [];
        private static readonly Dictionary<String, BitmapImage> _imagesCache = [];
        private static readonly HttpClient _httpClient = new();

        public static event EventHandler DataUpdated;

        private static Plugin _plugin;
        private static Timer _timer;
        public static void Init(Plugin plugin)
        {
            _plugin = plugin;

            _timer = new Timer(5 * 60 * 1000);
            _timer.Elapsed += (_, _) =>
            {
                _data.Clear();
                DataUpdated?.Invoke(_data, null);
            };
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public static WeatherData Get(String location, String apiKey)
        {
            if (_data.TryGetValue(location, out var data))
            {
                return data;
            }

            data = new WeatherData();
            _data[location] = data;

            Load(location, apiKey);

            return data;
        }

        public static async void Load(String location, String apiKey)
        {
            var data = Get(location, apiKey);
            if (data.IsLoading)
            {
                return;
            }

            data.IsLoading = true;

            try
            {
                var jsonRes = await _httpClient.GetAsync($"https://api.weatherapi.com/v1/forecast.json?key={apiKey}&q={HttpUtility.UrlEncode(location)}&days=2&aqi=yes");
                var json = JsonNode.Parse(await jsonRes.Content.ReadAsStringAsync());

                data.Name = json["location"]["name"].GetValue<String>();

                // Get weather data ...
                data.Forecasts.Clear();

                // ... for the current condition
                {
                    var current = new Dictionary<String, String>();
                    foreach (var metric in WeatherMetric.Keys)
                    {
                        if (!WeatherMetric.Get(metric).Forecast)
                        {
                            current.Add(metric, GetValue(json["current"], metric));
                        }
                    }
                    data.Forecasts.Add(current);
                }

                // .. for the forecasts
                foreach (var day in json["forecast"]["forecastday"].AsArray())
                {
                    var forecast = new Dictionary<String, String>();

                    foreach (var metric in WeatherMetric.Keys)
                    {
                        if (WeatherMetric.Get(metric).Forecast)
                        {
                            forecast.Add(metric, GetValue(day, metric));
                        }
                    }

                    data.Forecasts.Add(forecast);
                }

                // Get weather icons ...
                data.WeatherIcons.Clear();

                // ... for the current condition
                {
                    var weatherIconUrl = "https:" + json["current"]["condition"]["icon"].GetValue<String>();
                    if (!_imagesCache.TryGetValue(weatherIconUrl, out var weatherIcon))
                    {
                        var iconRes = await _httpClient.GetAsync(weatherIconUrl);
                        _imagesCache[weatherIconUrl] = BitmapImage.FromArray(await iconRes.Content.ReadAsByteArrayAsync());
                    }
                    data.WeatherIcons.Add(weatherIconUrl);
                }

                // .. for the forecasts
                foreach (var day in json["forecast"]["forecastday"].AsArray())
                {
                    var weatherIconUrl = "https:" + day["day"]["condition"]["icon"].GetValue<String>();
                    if (!_imagesCache.TryGetValue(weatherIconUrl, out var weatherIcon))
                    {
                        var iconRes = await _httpClient.GetAsync(weatherIconUrl);
                        _imagesCache[weatherIconUrl] = BitmapImage.FromArray(await iconRes.Content.ReadAsByteArrayAsync());
                    }
                    data.WeatherIcons.Add(weatherIconUrl);
                }

                data.IsValid = true;

                // Save successful API Key for re-use
                _plugin?.SetPluginSetting(WeatherPlugin.APIKeySettingName, apiKey);
            }
            catch (Exception e)
            {
                data.Name = e.Message;
            }
            finally
            {
                data.IsLoading = false;
                DataUpdated?.Invoke(null, null);
            }
        }

        private static String GetValue(JsonNode json, String index)
        {
            var node = json;
            foreach (var segment in index.Split('.'))
            {
                node = node[segment];
            }
            if (node.GetValueKind() == System.Text.Json.JsonValueKind.String)
            {
                return node.GetValue<String>();
            }
            else if (node.GetValueKind() == System.Text.Json.JsonValueKind.Number)
            {
                return node.GetValue<Single>().ToString();
            }
            return null;
        }
    }
}
