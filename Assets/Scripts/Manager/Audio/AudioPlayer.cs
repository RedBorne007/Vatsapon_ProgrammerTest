//------------------------------------------------------------------
//
//                         Audio System
//                  by Vatsapon Asawakittiporn
//
// Note: This class use for playing audio with auto-reference
//       to Audio Manager.
//
//------------------------------------------------------------------

using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [Tooltip("Specific audio source to play")]
    [SerializeField] private AudioSource source;

    private AudioManager audioM;

    private void Start()
    {
        audioM = AudioManager.Instance;
    }

    // Function to play SFX.
    public void PlaySFX(string audioName)
    {
        if (source)
        {
            audioM.PlaySFX(source, audioName);
        }
        else
        {
            audioM.PlaySFX(audioName);
        }
    }
    // Function to play SFX (for specific audio).
    public void PlaySFX(AudioProfile audio) => audioM.PlaySFX(audio);

    // Function to play random SFX.
    public void PlayRandomSFX(string audioName) => audioM.PlayRandomSFX(audioName);

    // Function to play SFX.
    public void PlayMusic(string audioName) => audioM.PlayMusic(audioName);

    // Function to play SFX (for specific audio).
    public void PlayMusic(AudioProfile audio) => audioM.PlayMusic(audio);

    // Function to play random SFX.
    public void PlayRandomMusic(string audioName) => audioM.PlayRandomMusic(audioName);

    // Function to stop music without fade-out.
    public void StopMusic() => audioM.StopMusic();

    // Function to fade-in music.
    public void FadeInMusic(string audioName) => audioM.FadeInMusic(audioName);

    // Function to fade-in music (for specific audio).
    public void FadeInMusic(AudioProfile audio) => audioM.FadeInMusic(audio);

    // Function to fade-in random music.
    public void FadeInRandomMusic(string audioName) => audioM.FadeInRandomMusic(audioName);

    // Function to fade-out music.
    public void FadeOutMusic() => audioM.FadeOutMusic();
}
