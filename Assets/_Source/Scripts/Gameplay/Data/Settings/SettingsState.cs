using System;
using DevKit.Saves;
using UnityEngine;

namespace ITCafe.Gameplay.Data
{
    [Serializable]
    public class SettingsState : IVersioned
    {
        public int Version
        {
            get => _version;
            set => _version = value;
        }

        public bool VSync;
        public int FPS;
        [Range(1, 100)] public int Sensitivity;

        [SerializeField] private int _version = 1;
    }
}