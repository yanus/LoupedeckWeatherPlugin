namespace Loupedeck.WeatherPlugin.Settings
{
    internal abstract class CommandSettings
    {
        protected const String LocationControlName = "location";
        protected const String LocationControlLabel = "Location";
        protected const String LocationControlDescription = "Specify the location to show weather information for.";

        protected const String APIKeyControlName = "apiKey";
        protected const String APIKeyControlLabel = "Weather API Key";
        protected const String APIKeyControlDescription = "You can get a free key on weatherapi.com.";

        protected const String ForecastControlName = "forecast";
        protected const String ForecastControlLabel = "Forecast";
        protected const String ForecastControlDescription = "Choose whether to show the current condition or a forecast.";
        protected static readonly String[][] ForecastControlChoices = [["0", "Current"], ["1", "Today"], ["2", "Tomorrow"]];
        protected const String ForecastControlDefault = "0";

        protected const String MetricControlName = "metric";
        protected const String MetricControlLabel = "Metric";
        protected const String MetricControlDescription = "Choose which weather metric to display.";

        protected const String ShowLocationControlName = "showLocation";
        protected const String ShowLocationControlLabel = "Show location";
        protected const String ShowLocationControlDescription = "Choose whether to show the name of the location.";
        protected const Boolean ShowLocationControlDefault = true;

        protected const String ShowMetricControlName = "showMetric";
        protected const String ShowMetricControlLabel = "Show metric";
        protected const String ShowMetricControlDescription = "Choose whether to show the name of the selected weather metric.";
        protected const Boolean ShowMetricControlDefault = false;

        protected const String ShowValueControlName = "showValue";
        protected const String ShowValueControlLabel = "Show value";
        protected const String ShowValueControlDescription = "Choose whether to show the value of the selected weather metric.";
        protected const Boolean ShowValueControlDefault = true;

        protected const String IconControlName = "icon";
        protected const String IconControlLabel = "Icon";
        protected const String IconControlDescription = "Choose whether to display no icon, an icon for the weather condition (e.g. partly cloudy), or an icon for the selected metric (e.g. humidity).";
        protected static readonly String[][] IconControlChoices = [["none", "None"], ["condition", "Condition"], ["metric", "Metric"]];
        protected const String IconControlDefault = "condition";

        public String Location;
        public String APIKey;
        public Int32 Forecast;
        public String Metric;
        public String Icon;
        public Boolean ShowLocation;
        public Boolean ShowMetric;
        public Boolean ShowValue;
    }
}
