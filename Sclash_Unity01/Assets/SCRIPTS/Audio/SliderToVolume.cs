using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SliderToVolume : MonoBehaviour
{
    [SerializeField] public Slider slider;
    [SerializeField] AudioMixer mixer = null;
    float oldValue = 0;
    [SerializeField] string mixerGroup = null;









    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        oldValue = slider.value;
    }

    // Update is called once per graphic frame
    void Update()
    {
        if (slider.value != oldValue)
            UpdateVolume();
    } 





    // UPDATE VOLUME
    public void UpdateVolume()
    {
        mixer.SetFloat(mixerGroup, slider.value);
    }
}
