using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSoundManager : MonoBehaviour
{
    #region Singleton
    private static DebrisSoundManager _instance;

    public static DebrisSoundManager Instance
    {
        get { return _instance; }
    }

    void Awake()
    {
        if (_instance != this && _instance != null)
            Destroy(this.gameObject);

        _instance = this;
    }

    #endregion

    //Add audiosource to GameObject and Destroy it when the audio clip is over
    public void AddSound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("No AudioClip");
            return;
        }

        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.pitch = Random.Range(0.75f, 1.75f);
        source.clip = clip;

        source.Play();

        StartCoroutine(DestroyAudioSource(source, clip.length + 0.1f));
    }


    IEnumerator DestroyAudioSource(AudioSource source, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        Destroy(source);
    }
}
