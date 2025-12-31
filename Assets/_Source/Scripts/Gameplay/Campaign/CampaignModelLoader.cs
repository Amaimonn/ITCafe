using System;
using ITCafe.Campaign;
using ITCafe.Infrastructure.Saves;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using R3;

namespace ITCafe.Data.Campaign
{
    public class CampaignModelLoader
    {
        public Observable<CampaignModel> OnProduced => _onProduced;
        
        private readonly ISaveStateProvider _gameStateProvider;
        private readonly CampaignUnlocker _campaignUnlocker;
        
        private readonly Subject<CampaignModel> _onProduced = new();
        private AsyncOperationHandle<AllLocationsDataSO> _locationsDataHandle;

        public CampaignModelLoader(ISaveStateProvider gameStateProvider, CampaignUnlocker campaignUnlocker)
        {
            _gameStateProvider = gameStateProvider;
            _campaignUnlocker = campaignUnlocker;
        }

        public void LoadModel(Action<CampaignModel> onLoaded)
        {
            _locationsDataHandle = Addressables.LoadAssetAsync<AllLocationsDataSO>(Constants.ALL_LOCATIONS_DATA_PATH);
            _locationsDataHandle.Completed += (handle) =>
            {
                var locationsDataSO = handle.Result;

                var campaignState = _gameStateProvider.SaveState.CampaignState;
                var campaignModel = new CampaignModel(campaignState);
                campaignModel.SetAllLocationsData(locationsDataSO);

                if (campaignState.CampaignDataVersion != locationsDataSO.Version)
                {
                    _campaignUnlocker.MigrateCampaign(campaignModel);
                    campaignState.CampaignDataVersion = locationsDataSO.Version;
                    _gameStateProvider.SaveAll(); // save new version
                }
                onLoaded(campaignModel);
                _onProduced.OnNext(campaignModel);
            };
        }

        public void Release()
        {
            Addressables.Release(_locationsDataHandle);
        }
    }
}