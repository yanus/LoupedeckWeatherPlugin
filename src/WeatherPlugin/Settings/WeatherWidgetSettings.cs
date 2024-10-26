namespace Loupedeck.WeatherPlugin.Settings
{
    using Loupedeck.WeatherPlugin.Models;

    internal class WeatherWidgetSettings : CommandSettings
    {
        public static WeatherWidgetSettings FromActionParameters(ActionEditorActionParameters actionParameters)
        {
            var settings = new WeatherWidgetSettings();

            ArgumentNullException.ThrowIfNull(actionParameters);

            settings.Location = actionParameters.GetString(LocationControlName);
            settings.APIKey = actionParameters.GetString(APIKeyControlName);
            settings.Forecast = actionParameters.GetInt32(ForecastControlName);
            settings.Metric = actionParameters.GetString(MetricControlName);
            settings.Icon = actionParameters.GetString(IconControlName);
            settings.ShowLocation = actionParameters.GetBoolean(ShowLocationControlName, true);
            settings.ShowMetric = actionParameters.GetBoolean(ShowMetricControlName, false);
            settings.ShowValue = actionParameters.GetBoolean(ShowValueControlName, true);

            if (settings.Location.IsNullOrEmpty() ||
                settings.APIKey.IsNullOrEmpty() ||
                settings.Metric.IsNullOrEmpty() ||
                settings.Icon.IsNullOrEmpty())
            {
                throw new ArgumentException("Required parameter is missing.");
            }

            if (settings.Forecast < 0 || settings.Forecast > 2)
            {
                throw new ArgumentException("Forecast parameter out of range.");
            }

            return settings;
        }

        public static void InitActionEditor(ActionEditor actionEditor, ActionEditorCommand command)
        {
            ArgumentNullException.ThrowIfNull(actionEditor);

            actionEditor.AddControlEx(new ActionEditorTextbox(name: LocationControlName, labelText: LocationControlLabel, description: LocationControlDescription).SetRequired());
            actionEditor.AddControlEx(new ActionEditorTextbox(name: APIKeyControlName, labelText: APIKeyControlLabel, description: APIKeyControlDescription).SetRequired());
            actionEditor.AddControlEx(new ActionEditorListbox(name: ForecastControlName, labelText: ForecastControlLabel, description: ForecastControlDescription).SetRequired());
            actionEditor.AddControlEx(new ActionEditorListbox(name: MetricControlName, labelText: MetricControlLabel, description: MetricControlDescription).SetRequired());
            actionEditor.AddControlEx(new ActionEditorListbox(name: IconControlName, labelText: IconControlLabel, IconControlDescription).SetRequired());
            actionEditor.AddControlEx(new ActionEditorCheckbox(name: ShowLocationControlName, labelText: ShowLocationControlLabel, description: ShowLocationControlDescription).SetDefaultValue(ShowLocationControlDefault));
            actionEditor.AddControlEx(new ActionEditorCheckbox(name: ShowMetricControlName, labelText: ShowMetricControlLabel, description: ShowMetricControlDescription).SetDefaultValue(ShowMetricControlDefault));
            actionEditor.AddControlEx(new ActionEditorCheckbox(name: ShowValueControlName, labelText: ShowValueControlLabel, description: ShowValueControlDescription).SetDefaultValue(ShowValueControlDefault));

            // Set default values
            actionEditor.ControlsStateRequested += (_, e) =>
            {
                if (e.ActionEditorState.GetControlValue(APIKeyControlName).IsNullOrEmpty() &&
                    command.Plugin.TryGetPluginSetting(WeatherPlugin.APIKeySettingName, out var apiKey))
                {
                    e.ActionEditorState.SetValue(APIKeyControlName, apiKey);
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
                else if (e.ControlName.Equals(MetricControlName))
                {
                    var forecast = e.ActionEditorState.GetControlValue(ForecastControlName);
                    var isForecast = !(forecast.IsNullOrEmpty() || forecast.Equals("0"));

                    var exists = false;
                    var currentMetric = e.ActionEditorState.GetControlValue(MetricControlName);

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
                        e.SetSelectedItemName(isForecast ? WeatherMetric.DefaultForecastMetric : WeatherMetric.DefaultCurrentMetric);
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
                    actionEditor.ListboxItemsChanged(MetricControlName);
                }
            };
        }
    }
}
