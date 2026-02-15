// https://github.com/workavast/Blame-Game 2025 Rodion

using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace ITCafe
{
    [Serializable]
    public class SerializableLocalizationLoader : ILocalizationLoader
    {
        [field: SerializeField] private LocaleTableCollectionSO _tableCollectionSO;

        private LocalizationLoadingService _localizationLoadingService;
        
        public void Init()
        {
            _localizationLoadingService = new LocalizationLoadingService();
            _localizationLoadingService.Init(_tableCollectionSO);
        }

        public UniTaskVoid LoadTables()
        {
            return _localizationLoadingService.LoadTables();
        }

        public Observable<Unit> LoadTablesObservable()
        {
            return _localizationLoadingService.LoadTablesObservable();
        }

        public void Dispose()
        {
            _localizationLoadingService.Dispose();
        }
    }
}