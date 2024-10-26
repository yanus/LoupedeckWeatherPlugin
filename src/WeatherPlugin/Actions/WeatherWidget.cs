namespace Loupedeck.WeatherPlugin.Actions
{
    using Loupedeck.WeatherPlugin.Models;
    using Loupedeck.WeatherPlugin.Settings;

    class WeatherWidget : ActionEditorCommand
    {
        public WeatherWidget() : base()
        {
            this.DisplayName = "Weather Widget";
            this.Description = "Shows weather information for a given location. Automatically updates every 5 minutes (or when pressed).";
            this.GroupName = "Weather";

            WeatherWidgetSettings.InitActionEditor(this.ActionEditor, this);

            this.ActionEditor.Finished += (_, e) =>
            {
                if (!e.IsCanceled)
                {
                    this.ActionImageChanged();
                }
            };

            WeatherData.DataUpdated += (_, _) => this.ActionImageChanged();
        }

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
        {
            var settings = WeatherWidgetSettings.FromActionParameters(actionParameters);

            WeatherData.Load(settings.Location, settings.APIKey);
            return true;
        }

        protected override BitmapImage GetCommandImage(ActionEditorActionParameters actionParameters, Int32 imageWidth, Int32 imageHeight)
        {
            var settings = WeatherWidgetSettings.FromActionParameters(actionParameters);

            var data = WeatherData.Get(settings.Location, settings.APIKey);

            return data.GetImage(settings, imageWidth, imageHeight);
        }
    }
}
