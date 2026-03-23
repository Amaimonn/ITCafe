using System.Collections;
using R3;
using UnityEngine;

namespace ITCafe.Gameplay.Shared
{
    [RequireComponent(typeof(AudioSource))]
    public class SfxSource : MonoBehaviour
    {
        public Observable<Unit> OnDisposed => _onDisposed;
        public SfxData Data { get; private set; }

        [field: SerializeField] public AudioSource AudioSource { get; private set; }

        private Coroutine _playingCoroutine;
        private AudioPlayer _audioPlayer;
        private readonly Subject<Unit> _onDisposed = new();
        private float _volumeMultiplier = 1.0f;
        
        public void Init(SfxData data, AudioPlayer audioPlayer)
        {
            Data = data;
            _audioPlayer = audioPlayer;

            AudioSource.clip = data.AudioClip;
            AudioSource.loop = data.IsLoop;

            AudioSource.volume = data.VolumeScale * _volumeMultiplier;
            AudioSource.pitch = data.Pitch;
            AudioSource.spatialBlend = data.SpacialBlend;

            if (data.PitchShift)
                AudioSource.pitch += Random.Range(data.MinPitchShift, data.MaxPitchShift);
        }

        public void SetVolumeMultiplier(float volumeScale)
        {
            _volumeMultiplier = volumeScale;
            if (Data != null)
                AudioSource.volume = volumeScale * Data.VolumeScale;
        }

        public void Play()
        {
            if (_playingCoroutine != null)
                StopCoroutine(_playingCoroutine);

            AudioSource.Play();
            _playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }

        IEnumerator WaitForSoundToEnd()
        {
            yield return new WaitWhile(() => AudioSource.isPlaying);

            Stop();
        }

        public void Stop()
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
                _playingCoroutine = null;
            }

            AudioSource.Stop();
            _audioPlayer.Release(this);
            _onDisposed.OnNext(Unit.Default);
        }
    }
}