using UnityEngine;
using System.Collections;
using DevKit.Utils;
using R3;

namespace ITCafe.Gameplay.Shared
{
    public class AudioPlayer
    {
        public readonly ReactiveProperty<float> VolumeMultiplier = new(1.0f);

        private readonly AudioSource _musicSource;
        private readonly AudioSource _oneShotSfxSource;
        private readonly NotNullPool<AudioSource> _sfxSourcePool;
        private readonly ReactiveProperty<float> _musicVolumeFadeScale = new(1.0f);
        private readonly ReactiveProperty<float> _sfxVolumeFadeScale = new(1.0f);
        private readonly MonoBehaviourHook _monoHook;
        private bool _isMusicFading = false;
        
        private CompositeDisposable _poolDisposables = new();

        public AudioPlayer(Observable<float> musicVolume, Observable<float> sfxVolume, MonoBehaviourHook monoHook)
        {
            _monoHook = monoHook;
            var audioPlayerObject = new GameObject("AudioPlayer");
            _musicSource = audioPlayerObject.AddComponent<AudioSource>();
            _oneShotSfxSource = audioPlayerObject.AddComponent<AudioSource>();
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
                .Subscribe(x => _musicSource.volume = x);
            sfxVolume.CombineLatest(_sfxVolumeFadeScale, VolumeMultiplier, (a, b, c) => a * b * c)
                .Subscribe(x => _oneShotSfxSource.volume = x);
        }
        
        public void PlayMusic(AudioClip musicClip, bool loop = true)
        {
            _musicSource.clip = musicClip;
            _musicSource.loop = loop;
            _musicSource.Play();
        }

        public void TurnOnSoundFade(float duration)
        {
            if (_musicSource.isPlaying)
            {
                if (_isMusicFading)
                {
                    _monoHook.StopCoroutine(FadeInRoutine());
                }

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
        public void PlayOneShotSFX(AudioClip clip, float volumeScale = 1.0f)
        {
            _oneShotSfxSource.PlayOneShot(clip, volumeScale);
        }

        /// <summary>
        /// SFX will stop playing automatically if scene is unloaded. Extends the pool
        /// </summary>
        public void PlaySFX(AudioClip clip, float pitch = 1.0f)
        {
            var source = _sfxSourcePool.Get();
            source.pitch = pitch;
            source.clip = clip;
            Observable.Timer(System.TimeSpan.FromSeconds(clip.length))
                .Take(1)
                .Subscribe(x => _sfxSourcePool.Release(source))
                .AddTo(_poolDisposables);
            
            source.Play();
        }

        /// <summary>
        /// Same as <see cref="PlaySFX"/> but with random pitch between 0.9 and 1.1. Also extends the pool
        /// </summary>
        public void PlayRandomPitchSFX(AudioClip clip)
        {
            var randomPitch = UnityEngine.Random.Range(0.9f, 1.1f);
            PlaySFX(clip, randomPitch);
        }

        public void ClearPoolSFX()
        {
            _poolDisposables.Dispose();
            _poolDisposables = new CompositeDisposable();
            _sfxSourcePool.Clear();
        }

        public void PauseMusic()
        {
            _musicSource.Pause();
        }

        public void UnPauseMusic()
        {
            _musicSource.UnPause();
        }
    }
}
