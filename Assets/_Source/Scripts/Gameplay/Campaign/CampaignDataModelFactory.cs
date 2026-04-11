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
        private AsyncOperationHandle<LocationDataCollectionSO> _locationsDataHandle;

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

            ILocationDataCollection locationsDataCollectionSO = null;
            var isLocaleLoaded = false;

            _campaignLocaleLoader.Init();
            var localeObservable = _campaignLocaleLoader.LoadTablesObservable();
            localeObservable.Take(1).Subscribe(_ =>
            {
                isLocaleLoaded = true;
                if (locationsDataCollectionSO != null)
                    campaignDataModel.SetAllLocationsData(locationsDataCollectionSO);
            });

            _locationsDataHandle = Addressables.LoadAssetAsync<LocationDataCollectionSO>(Constants.ALL_LOCATIONS_DATA_PATH);
            _locationsDataHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    locationsDataCollectionSO = handle.Result;
                    if (isLocaleLoaded)
                        campaignDataModel.SetAllLocationsData(locationsDataCollectionSO);
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