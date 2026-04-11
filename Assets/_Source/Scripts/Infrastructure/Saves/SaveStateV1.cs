// using DevKit.Saves;
// using ITCafe.Data.Settings;
// using ITCafe.Data.Campaign;
// using UnityEngine;
//
// namespace ITCafe.Infrastructure.Saves
// {
//     public class SaveStateV1 : SaveStateBase, ISaveState
//     {
//         public override int Version
//         {
//             get => _version;
//             set => _version = value;
//         }
//
//         public SettingsState SettingsState
//         {
//             get => _settingsState;
//             set => _settingsState = value;
//         }
//         
//         // For JsonUtility serialization
//         [SerializeField] private int _version = 1;
//         [SerializeField] private SettingsState _settingsState;
//     }
// }