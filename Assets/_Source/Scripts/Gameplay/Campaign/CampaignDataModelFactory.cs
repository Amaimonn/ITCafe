using DevKit.Solutions;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ITCafe.Data.Campaign
{
    public class CampaignDataModelFactory : IFactory<CampaignDataModel>
    {
        private AsyncOperationHandle<AllLocationsDataSO> _locationsDataHandle;
        
        // There could be a cached model with loaded data (Create method will just increase addressables counter)
        
        public CampaignDataModel Create()
        {
            var campaignDataModel = new CampaignDataModel();
            
            _locationsDataHandle = Addressables.LoadAssetAsync<AllLocationsDataSO>(Constants.ALL_LOCATIONS_DATA_PATH);
            _locationsDataHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var locationsDataSO = handle.Result;
                    campaignDataModel.SetAllLocationsData(locationsDataSO);
                }
                else
                {
                    campaignDataModel.SetAllLocationsData(null);
                }
                
                // if (campaignState.CampaignDataVersion != locationsDataSO.Version) // TODO: move to bootstrap init
                // {
                //     _campaignUnlocker.MigrateCampaign(campaignModel);
                //     campaignState.CampaignDataVersion = locationsDataSO.Version;
                //     _gameStateProvider.SaveAll(); // save new version
                // }
            };
            
            return campaignDataModel;
        }

        public void Release()
        {
            Addressables.Release(_locationsDataHandle);
        }
    }
}