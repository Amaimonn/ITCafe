using System;
using System.Collections.Generic;
using DevKit.UI.MVVM.Bases;
using DevKit.Utils;
using ITCafe.Campaign;
using ITCafe.Data.Campaign;
using ObservableCollections;
using R3;
using VContainer;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class CampaignViewModel : ScreenViewModel
    {
        public Observable<IReadOnlyDictionary<string, ILocationData>> LocationsDataMap => _locationsDataMap;
        public ReadOnlyReactiveProperty<ILocationData> SelectedLocationData => _selectedLocationData;
        public ReadOnlyReactiveProperty<IMissionData> SelectedMissionData => _selectedMissionData;
        public Observable<IReadOnlyList<IMissionData>> CurrentMissionsData => _currentMissionsData;
        public IReadOnlyObservableDictionary<string, LocationModel> OpenedLocationsMap => _openedLocationsMap;
        public IReadOnlyObservableDictionary<string, MissionModel> OpenedMissionsMap => _openedMissionsMap;

        private readonly CampaignDataLoader _campaignDataLoader;
        private readonly Subject<Unit> _startMissionSubject;

        private CampaignModel _campaignModel;
        private CampaignDataModel _campaignDataModel;

        private readonly ReactiveProperty<ILocationData> _selectedLocationData = new();
        private readonly ReactiveProperty<IMissionData> _selectedMissionData = new();
        private readonly ReactiveProperty<IReadOnlyList<IMissionData>> _currentMissionsData = new();
        private readonly ObservableDictionary<string, MissionModel> _openedMissionsMap = new();
        private readonly ObservableDictionary<string, LocationModel> _openedLocationsMap = new();
        private readonly ReactiveProperty<IReadOnlyDictionary<string, ILocationData>> _locationsDataMap = new();
        private CompositeDisposable _disposables;

        public CampaignViewModel(CampaignDataLoader campaignDataLoader,
            [Key(Constants.START_MISSION_SIGNAL)] Subject<Unit> startMissionSubject)
        {
            _campaignDataLoader = campaignDataLoader;
            _startMissionSubject = startMissionSubject;
        }

        public void Bind(CampaignModel campaignModel, CampaignDataModel campaignDataModel)
        {
            _disposables = new();

            _campaignModel = campaignModel;
            BindOpenedLocations(_campaignModel.OpenedLocationsMap);

            _campaignDataModel = campaignDataModel;
            _campaignDataModel.IsLoaded
                .Where(x => x == true)
                .Take(1)
                .Subscribe(_ => BindDataOnLoaded())
                .AddTo(_disposables);
        }

        public void StartGameplay()
        {
            _startMissionSubject.OnNext(Unit.Default);
        }

        public void SelectLocation(ILocationData locationData)
        {
            _campaignModel.SelectedLocationId.Value = locationData?.Id;
            _campaignModel.SelectedMissionId.Value = null;
        }

        public void SelectMission(IMissionData missionData)
        {
            _campaignModel.SelectedMissionId.Value = missionData?.Id;
        }

        /// <summary>
        /// Binds Campaign opened locations/missions providing
        /// runtime locations/missions opening synchronization support.
        /// </summary>
        private void BindOpenedLocations(IDictionary<string, LocationModel> locationsMap)
        {
            foreach (var idLocationPair in locationsMap)
                BindLocation(idLocationPair.Value);

            _campaignModel.OpenedLocationsMap.ObserveAdd()
                .Subscribe(x => BindLocation(x.Value.Value))
                .AddTo(_disposables);

            return;


            void BindLocation(LocationModel locationModel)
            {
                _openedLocationsMap.Add(locationModel.State.Id, locationModel);

                foreach (var idMissionPair in locationModel.OpenedMissionsMap)
                    _openedMissionsMap.Add(idMissionPair.Key, idMissionPair.Value);

                locationModel.OpenedMissionsMap.ObserveAdd()
                    .Subscribe(x => _openedMissionsMap.Add(x.Value.Key, x.Value.Value))
                    .AddTo(_disposables);
            }
        }

        private void BindDataOnLoaded()
        {
            FLogger.Log<CampaignViewModel>("Campaign Data Binding");

            _campaignDataModel.LocationsDataMap.Subscribe(x => _locationsDataMap.Value = x)
                .AddTo(_disposables);

            _campaignDataModel.SelectedLocationData.Subscribe(x => _selectedLocationData.Value = x)
                .AddTo(_disposables);

            _campaignDataModel.CurrentMissionsData.Subscribe(x =>
            {
                _currentMissionsData.Value = x ?? Array.Empty<IMissionData>();
                _campaignDataLoader.SelectMissionAsync(_campaignDataModel, _campaignModel.SelectedMissionId.CurrentValue)
                    .Forget();
            }).AddTo(_disposables);

            _campaignDataModel.SelectedMissionData.Subscribe(x => _selectedMissionData.Value = x)
                .AddTo(_disposables);


            _campaignModel.SelectedLocationId.Subscribe(x =>
                    _campaignDataLoader.SelectLocationAsync(_campaignDataModel, x).Forget())
                .AddTo(_disposables);

            _campaignModel.SelectedMissionId.Subscribe(x =>
                    _campaignDataLoader.SelectMissionAsync(_campaignDataModel, x).Forget())
                .AddTo(_disposables); // no data selected on init. Sub after SelectLocationAsync finoshed mb
        }

        public override void Dispose()
        {
            Disposes.ClearDispose(ref _disposables);
            _campaignDataLoader.UnloadAll(_campaignDataModel);
        }
    }
}