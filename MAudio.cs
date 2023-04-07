using System;
using System.Linq;
using _Configs;
using UnityEngine;
using Zenject;

namespace _Managers
{
    public class MAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _soundSource;
        [SerializeField] private AudioSource _loopedSoundSource;

        private MData       _data;
        private AudioConfig _config;

        private DateTime _lastTapticTime;
        private float    _minTapticInterval = 0.1f;

        [Inject]
        private void Construct(MData data, CoreConfig config)
        {
            _data = data;
            _config = config.audio;
        }

        private async void OnEnable()
        {
            _data.SoundChanged += MuteAudio;
        }

        private void OnDisable()
        {
            _data.SoundChanged -= MuteAudio;
            PlayMusic(_config.music);
        }

        public void PlayVibro(VibroEvent type, bool force = false)
        {
            try
            {
                var vibro = _config.vibro.First(c => c.type == type);
                PlayVibro(vibro.impact, force);
            }
            catch (InvalidOperationException)
            {
                Debug.Log($"No vibro fx for type {type}");
            }
        }

        public void PlaySound(AudioEvent type)
        {
            try
            {
                var sound = _config.sounds.First(c => c.type == type);
                PlaySound(sound);
            }
            catch (InvalidOperationException)
            {
                Debug.Log($"No audio fx for type {type}");
            }
        }

        public void PlayLoopedSound(AudioEvent soundType)
        {
            var audioFX = _config.sounds.First(c => c.type == soundType);
            PlayLoopedSound(audioFX);
        }

        private void PlaySound(AudioConfig.SoundSettings sound)
        {
            if (!sound.clip || !_data.SoundEnabled) return;
            _soundSource.PlayOneShot(sound.clip, sound.volume);
        }

        private void PlayLoopedSound(AudioConfig.SoundSettings sound)
        {
            if (!sound.clip || !_data.SoundEnabled) return;

            if (sound.clip != _loopedSoundSource.clip)
            {
                _loopedSoundSource.clip = sound.clip;
                _loopedSoundSource.volume = sound.volume;
                _loopedSoundSource.loop = true;
            }

            _loopedSoundSource.Play();
        }

        private void PlayMusic(AudioConfig.SoundSettings musicClip)
        {
            if (!musicClip.clip || !_data.SoundEnabled) return;

            if (musicClip.clip != _musicSource.clip)
            {
                _musicSource.clip = musicClip.clip;
                _musicSource.volume = musicClip.volume;
                _musicSource.loop = true;
            }

            _musicSource.Play();
        }
        
        private void PlayVibro(TapticImpact type, bool force)
        {
            if (!_data.HapticEnabled) return;

            if ((DateTime.Now - _lastTapticTime).TotalSeconds >= _minTapticInterval || force)
            {
                _lastTapticTime = DateTime.Now;
                TapticManager.Impact(type);
            }
        }

        private void MuteAudio(bool state)
        {
            _loopedSoundSource.mute = !state;
            _soundSource.mute = !state;
            _musicSource.mute = !state;
        }
    }
    
}