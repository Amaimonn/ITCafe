// using System;
// using ITCafe.Data.Campaign;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;
// using R3;
//
// using LostKaiju.Game.Providers.GameState;
// using LostKaiju.Game.Constants;
//
// namespace ITCafe.Data.Campaign
// {
//     public class CampaignModelFactory : ILoadableModelFactory<CampaignModel>
//     {
//         public Observable<CampaignModel> OnProduced => _onProduced;
//         private readonly IGameStateProvider _gameStateProvider;
//         private readonly Subject<CampaignModel> _onProduced = new();
//         private AsyncOperationHandle<AllLocationsDataSO> _handle;
//
//         public CampaignModelFactory(IGameStateProvider gameStateProvider)
//         {
//             _gameStateProvider = gameStateProvider;
//         }
//
//         public void GetModelOnLoaded(Action<CampaignModel> onLoaded)
//         {
//             _handle = Addressables.LoadAssetAsync<AllLocationsDataSO>(Paths.LOCATIONS_DATA);
//             _handle.Completed += (handler) =>
//             {
//                 var locationsDataSO = handler.Result;
//
//                 var campaignState = _gameStateProvider.Campaign;
//                 var campaignModel = new CampaignModel(campaignState, locationsDataSO);
//
//                 // check for new campaign data
//                 if (_gameStateProvider.Campaign.CampaignDataVersion != locationsDataSO.Version)
//                 {
//                     campaignModel.UpdateAvailableLocationsAndMissions();
//                     _gameStateProvider.Campaign.CampaignDataVersion = locationsDataSO.Version;
//                     _gameStateProvider.SaveCampaign(); // save new version
//                 }
//                 onLoaded(campaignModel);
//                 _onProduced.OnNext(campaignModel);
//             };
//         }
//
//         public void Release()
//         {
//             Addressables.Release(_handle);
//         }
//     }
// }