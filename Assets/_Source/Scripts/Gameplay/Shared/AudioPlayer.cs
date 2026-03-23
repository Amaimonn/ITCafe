using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DevKit.Utils;
using R3;
using UnityEngine.Pool;

namespace ITCafe.Gameplay.Shared
{
    public class AudioPlayer : MonoBehaviour
    {
        public readonly ReactiveProperty<float> VolumeMultiplier = new(1.0f);

        [SerializeField] private AudioSource _singletonMusicSource;
        [SerializeField] private AudioSource _singletonSfxSource;
        [SerializeField] private SfxSource _sfxSourcePrefab;
        [SerializeField] private bool _collectionCheck = true;
        [SerializeField] private int _defaultCapacity = 10;
        [SerializeField] private int _maxPoolSize = 100;

        private IObjectPool<SfxSource> _sfxSourcePool;
        private readonly ReactiveProperty<float> _audioVolumeFadeScale = new(1.0f);
        private readonly MonoBehaviourHook _monoHook;
        private bool _isMusicFading = false;

        private readonly List<SfxSource> _activeSfxSources = new();
        private Observable<float> _musicVolumeSetting;
        private Observable<float> _sfxVolumeSetting;

        public void Init(Observable<float> musicVolume, Observable<float> sfxVolume)
        {
            _musicVolumeSetting = musicVolume;
            _sfxVolumeSetting = sfxVolume;

            _sfxSourcePool = new ObjectPool<SfxSource>(
                CreateSfxSource,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                _collectionCheck,
                _defaultCapacity,
                _maxPoolSize);

            _musicVolumeSetting.CombineLatest(_audioVolumeFadeScale, VolumeMultiplier, (a, b, c) => a * b * c)
                .Subscribe(x => _singletonMusicSource.volume = x);

            _sfxVolumeSetting.CombineLatest(_audioVolumeFadeScale, VolumeMultiplier, (a, b, c) => a * b * c)
                .Subscribe(x => _singletonSfxSource.volume = x);
        }

        public SfxSource Get()
        {
            return _sfxSourcePool.Get();
        }

        public void Release(SfxSource soundEmitter)
        {
            _sfxSourcePool.Release(soundEmitter);
        }

        public void StopAllSfx()
        {
            var tempList = new List<SfxSource>(_activeSfxSources);

            foreach (var soundEmitter in tempList)
                soundEmitter.Stop();
        }

        public void PlaySingletonMusic(AudioClip musicClip, bool loop = true)
        {
            _singletonMusicSource.clip = musicClip;
            _singletonMusicSource.loop = loop;
            _singletonMusicSource.Play();
        }

        public SfxBuilder GetSfxBuilder()
        {
            return new SfxBuilder(this);
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
                    _audioVolumeFadeScale.Value += Mathf.Lerp(0, 1, currentDuration / duration);

                    yield return null;

                    currentDuration += Time.deltaTime;
                }

                _audioVolumeFadeScale.Value = 1;
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

        public void PauseMusic()
        {
            _singletonMusicSource.Pause();
        }

        public void UnPauseMusic()
        {
            _singletonMusicSource.UnPause();
        }

        private SfxSource CreateSfxSource()
        {
            var sfxSource = Instantiate(_sfxSourcePrefab, transform, true);
            
            // TODO: bind pause

            _sfxVolumeSetting.CombineLatest(_audioVolumeFadeScale, VolumeMultiplier, (a, b, c) => a * b * c)
                .Subscribe(x => sfxSource.AudioSource.volume = x * sfxSource.Data.VolumeScale)
                .AddTo(sfxSource.gameObject);

            sfxSource.gameObject.SetActive(false);

            return sfxSource;
        }

        private void OnTakeFromPool(SfxSource soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            _activeSfxSources.Add(soundEmitter);
        }

        private void OnReturnedToPool(SfxSource soundEmitter)
        {
            soundEmitter.gameObject.SetActive(false);
            _activeSfxSources.Remove(soundEmitter);
        }

        private void OnDestroyPoolObject(SfxSource soundEmitter)
        {
            Destroy(soundEmitter.gameObject);
        }
    }
}