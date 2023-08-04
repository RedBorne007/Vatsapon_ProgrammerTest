//------------------------------------------------------------------
//
//                         Audio System
//                  by Vatsapon Asawakittiporn
//
// Note: This class use for handling Audio volume and pitch.
//
//------------------------------------------------------------------

using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Audio Profile", menuName = "Audio Profile")]
public class AudioProfile : ScriptableObject
{
    [SerializeField] private AudioClip clip;
    [Range(0f, 100f)]
    [SerializeField] private float volume = 100f;
    [Range(-3f, 3f)]
    [SerializeField] private float pitch = 1f;

    public string Name { get { return name; } set { name = value; } }
    public AudioClip Clip => clip;
    public float Volume => volume;
    public float Pitch => pitch;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Audio Profile from File", false, 1)]
    public static void CreateAudioProfile()
    {
        Object[] selected = Selection.objects;

        if (selected == null)
        {
            return;
        }

        for (int i = 0; i < selected.Length; i++)
        {
            if (selected[i] is AudioClip)
            {
                CreateAudioProfile(selected[i] as AudioClip);
            }
        }
    }

    // Function to create audio profile.
    private static void CreateAudioProfile(AudioClip clip)
    {
        string filePath = AssetDatabase.GetAssetPath(clip);

        string folderPath = Path.GetDirectoryName(filePath);
        string assetName = Path.GetFileNameWithoutExtension(filePath);

        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{assetName}.asset");

        AudioProfile audioProfileAsset = ScriptableObject.CreateInstance<AudioProfile>();
        audioProfileAsset.name = clip.name;
        audioProfileAsset.clip = clip;

        AssetDatabase.CreateAsset(audioProfileAsset, assetPath);
        Undo.RegisterCreatedObjectUndo(audioProfileAsset, "Create " + audioProfileAsset.name);
    }
#endif
}