using System;
using DevKit.Utils;
using R3;
using UnityEngine;

namespace ITCafe.Gameplay.Shared
{
    public struct SfxBuilder
    {
        private readonly AudioPlayer _audioPlayer;
        private Vector3? _position;

        public SfxBuilder(AudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;
            _position = null;
        }

        public SfxBuilder WithPosition(Vector3 position)
        {
            _position = position;
            return this;
        }

        public void Play(SfxData sfxData)
        {
            if (!TryContinueWithSfxSource(sfxData, out var sfxSource))
                return;

            sfxSource.Play();
        }

        public bool TryPlayWithCancellation(SfxData sfxData, out IDisposable disposable)
        {
            if (!TryContinueWithSfxSource(sfxData, out var sfxSource))
            {
                disposable = null;
                return false;
            }
            
            var isDisposed = false;

            sfxSource.OnDisposed.Take(1)
                .Subscribe(_ => isDisposed = true);

            disposable = Disposable.Create(() =>
            {
                if (isDisposed)
                    return;
                
                sfxSource.Stop();
                isDisposed = true;
            });

            sfxSource.Play();

            return true;
        }

        private bool TryContinueWithSfxSource(SfxData sfxData, out SfxSource sfxSource)
        {
            if (!sfxData.IsValid)
            {
                FLogger.LogError<SfxBuilder>("sfxData is invalid");
                sfxSource = null;
                
                return false;
            }

            if (sfxData.IsSingleton)
            {
                _audioPlayer.PlayOneShotSingletonSFX(sfxData.AudioClip, sfxData.VolumeScale);
                sfxSource = null;
                
                return false;
            }

            sfxSource = _audioPlayer.Get();
            sfxSource.Init(sfxData, _audioPlayer);

            if (_position.HasValue)
                sfxSource.transform.position = _position.Value;
            else
                sfxSource.AudioSource.spatialBlend = 0.0f;

            return true;
        }
    }
}