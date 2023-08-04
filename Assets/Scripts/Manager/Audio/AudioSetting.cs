//------------------------------------------------------------------
//
//                         Audio System
//                  by Vatsapon Asawakittiporn
//
// Note: This class use for handling Audio setting which can be
//       use in game object in game scene to adjust volume.
//
//------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AudioManager;

public class AudioSetting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private TMP_Text masterText;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private TMP_Text BGMText;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private TMP_Text SFXText;

    private AudioManager audioM;

    private void Start()
    {
        audioM = AudioManager.Instance;

        if (!audioM)
        {
            Debug.LogWarning("There's no singleton of AudioManager. The Audio Setting will be disabled.");
            enabled = false;
            return;
        }

        if (masterSlider)
        {
            masterSlider.value = audioM.MasterVolume;
            masterText?.SetText(masterSlider.value.ToString());
        }

        if (BGMSlider)
        {
            BGMSlider.value = audioM.MusicVolume;
            BGMText?.SetText(BGMSlider.value.ToString());
        }

        if (SFXSlider)
        {
            SFXSlider.value = audioM.SoundVolume;
            SFXText?.SetText(SFXSlider.value.ToString());
        }
    }

    // Function to adjust master volume with slider.
    public void UpdateMasterVolume() => UpdateVolume(AudioCategory.Master);

    // Function to adjust master volume with value (can be positive/negative).
    public void AdjustMasterVolume(float adjustValue) => AdjustVolume(AudioCategory.Master, adjustValue);

    // Function to adjust music volume with slider.
    public void UpdateBGMVolume() => UpdateVolume(AudioCategory.Music);

    // Function to adjust BGM volume with value (can be positive/negative).
    public void AdjustBGMVolume(float adjustValue) => AdjustVolume(AudioCategory.Music, adjustValue);

    // Function to adjust SFX volume with slider.
    public void UpdateSFXVolume() => UpdateVolume(AudioCategory.Sound);

    // Function to adjust SFX volume with value (can be positive/negative).
    public void AdjustSFXVolume(float adjustValue) => AdjustVolume(AudioCategory.Sound, adjustValue);

    // Function to update volume with slider.
    private void UpdateVolume(AudioCategory audioCategory)
    {
        Slider selectedSlider = null;
        TMP_Text text = null;

        switch (audioCategory)
        {
            case AudioCategory.Master:
            selectedSlider = masterSlider;
            text = masterText;
            break;

            case AudioCategory.Music:
            selectedSlider = BGMSlider;
            text = BGMText;
            break;

            case AudioCategory.Sound:
            selectedSlider = SFXSlider;
            text = SFXText;
            break;

            default:
            return;
        }

        float value = Mathf.Clamp(selectedSlider.value, 0f, 100f);
        text?.SetText(value.ToString());
        audioM.SetVolume(audioCategory, value);
    }

    // Function to adjust volume with value (can be positive/negative).
    private void AdjustVolume(AudioCategory audioCategory, float adjustValue)
    {
        float selectedVolume;

        switch (audioCategory)
        {
            case AudioCategory.Master:
            selectedVolume = audioM.MasterVolume;
            break;

            case AudioCategory.Music:
            selectedVolume = audioM.MusicVolume;
            break;

            case AudioCategory.Sound:
            selectedVolume = audioM.SoundVolume;
            break;

            default:
            return;
        }

        float value = selectedVolume + adjustValue;
        value = Mathf.Clamp(value, 0f, 100f);
        audioM.SetVolume(audioCategory, value);
    }

    // Function to get master volume.
    public float GetMasterVolume() => audioM.MasterVolume;

    // Function to get music volume.
    public float GetMusicVolume() => audioM.MusicVolume;

    // Function to get sound volume.
    public float GetSoundVolume() => audioM.SoundVolume;
}
