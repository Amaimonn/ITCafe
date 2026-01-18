using System;
using DevKit.UI.MVVM;
using DevKit.UI.MVVM.Bases;
using Inui.UI.MVVM.Settings;
using ITCafe.Data.Settings;
using R3;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class SettingsBinder : SimpleAttachBinder<SettingsView, SettingsViewModel>
    {
        private readonly SettingsModel _settingsModel;
        private readonly ILocalizationLoader _settingsLocaleLoader;
        private AsyncOperationHandle? _settingsDataHandle;

        public SettingsBinder(SettingsModel settingsModel,
            Func<SettingsViewModel> viewModelFactory,
            IRootUIBinder rootUIBinder,
            SettingsView view,
            [Key(Constants.SETTINGS_DATA_LOCALE_LOADER)] ILocalizationLoader settingsLocaleLoader) :
                base(viewModelFactory, rootUIBinder, view)
        {
            _settingsModel = settingsModel;
            _settingsLocaleLoader = settingsLocaleLoader;
        }

        protected override SettingsViewModel GetViewModel()
        {
            var viewModel = base.GetViewModel();

            viewModel.Bind(_settingsModel);

            var settingsDataHandle = Addressables.LoadAssetAsync<SettingsDataSO>(Constants.SETTINGS_DATA_PATH);
            _settingsDataHandle = settingsDataHandle;

            var isLocaleLoaded = false;
            ISettingsData settingsData = null;
            
            settingsDataHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    settingsData = handle.Result;
                    if (isLocaleLoaded)
                        viewModel.SetSettingsData(settingsData);
                }
            };

            _settingsLocaleLoader.Init();
            var localeObservable = _settingsLocaleLoader.LoadTablesObservable();
            localeObservable.Take(1).Subscribe(_ =>
            {
                isLocaleLoaded = true;
                if (settingsData != null)
                    viewModel.SetSettingsData(settingsData);
            });

            return viewModel;
        }

        protected override void DisposeInstances()
        {
            _settingsDataHandle?.Release();
            _settingsLocaleLoader.Dispose();
            base.DisposeInstances();
        }
    }
}