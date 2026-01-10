// https://github.com/workavast/Blame-Game 2025 Rodion

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace ITCafe
{
    [Serializable]
    public class LocalizationLoader : ILocalizationLoader
    {
        [field: SerializeField] private StringTablesConfig _tablesConfig;

        private Locale _locale;

        public void Init()
        {
            LocalizationSettings.SelectedLocaleChanged += UnloadTables;
        }

        public async UniTaskVoid LoadTables()
        {
            _locale = LocalizationSettings.SelectedLocale;
            var handle = LocalizationSettings.StringDatabase.PreloadTables(_tablesConfig.TableReferences,
                _locale);

            await handle.Task;
        }

        private void Release()
        {
            foreach (var table in _tablesConfig.TableReferences)
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