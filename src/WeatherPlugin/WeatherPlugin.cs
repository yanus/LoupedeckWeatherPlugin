namespace Loupedeck.WeatherPlugin
{
    using System;

    using Loupedeck.WeatherPlugin.Models;

    public class WeatherPlugin : Plugin
    {
        internal const String APIKeySettingName = "apiKey";

        public override Boolean UsesApplicationApiOnly => true;

        public override Boolean HasNoApplication => true;

        public WeatherPlugin() => WeatherData.Init(this);
    }
}
