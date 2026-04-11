using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace ITCafe
{
    public class LocalizationLoadingService : IDisposable
    {
        private ILocaleTableCollection _tableCollection;
        private Locale _locale;
        
        public void Init(ILocaleTableCollection tableCollection)
        {
            _tableCollection =  tableCollection;
            LocalizationSettings.SelectedLocaleChanged += UnloadTables;
        }

        public async UniTaskVoid LoadTables()
        {
            _locale = LocalizationSettings.SelectedLocale;
            await LocalizationSettings.StringDatabase.PreloadTables(_tableCollection.TableReferences,
                _locale);
        }

        public Observable<Unit> LoadTablesObservable()
        {
            _locale = LocalizationSettings.SelectedLocale;
            var handle = LocalizationSettings.StringDatabase.PreloadTables(_tableCollection.TableReferences,
                _locale);

            if (handle.IsDone)
                return Observable.ReturnUnit();

            var completeSignal = new Subject<Unit>();
            handle.Completed += _ => completeSignal.OnNext(Unit.Default);

            return completeSignal;
        }

        private void Release()
        {
            foreach (var table in _tableCollection.TableReferences)
                LocalizationSettings.StringDatabase.ReleaseTable(table, _locale);
        }

        private void UnloadTables(Locale _)
        {
            Release();
            LoadTables().Forget();
        }

        public void Dispose()
        {
            LocalizationSettings.SelectedLocaleChanged -= UnloadTables;
            Release();
        }
    }
}