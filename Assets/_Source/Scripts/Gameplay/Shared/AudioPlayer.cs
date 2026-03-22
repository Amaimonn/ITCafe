using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DevKit.Utils;
using R3;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace ITCafe.Gameplay.Shared
{
    public class AudioPlayer
    {
        public readonly ReactiveProperty<float> VolumeMultiplier = new(1.0f);

        private readonly AudioSource _singletonMusicSource;
        private readonly AudioSource _singletonSfxSource;
        private readonly NotNullPool<AudioSource> _sfxSourcePool;
        private readonly ReactiveProperty<float> _musicVolumeFadeScale = new(1.0f);
        private readonly ReactiveProperty<float> _sfxVolumeFadeScale = new(1.0f);
        private readonly MonoBehaviourHook _monoHook;
        private bool _isMusicFading = false;

        private CompositeDisposable _poolDisposables = new();
        private Dictionary<Observable<Unit>, IDisposable> _activeTimers = new();

        public AudioPlayer(Observable<float> musicVolume, Observable<float> sfxVolume, MonoBehaviourHook monoHook)
        {
            _monoHook = monoHook;

            var audioPlayerObject = new GameObject("AudioPlayer");

            _singletonMusicSource = audioPlayerObject.AddComponent<AudioSource>();
            _singletonSfxSource = audioPlayerObject.AddComponent<AudioSource>();
            Object.DontDestroyOnLoad(audioPlayerObject);

            _sfxSourcePool = new NotNullPool<AudioSource>
            (
                createFunc: () =>
                {
                    var sfxObject = new GameObject("AudioSourceSFX");
                    sfxObject.transform.SetParent(audioPlayerObject.transform);

                    var audioSource = sfxObject.AddComponent<AudioSource>();
                    audioSource.playOnAwake = false;

                    sfxVolume.CombineLatest(_sfxVolumeFadeScale, VolumeMultiplier, (a, b, c) => a * b * c)
                        .Subscribe(x => audioSource.volume = x)
                        .AddTo(_poolDisposables);

                    return audioSource;
                },
                onGet: x => x.gameObject.SetActive(true),
                onRelease: x => x.gameObject.SetActive(false),
                onClear: x => Object.Destroy(x.gameObject),
                checkNotNull: x => x != null && x.gameObject != null
            );

            musicVolume.CombineLatest(_sfxVolumeFadeScale, VolumeMultiplier, (a, b, c) => a * b * c)
                .Subscribe(x => _singletonMusicSource.volume = x);

            sfxVolume.CombineLatest(_sfxVolumeFadeScale, VolumeMultiplier, (a, b, c) => a * b * c)
                .Subscribe(x => _singletonSfxSource.volume = x);
        }

        public void PlaySingletonMusic(AudioClip musicClip, bool loop = true)
        {
            _singletonMusicSource.clip = musicClip;
            _singletonMusicSource.loop = loop;
            _singletonMusicSource.Play();
        }

        public void TurnOnSoundFade(float duration)
        {
            if (_singletonMusicSource.isPlaying)
            {
                if (_isMusicFading)
                    _monoHook.StopCoroutine(FadeInRoutine());

                _monoHook.StartCoroutine(FadeInRoutine());
            }

            IEnumerator FadeInRoutine()
            {
                _isMusicFading = true;
                var currentDuration = 0.0f;

                while (currentDuration < duration)
                {
                    _musicVolumeFadeScale.Value += Mathf.Lerp(0, 1, currentDuration / duration);

                    yield return null;

                    currentDuration += Time.deltaTime;
                }

                _musicVolumeFadeScale.Value = 1;
                _isMusicFading = false;
            }
        }

        /// <summary>
        /// SFX will continue playing even if scene is unloaded until it`s source will be stopped by script.
        /// No pooling.
        /// </summary>
        public void PlayOneShotSingletonSFX(AudioClip clip, float volumeScale = 1.0f)
        {
            _singletonSfxSource.PlayOneShot(clip, volumeScale);
        }

        public IDisposable StartLoopedSfx(AudioClip clip, float pitch = 1.0f, Vector3? sfxPosition = null)
        {
            var sfx = GetAudioSource(clip, out var disposable, pitch, sfxPosition, loop: true);
            sfx.Play();

            return disposable;
        }

        /// <summary>
        /// SFX will stop playing automatically if scene is unloaded. Extends the pool
        /// </summary>
        public void PlaySfx(AudioClip clip, float pitch = 1.0f, Vector3? sfxPosition = null)
        {
            var sfx = GetAudioSource(clip, out _, pitch, sfxPosition);
            sfx.Play();
        }

        /// <summary>
        /// Same as <see cref="PlaySfx"/> but with random pitch between 0.9 and 1.1. Also extends the pool
        /// </summary>
        public void PlayRandomPitchSfx(AudioClip clip, float minPitch = 0.9f, float maxPitch = 1.1f,
            Vector3? sfxPosition = null)
        {
            var randomPitch = Random.Range(minPitch, maxPitch);
            PlaySfx(clip, randomPitch, sfxPosition);
        }

        public void ClearPoolSfx()
        {
            _poolDisposables.Dispose();
            _poolDisposables = new CompositeDisposable();

            foreach (var timer in _activeTimers.Values)
                timer?.Dispose();

            _sfxSourcePool.Clear();
        }

        public void PauseMusic()
        {
            _singletonMusicSource.Pause();
        }

        public void UnPauseMusic()
        {
            _singletonMusicSource.UnPause();
        }

        private AudioSource GetAudioSource(AudioClip clip, out IDisposable disposable, float pitch = 1.0f,
            Vector3? sfxPosition = null, bool loop = false)
        {
            var sfx = _sfxSourcePool.Get();

            if (sfxPosition != null)
            {
                sfx.spatialBlend = 1.0f;
                sfx.transform.position = sfxPosition.Value;
            }
            else
            {
                sfx.transform.position = Vector3.zero;
                sfx.spatialBlend = 0.0f;
            }

            sfx.pitch = pitch;
            sfx.clip = clip;
            sfx.loop = loop;

            if (!loop)
            {
                var timer = Observable.Timer(TimeSpan.FromSeconds(clip.length));
                var disposed = false;
                disposable = timer.Take(1).Subscribe(_ =>
                {
                    _sfxSourcePool.Release(sfx);
                    _activeTimers.Remove(timer);
                    disposed = true;
                });

                if (!disposed) // zero length clip guard
                    _activeTimers[timer] = disposable;
            }
            else
            {
                disposable = Disposable.Create(() => _sfxSourcePool.Release(sfx));
            }

            return sfx;
        }
    }
}