using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SliderToVolume : MonoBehaviour
{
    [SerializeField] public Slider slider = null;
    [SerializeField] AudioMixer mixer = null;
    [SerializeField] string mixerGroup = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVolume();
    }

    public void UpdateVolume()
    {
        mixer.SetFloat(mixerGroup, slider.value);
    }
}
