using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SliderToVolume : MonoBehaviour
{
    [SerializeField]
    Slider slider;
    [SerializeField]
    AudioMixer mixer;
    [SerializeField]
    string mixerGroup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mixer.SetFloat(mixerGroup, slider.value);
    }
}
