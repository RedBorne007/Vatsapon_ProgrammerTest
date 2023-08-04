//------------------------------------------------------------------
//
//                         Audio System
//                  by Vatsapon Asawakittiporn
//
// Note: This class use for handling Audio system and data.
//
//------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public enum AudioCategory
    {
        Master, Music, Sound
    }

    [Header("Settings")]
    [Tooltip("Speed to fade-in audio")]
    [SerializeField] private float fadeInSpeed = 1f;
    [Tooltip("Speed to fade-out audio")]
    [SerializeField] private float fadeOutSpeed = 1f;
    [Tooltip("Minimum volume for Audio Mixer (-80 is default, change if necessary)")]
    [SerializeField] private float mixerMinVolume = -80f;

    [Header("References")]
    [SerializeField] private AudioMixerGroup mixerMaster;
    [SerializeField] private AudioMixerGroup mixerBGM;
    [SerializeField] private AudioMixerGroup mixerSFX;

    [Space]

    [SerializeField] private AudioSource sourceBGM;
    [SerializeField] private AudioSource sourceSFX;

    [SerializeField] private float masterVolume;
    [SerializeField] private float musicVolume;
    [SerializeField] private float soundVolume;

    private bool isFading = false;
    private CancellationTokenSource cancelTokenSource;
    private AudioProfile currentMusic;

    [SerializeField] private List<AudioProfile> audioProfiles;

    public float MasterVolume => masterVolume;
    public float MusicVolume => musicVolume;
    public float SoundVolume => soundVolume;
    public List<AudioProfile> Profiles { get { return audioProfiles; } set { audioProfiles = value; } }
    public AudioProfile CurrentMusic => currentMusic;

    private const string MASTER_VOL = "masterVol";
    private const string MUSIC_VOL = "musicVol";
    private const string SFX_VOL = "soundVol";

    private const float MIXER_MAX_VOL = 0f;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadData();

        cancelTokenSource = new CancellationTokenSource();
    }

    private void OnEnable()
    {
        LoadData();
    }

    private void OnDisable()
    {
        try
        {
            cancelTokenSource?.Cancel();
        }
        catch (ObjectDisposedException) { }

        SaveData();
    }

    private void Update()
    {
        mixerMaster.audioMixer.SetFloat(MASTER_VOL, ConvertToMixerVolume(masterVolume));
        mixerBGM.audioMixer.SetFloat(MUSIC_VOL, ConvertToMixerVolume(musicVolume));
        mixerSFX.audioMixer.SetFloat(SFX_VOL, ConvertToMixerVolume(soundVolume));
    }

    // Function to set master volume.
    public void SetMasterVolume(float value) => SetVolume(AudioCategory.Master, value);

    // Function to set BGM volume.
    public void SetBGMVolume(float value) => SetVolume(AudioCategory.Music, value);

    // Function to set SFX volume.
    public void SetSFXVolume(float value) => SetVolume(AudioCategory.Sound, value);

    // Function to set volume based on category.
    public void SetVolume(AudioCategory audioCategory, float value)
    {
        value = Mathf.Clamp(value, 0f, 100f);

        switch (audioCategory)
        {
            case AudioCategory.Master:
            masterVolume = value;
            break;

            case AudioCategory.Music:
            musicVolume = value;
            break;

            case AudioCategory.Sound:
            soundVolume = value;
            break;
        }
    }

    // Function to play SFX.
    public void PlaySFX(string audioName) => PlaySFX(sourceSFX, audioName);

    // Function to play SFX.
    public void PlaySFX(AudioSource source, string audioName)
    {
        AudioProfile audio = GetAudio(audioName, false);
        PlaySFX(source, audio);
    }

    // Function to play random SFX.
    public void PlayRandomSFX(string audioName)
    {
        AudioProfile audio = GetAudio(audioName, true);
        PlaySFX(audio);
    }

    // Function to play SFX (for specific audio).
    public void PlaySFX(AudioProfile audio) => PlaySFX(sourceSFX, audio);

    // Function to play SFX (for specific audio and source).
    public void PlaySFX(AudioSource source, AudioProfile audio)
    {
        if (audio == null)
        {
            return;
        }

        source.volume = audio.Volume / 100f;
        source.pitch = audio.Pitch;
        source.PlayOneShot(audio.Clip);
    }

    // Function to play music without fade-in.
    public void PlayMusic(string audioName)
    {
        AudioProfile audio = GetAudio(audioName, false);
        PlayMusic(audio);
    }

    // Function to play random music without fade-in.
    public void PlayRandomMusic(string audioName)
    {
        AudioProfile audio = GetAudio(audioName, true);
        PlayMusic(audio);
    }

    // Function to play music (for specific audio).
    public void PlayMusic(AudioProfile audio)
    {
        if (audio == null)
        {
            return;
        }

        if (isFading)
        {
            try
            {
                cancelTokenSource?.Cancel();
            }
            catch (ObjectDisposedException) { }

            cancelTokenSource = new CancellationTokenSource();

            isFading = false;
        }

        if (sourceBGM.isPlaying)
        {
            sourceBGM.Stop();
        }

        sourceBGM.clip = audio.Clip;
        sourceBGM.volume = audio.Volume / 100f;
        sourceBGM.pitch = audio.Pitch;
        sourceBGM.Play();

        currentMusic = audio;
    }

    // Function to stop music without fade-out.
    public void StopMusic()
    {
        if (isFading)
        {
            try
            {
                cancelTokenSource?.Cancel();
            }
            catch (ObjectDisposedException) { }

            cancelTokenSource = new CancellationTokenSource();

            isFading = false;
        }

        sourceBGM.Stop();
        currentMusic = null;
    }

    // Function to fade-in music.
    public void FadeInMusic(string audioName)
    {
        AudioProfile audio = GetAudio(audioName, false);
        FadeInMusic(audio);
    }

    // Function to fade-in random music.
    public void FadeInRandomMusic(string audioName)
    {
        AudioProfile audio = GetAudio(audioName, true);
        FadeInMusic(audio);
    }

    // Function to fade-in music (for specific audio).
    public async void FadeInMusic(AudioProfile audio)
    {
        if (audio == null)
        {
            return;
        }

        if (sourceBGM.isPlaying)
        {
            await FadeOutMusicAsync();
        }

        if (isFading)
        {
            try
            {
                cancelTokenSource?.Cancel();
            }
            catch (ObjectDisposedException) { }

            cancelTokenSource = new CancellationTokenSource();
        }
        else
        {
            isFading = true;
        }

        await FadeInMusicAsync(audio);
    }

    // Function to fade-in music (for handling async method).
    private async Task FadeInMusicAsync(AudioProfile audio)
    {
        sourceBGM.clip = audio.Clip;
        sourceBGM.volume = 0f;
        sourceBGM.pitch = audio.Pitch;
        sourceBGM.Play();

        currentMusic = audio;

        float targetVolume = audio.Volume / 100f;

        while (sourceBGM.volume < targetVolume)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            try
            {
                sourceBGM.volume += fadeInSpeed * Time.deltaTime;
                await Task.Yield();
            }
            catch (OperationCanceledException)
            {
                return;
            }
            finally
            {
                cancelTokenSource.Dispose();
            }
        }

        sourceBGM.volume = targetVolume;
        isFading = false;
    }

    // Function to fade-out music.
    public async void FadeOutMusic()
    {
        if (!sourceBGM.isPlaying)
        {
            return;
        }

        if (isFading)
        {
            try
            {
                cancelTokenSource?.Cancel();
            }
            catch (ObjectDisposedException) { }

            cancelTokenSource = new CancellationTokenSource();
        }
        else
        {
            isFading = true;
        }

        await FadeOutMusicAsync();
    }

    // Function to fade-out music (for handling async method).
    private async Task FadeOutMusicAsync()
    {
        while (sourceBGM.volume > 0f)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            try
            {
                sourceBGM.volume -= fadeOutSpeed * Time.deltaTime;
                await Task.Yield();
            }
            catch (OperationCanceledException)
            {
                return;
            }
            finally
            {
                cancelTokenSource.Dispose();
            }
        }

        sourceBGM.volume = 0f;
        currentMusic = null;
        isFading = false;
    }

    // Function to set fade-in speed.
    public void SetFadeInSpeed(float value)
    {
        value = Mathf.Clamp(value, 0f, 10f);
        fadeInSpeed = value;
    }

    // Function to set fade-out speed.
    public void SetFadeOutSpeed(float value)
    {
        value = Mathf.Clamp(value, 0f, 10f);
        fadeOutSpeed = value;
    }

    // Function to get audio from name.
    private AudioProfile GetAudio(string audioName, bool random)
    {
        List<AudioProfile> selected = new List<AudioProfile>();

        for (int i = 0; i < audioProfiles.Count; i++)
        {
            AudioProfile audio = audioProfiles[i];

            if (random)
            {
                if (audio.Name.StartsWith(audioName))
                {
                    selected.Add(audio);
                }
            }
            else
            {
                if (audio.Name.Equals(audioName))
                {
                    return audio;
                }
            }
        }
        
        if (selected.Count > 0)
        {
            return selected[UnityEngine.Random.Range(0, selected.Count - 1)];
        }

        return null;
    }
    
    // Function to load volume data.
    private void LoadData()
    {
        float value = PlayerPrefs.GetFloat(MASTER_VOL, 100f);
        mixerMaster.audioMixer.SetFloat(MASTER_VOL, ConvertToMixerVolume(value));
        masterVolume = value;

        value = PlayerPrefs.GetFloat(MUSIC_VOL, 100f);
        mixerBGM.audioMixer.SetFloat(MUSIC_VOL, ConvertToMixerVolume(value));
        musicVolume = value;

        value = PlayerPrefs.GetFloat(SFX_VOL, 100f);
        mixerSFX.audioMixer.SetFloat(SFX_VOL, ConvertToMixerVolume(value));
        soundVolume = value;
    }

    // Function to save volume data.
    private void SaveData()
    {
        mixerMaster.audioMixer.GetFloat(MASTER_VOL, out float value);
        PlayerPrefs.SetFloat(MASTER_VOL, ConvertToAudioVolume(value));

        mixerMaster.audioMixer.GetFloat(MUSIC_VOL, out value);
        PlayerPrefs.SetFloat(MUSIC_VOL, ConvertToAudioVolume(value));

        mixerMaster.audioMixer.GetFloat(SFX_VOL, out value);
        PlayerPrefs.SetFloat(SFX_VOL, ConvertToAudioVolume(value));
    }

    // Function to convert audio volume (0 - 100) to mixer volume (-80 - 0).
    private float ConvertToMixerVolume(float volume)
    {
        float range = MIXER_MAX_VOL - mixerMinVolume;
        float mixerVolume = (volume / 100f) * range;
        return mixerVolume - range;
    }

    // Function to convert mixer volume (-80 - 0) to audio volume (0 - 100).
    private float ConvertToAudioVolume(float mixerVolume)
    {
        float range = MIXER_MAX_VOL - mixerMinVolume;
        float ratio = (mixerVolume - mixerMinVolume) / range;
        return ratio * 100f;
    }
}