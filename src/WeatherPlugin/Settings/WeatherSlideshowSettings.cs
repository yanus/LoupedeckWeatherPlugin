namespace Loupedeck.WeatherPlugin.Settings
{
    using Loupedeck.WeatherPlugin.Models;

    internal class WeatherSlideshowSettings : CommandSettings
    {
        private const String IntervalControlName = "interval";
        private const String IntervalControlLabel = "Interval (s)";
        private const String IntervalControlDescription = "Choose after how many seconds to change the displayed metric.";

        private const String InstanceControlName = "instance";

        private const Int32 NumSlides = 5;

        private Guid _instance;

        // Each instance of the command can be advanced manually
        private static readonly Dictionary<Guid, Int32> _slideShift = [];
        public void Shift() => _slideShift[this._instance]++;

        public static WeatherSlideshowSettings FromActionParameters(ActionEditorActionParameters actionParameters)
        {
            var settings = new WeatherSlideshowSettings();

            ArgumentNullException.ThrowIfNull(actionParameters);

            settings.Location = actionParameters.GetString(LocationControlName);
            settings.APIKey = actionParameters.GetString(APIKeyControlName);
            settings.Forecast = actionParameters.GetInt32(ForecastControlName);
            settings.Icon = actionParameters.GetString(IconControlName);
            settings.ShowLocation = actionParameters.GetBoolean(ShowLocationControlName, true);
            settings.ShowMetric = actionParameters.GetBoolean(ShowMetricControlName, false);
            settings.ShowValue = actionParameters.GetBoolean(ShowValueControlName, true);

            if (settings.Location.IsNullOrEmpty() ||
                settings.APIKey.IsNullOrEmpty() ||
                settings.Icon.IsNullOrEmpty())
            {
                throw new ArgumentException("Required parameter is missing.");
            }

            if (settings.Forecast < 0 || settings.Forecast > 2)
            {
                throw new ArgumentException("Forecast parameter out of range.");
            }

            // The metric is selected from the given options based on the time and manual shift
            IList<String> metrics = [];
            for (var i = 0; i < NumSlides; i++)
            {
                var metric = actionParameters.GetString(MetricControlName + i);
                if (!metric.IsNullOrEmpty())
                {
                    metrics.Add(metric);
                }
            }
            if (metrics.Count == 0)
            {
                throw new ArgumentException("Required parameter is missing.");
            }

            var interval = actionParameters.GetInt32(IntervalControlName);

            settings._instance = Guid.Parse(actionParameters.GetString(InstanceControlName));
            var manualShift = _slideShift.TryGetValue(settings._instance, out var value) ? value : 0;
            _slideShift[settings._instance] = manualShift % metrics.Count;

            var timeShift = interval > 0 ? (DateTime.Now.Minute * 60 + DateTime.Now.Second) / interval : 0;

            settings.Metric = metrics[(timeShift + manualShift) % metrics.Count];

            return settings;
        }

        public static void InitActionEditor(ActionEditor actionEditor, ActionEditorCommand command)
        {
            ArgumentNullException.ThrowIfNull(actionEditor);

            actionEditor.AddControlEx(new ActionEditorTextbox(name: LocationControlName, labelText: LocationControlLabel, description: LocationControlDescription).SetRequired());
            actionEditor.AddControlEx(new ActionEditorTextbox(name: APIKeyControlName, labelText: APIKeyControlLabel, description: APIKeyControlDescription).SetRequired());
            actionEditor.AddControlEx(new ActionEditorSlider(name: IntervalControlName, labelText: IntervalControlLabel, IntervalControlDescription).SetValues(0, 60, 5, 1));
            actionEditor.AddControlEx(new ActionEditorListbox(name: ForecastControlName, labelText: ForecastControlLabel, description: ForecastControlDescription).SetRequired());
            for (var i = 0; i < NumSlides; i++)
            {
                var listbox = new ActionEditorListbox(name: MetricControlName + i, labelText: $"{MetricControlLabel} {i + 1}", description: MetricControlDescription);
                if (i == 0)
                {
                    listbox.SetRequired();
                }
                actionEditor.AddControlEx(listbox);
            }
            actionEditor.AddControlEx(new ActionEditorListbox(name: IconControlName, labelText: IconControlLabel, description: IconControlDescription).SetRequired());
            actionEditor.AddControlEx(new ActionEditorCheckbox(name: ShowLocationControlName, labelText: ShowLocationControlLabel, description: ShowLocationControlDescription).SetDefaultValue(ShowLocationControlDefault));
            actionEditor.AddControlEx(new ActionEditorCheckbox(name: ShowMetricControlName, labelText: ShowMetricControlLabel, description: ShowMetricControlDescription).SetDefaultValue(ShowMetricControlDefault));
            actionEditor.AddControlEx(new ActionEditorCheckbox(name: ShowValueControlName, labelText: ShowValueControlLabel, description: ShowValueControlDescription).SetDefaultValue(ShowValueControlDefault));
            actionEditor.AddControlEx(new ActionEditorHidden(name: InstanceControlName));

            // Set default values
            actionEditor.ControlsStateRequested += (_, e) =>
            {
                if (e.ActionEditorState.GetControlValue(APIKeyControlName).IsNullOrEmpty() &&
                    command.Plugin.TryGetPluginSetting(WeatherPlugin.APIKeySettingName, out var apiKey))
                {
                    e.ActionEditorState.SetValue(APIKeyControlName, apiKey);
                }

                if (e.ActionEditorState.GetControlValue(InstanceControlName).IsNullOrEmpty())
                {
                    e.ActionEditorState.SetValue(InstanceControlName, Guid.NewGuid().ToString());
                }

                if (e.ActionEditorState.GetControlValue(ForecastControlName).IsNullOrEmpty())
                {
                    e.ActionEditorState.SetValue(ForecastControlName, ForecastControlDefault);
                }

                if (e.ActionEditorState.GetControlValue(IconControlName).IsNullOrEmpty())
                {
                    e.ActionEditorState.SetValue(IconControlName, IconControlDefault);
                }
            };

            actionEditor.ListboxItemsRequested += (_, e) =>
            {
                if (e.ControlName.Equals(ForecastControlName))
                {
                    foreach (var choice in ForecastControlChoices)
                    {
                        e.AddItem(choice[0], choice[1], choice[1]);
                    }
                }
                else if (e.ControlName.StartsWith(MetricControlName))
                {
                    var forecast = e.ActionEditorState.GetControlValue(ForecastControlName);
                    var isForecast = !(forecast.IsNullOrEmpty() || forecast.Equals("0"));

                    var exists = false;
                    var currentMetric = e.ActionEditorState.GetControlValue(e.ControlName);

                    foreach (var metric in WeatherMetric.Keys)
                    {
                        if (isForecast == WeatherMetric.Get(metric).Forecast)
                        {
                            e.AddItem(metric, WeatherMetric.Get(metric).Name, WeatherMetric.Get(metric).Name);
                            if (metric.Equals(currentMetric))
                            {
                                exists = true;
                            }
                        }
                    }
                    if (!exists)
                    {
                        e.SetSelectedItemName(e.ControlName.Equals(MetricControlName + 0) ? isForecast ? WeatherMetric.DefaultForecastMetric : WeatherMetric.DefaultCurrentMetric : "");
                    }
                }
                else if (e.ControlName.Equals(IconControlName))
                {
                    foreach (var choice in IconControlChoices)
                    {
                        e.AddItem(choice[0], choice[1], choice[1]);
                    }
                }
            };
            actionEditor.ControlValueChanged += (_, e) =>
            {
                if (e.ControlName.Equals(ForecastControlName))
                {
                    for (var i = 0; i < NumSlides; i++)
                    {
                        actionEditor.ListboxItemsChanged(MetricControlName + i);
                    }
                }
            };
        }
    }
}
