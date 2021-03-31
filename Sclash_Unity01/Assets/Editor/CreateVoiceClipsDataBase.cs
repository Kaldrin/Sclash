using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;





// For Sclash

// REQUIREMENTS
// VoiceClipsDataBase scriptable object
// VoiceClip class


/// <summary>
/// This is an editor script that creates a menu to update the voice clips data base based on the hierarchy of folders and audio clips in the Assets/SOUNDS/Voices folder, for the voice acting localization
/// </summary>

// UNITY 2019.4.14
public class CreateVoiceClipsDataBase : MonoBehaviour
{
    [MenuItem("Localization/Update voice clips database")]
    public static VoiceClipsDataBase Create()
    {
        VoiceClipsDataBase asset = VoiceClipsDataBase.CreateInstance<VoiceClipsDataBase>();


        // Find folders / keys
        string[] subfolders = AssetDatabase.GetSubFolders("Assets/SOUND/Voices");
        string[] keys = subfolders;



        for (int i = 0; i < subfolders.Length; i++)
        {
            VoiceClip newVoiceClip = new VoiceClip();
            newVoiceClip.key = subfolders[i].Remove(0, 20);

            // Languages
            // FR
            if (AssetDatabase.LoadAssetAtPath<AudioClip>(subfolders[i] + "/FR.mp3"))
                newVoiceClip.fr = AssetDatabase.LoadAssetAtPath<AudioClip>(subfolders[i] + "/FR.mp3");
            // EN
            if (AssetDatabase.LoadAssetAtPath<AudioClip>(subfolders[i] + "/EN.mp3"))
                newVoiceClip.en = AssetDatabase.LoadAssetAtPath<AudioClip>(subfolders[i] + "/EN.mp3");
            // GER
            if (AssetDatabase.LoadAssetAtPath<AudioClip>(subfolders[i] + "/GER.mp3"))
                newVoiceClip.ger = AssetDatabase.LoadAssetAtPath<AudioClip>(subfolders[i] + "/GER.mp3");



            asset.voiceClips.Add(newVoiceClip);
        }



        AssetDatabase.CreateAsset(asset, "Assets/Localization/VoiceClipsDataBase.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}
