using System;
using DevKit.Saves;
using UnityEngine;

namespace ITCafe.Data.Settings
{
    [Serializable]
    public class SettingsState : IVersioned
    {
        public int Version
        {
            get => _version;
            set => _version = value;
        }

        // Sound settings
        [Range(0, 100)] public int MusicVolume = 80;
        [Range(0, 100)] public int SfxVolume = 80;
        
        // Input settings
        [Range(1, 100)] public int Sensitivity = 50;
        
        // Video settings
        public bool VSync = false;
        public int FPS = -1;
        [Range(0, 100)] public int Brightness = 50;
        public bool IsPostProcessingEnabled = true;
        public bool IsBloomEnabled = true;
        public bool IsFilmGrainEnabled = false;
        public bool IsAntiAliasingEnabled = true;
        // public int QualityPreset = 2;
        public int ScreenWidth;
        public int ScreenHeight;
        public bool Fullscreen = true;
        
        // Language settings
        public int LanguageIndex = 0;

        [SerializeField] private int _version = 1;
    }
}