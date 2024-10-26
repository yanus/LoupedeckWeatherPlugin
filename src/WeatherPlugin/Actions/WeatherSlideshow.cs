namespace Loupedeck.WeatherPlugin.Actions
{
    using System.Timers;

    using Loupedeck.WeatherPlugin.Models;
    using Loupedeck.WeatherPlugin.Settings;

    class WeatherSlideshow : ActionEditorCommand
    {
        private readonly Timer _timer;

        public WeatherSlideshow() : base()
        {
            this.DisplayName = "Weather Slideshow";
            this.Description = "Shows a slideshow of selected weather information. Changes at a set interval (or when pressed).";
            this.GroupName = "Weather";

            WeatherSlideshowSettings.InitActionEditor(this.ActionEditor, this);

            this.ActionEditor.Finished += (_, e) =>
            {
                if (!e.IsCanceled)
                {
                    this.ActionImageChanged();
                }
            };

            // Refresh images every second
            this._timer = new Timer(1000);
            this._timer.Elapsed += (_, _) => this.ActionImageChanged();
            this._timer.AutoReset = true;
            this._timer.Enabled = true;
        }

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
        {
            var settings = WeatherSlideshowSettings.FromActionParameters(actionParameters);
            settings.Shift();
            this.ActionImageChanged();
            return true;
        }

        protected override BitmapImage GetCommandImage(ActionEditorActionParameters actionParameters, Int32 imageWidth, Int32 imageHeight)
        {
            var settings = WeatherSlideshowSettings.FromActionParameters(actionParameters);

            var data = WeatherData.Get(settings.Location, settings.APIKey);

            return data.GetImage(settings, imageWidth, imageHeight);
        }
    }
}
