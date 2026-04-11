using System;
using ITCafe.Data.Settings;

namespace ITCafe
{
    public abstract class SettingsApplier
    {
        public abstract IDisposable BindSettings(SettingsModel settingsModel);
    }
}