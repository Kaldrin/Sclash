using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressLongToCallButton : MonoBehaviour
{
    [SerializeField] Button buttonToCall = null;
    [SerializeField] Slider sliderToSlide = null;
    [SerializeField] float incrementationSpeed = 0.015f;
    [SerializeField] bool pressing = false;
    [SerializeField] bool lastPressState = false;
    [SerializeField] bool selected = false;
    [SerializeField] AudioSource pressSoundAudioSource = null;
    bool hasStartedPlayingSound = false;
    [SerializeField] Vector2 pitchLimits = new Vector2(1, 1.8f);

    private void OnEnable()
    {
        sliderToSlide.value = sliderToSlide.minValue;
        pressing = false;
    }

    void FixedUpdate()
    {
        if (pressing)
        {
            if (selected)
                pressSoundAudioSource.pitch = pitchLimits.x + (pitchLimits.y - pitchLimits.x) * (sliderToSlide.value / sliderToSlide.maxValue);

            if (!lastPressState && !hasStartedPlayingSound && selected)
            {
                Debug.Log(pressSoundAudioSource.pitch);
                pressSoundAudioSource.loop = true;
                hasStartedPlayingSound = true;
                pressSoundAudioSource.Play();
            }

            if (sliderToSlide.value < sliderToSlide.maxValue)
                sliderToSlide.value += incrementationSpeed;
            else
            {
                CallButton();
                sliderToSlide.value = sliderToSlide.maxValue;
            }
        }
        else
        {
            if (selected)
            {
                pressSoundAudioSource.loop = false;
                hasStartedPlayingSound = false;
            }

            if (sliderToSlide.value > sliderToSlide.minValue)
                sliderToSlide.value -= incrementationSpeed;
            else
                sliderToSlide.value = sliderToSlide.minValue;
        }
    }

    private void Update()
    {
        if (GameObject.Find("GlobalManager"))
            if (GameObject.Find("GlobalManager").GetComponent<InputManager>().submitInputUp)
                pressing = false;
    }


    public void Press(bool onOff)
    {
        lastPressState = pressing;
        pressing = onOff;
    }

    public void Select(bool onOff)
    {
        selected = onOff;
    }

    void CallButton()
    {
        pressSoundAudioSource.Stop();
        hasStartedPlayingSound = false;
        sliderToSlide.value = sliderToSlide.minValue;
        pressing = false;
        buttonToCall.onClick.Invoke();
    }
}
