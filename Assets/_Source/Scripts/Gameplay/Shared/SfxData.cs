using System;
using UnityEngine;

namespace ITCafe.Gameplay.Shared
{
    [Serializable]
    public class SfxData
    {
        public bool IsValid => AudioClip != null;
        
        [field: SerializeField] public AudioClip AudioClip { get; private set; }
        
        [field: SerializeField] public bool IsLoop { get; private set; } = false;
        
        [field: SerializeField, Range(0f, 1f)] public float VolumeScale { get; private set; } = 1f;
        
        [field: SerializeField, Range(0f, 1f)] public float SpacialBlend { get; private set; } = 1f;
        
        [field: SerializeField, Min(0)] public float Pitch { get; private set; } = 1f;
        
        [field: SerializeField] public bool PitchShift { get; private set; } = true;
        
        [field: SerializeField] public float MinPitchShift { get; private set; } = -0.1f;
        
        [field: SerializeField] public float MaxPitchShift { get; private set; } = 0.1f;
        
        [field: SerializeField] public bool IsPausable { get; private set; } = true;

        /// <summary>
        /// If set to true: random pitch is not applied. Sfx doesn`t stop if scene changes.
        /// TIP: use in UI.
        /// </summary>
        [field: SerializeField] public bool IsSingleton { get; private set; } = false;
    }
}