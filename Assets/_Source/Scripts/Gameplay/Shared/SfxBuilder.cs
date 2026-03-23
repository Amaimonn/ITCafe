using System;
using DevKit.Utils;
using R3;
using UnityEngine;

namespace ITCafe.Gameplay.Shared
{
    public struct SfxBuilder
    {
        private readonly AudioPlayer _audioPlayer;
        private Vector3 _position;

        public SfxBuilder(AudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;
            _position = Vector3.zero;
        }

        public SfxBuilder WithPosition(Vector3 position)
        {
            _position = position;
            return this;
        }

        public void Play(SfxData sfxData)
        {
            if (sfxData.AudioClip == null)
            {
                FLogger.LogError<SfxBuilder>("AudioClip is null");
                return;
            }

            if (sfxData.IsSingleton)
            {
                _audioPlayer.PlayOneShotSingletonSFX(sfxData.AudioClip, sfxData.VolumeScale);
                return;
            }

            var sfxSource = _audioPlayer.Get();
            sfxSource.Init(sfxData, _audioPlayer);
            sfxSource.transform.position = _position;

            sfxSource.Play();
        }
        
        public bool TryPlayWithCancellation(SfxData sfxData, out IDisposable disposable)
        {
            if (sfxData.AudioClip == null)
            {
                FLogger.LogError<SfxBuilder>("AudioClip is null");
                disposable = null;
                
                return false;
            }

            if (sfxData.IsSingleton)
            {
                _audioPlayer.PlayOneShotSingletonSFX(sfxData.AudioClip, sfxData.VolumeScale);
                disposable = null;
                
                return false;
            }

            var sfxSource = _audioPlayer.Get();
            sfxSource.Init(sfxData, _audioPlayer);
            sfxSource.transform.position = _position;
            
            var isDisposed = false;
            
            sfxSource.OnDisposed.Take(1)
                .Subscribe(_ => isDisposed = true);
            
            disposable = Disposable.Create(() =>
            {
                if (!isDisposed)
                {
                    sfxSource.Stop();
                    isDisposed = true;
                }
            });
            
            sfxSource.Play();
            
            return true;
        }
    }
}