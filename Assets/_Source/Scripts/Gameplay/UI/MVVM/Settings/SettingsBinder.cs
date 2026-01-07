using System;
using DevKit.Solutions;
using DevKit.UI.MVVM;
using DevKit.UI.MVVM.Bases;
using Inui.UI.MVVM.Settings;
using ITCafe.Data.Settings;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class SettingsBinder : SimpleAttachBinder<SettingsView, SettingsViewModel>
    {
        private readonly SettingsModel _settingsModel;
        private AsyncOperationHandle? _settingsDataHandle;

        public SettingsBinder(SettingsModel settingsModel, Func<SettingsViewModel> viewModelFactory,
            IRootUIBinder rootUIBinder, SettingsView view) : base(viewModelFactory, rootUIBinder, view)
        {
            _settingsModel = settingsModel;
        }

        protected override SettingsViewModel GetViewModel()
        {
            var viewModel = base.GetViewModel();

            viewModel.Bind(_settingsModel);

            var settingsDataHandle = Addressables.LoadAssetAsync<SettingsDataSO>(Constants.SETTINGS_DATA_PATH);
            _settingsDataHandle = settingsDataHandle;
            settingsDataHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var settingsData = handle.Result;
                    viewModel.SetSettingsData(settingsData);
                }
            };

            return viewModel;
        }

        protected override void DisposeInstances()
        {
            _settingsDataHandle?.Release();
            base.DisposeInstances();
        }
    }
}