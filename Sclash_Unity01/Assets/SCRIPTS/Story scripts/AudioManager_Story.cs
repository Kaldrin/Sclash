using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager_Story : MonoBehaviour
{
    public static AudioManager_Story Instance;
    [Header("STORY")]
    [SerializeField] public AudioSource mainMusic = null;

    private void Awake()
    {
        Instance = this;
    }
}
