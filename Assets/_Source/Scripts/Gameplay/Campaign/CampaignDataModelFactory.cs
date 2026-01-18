using DevKit.Solutions;
using R3;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;

namespace ITCafe.Data.Campaign
{
    public class CampaignDataModelFactory : IFactory<CampaignDataModel>
    {
        private readonly ILocalizationLoader _campaignLocaleLoader;
        public ReadOnlyReactiveProperty<CampaignDataModel> CurrentInstance => _currentInstance;

        private readonly ReactiveProperty<CampaignDataModel> _currentInstance = new();
        private AsyncOperationHandle<AllLocationsDataSO> _locationsDataHandle;

        // There could be a cached model with loaded data (Create method will just increase addressables counter)

        public CampaignDataModelFactory(
            [Key(Constants.CAMPAIGN_DATA_LOCALE_LOADER)]
            ILocalizationLoader campaignLocaleLoader)
        {
            _campaignLocaleLoader = campaignLocaleLoader;
        }

        public CampaignDataModel Create()
        {
            var campaignDataModel = new CampaignDataModel();

            IAllLocationsData locationsDataSO = null;
            var isLocaleLoaded = false;

            _campaignLocaleLoader.Init();
            var localeObservable = _campaignLocaleLoader.LoadTablesObservable();
            localeObservable.Take(1).Subscribe(_ =>
            {
                isLocaleLoaded = true;
                if (locationsDataSO != null)
                    campaignDataModel.SetAllLocationsData(locationsDataSO);
            });

            _locationsDataHandle = Addressables.LoadAssetAsync<AllLocationsDataSO>(Constants.ALL_LOCATIONS_DATA_PATH);
            _locationsDataHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    locationsDataSO = handle.Result;
                    if (isLocaleLoaded)
                        campaignDataModel.SetAllLocationsData(locationsDataSO);
                }
                else
                {
                    campaignDataModel.SetAllLocationsData(null);
                }
            };
            _currentInstance.Value = campaignDataModel;

            return campaignDataModel;
        }

        public void Release()
        {
            Addressables.Release(_locationsDataHandle);
            _campaignLocaleLoader.Dispose();
        }
    }
}