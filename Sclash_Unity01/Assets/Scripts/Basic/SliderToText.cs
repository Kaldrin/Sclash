﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderToText : MonoBehaviour {

    // SLIDER & TEXT
    [Header("Slider & text")]
    [SerializeField] Slider slider = null;
    [SerializeField] GameObject textObject = null;

    // SLIDER PROPERTIES TO DISPLAY
    [Header("Slider properties")]
    [SerializeField] bool minValueToText = false;
    [SerializeField] bool
        maxValueToText = false,
        currentValueToText = true;
    [SerializeField] float valueToAdd = 0;





    // BASE FUNCTIONS
    // Use this for initialization
    void Start () { 
    }
	
	// Update is called once per frame
	void Update () {
        SliderValueToText();
    }




    // DISPLAY
    void SliderValueToText()
    {
        if (textObject.GetComponent<TextMeshPro>())
        {
            TextMeshPro textComponent = GetComponent<TextMeshPro>();

            if (minValueToText)
                textComponent.text = (slider.minValue + valueToAdd).ToString();
            else if (maxValueToText)
                textComponent.text = (slider.maxValue + valueToAdd).ToString();
            else if (currentValueToText)
                textComponent.text = (slider.value + valueToAdd).ToString();
        }
        if (textObject.GetComponent<TextMeshProUGUI>())
        {
            TextMeshProUGUI textComponent = GetComponent<TextMeshProUGUI>();

            if (minValueToText)
                textComponent.text = (slider.minValue + valueToAdd).ToString();
            else if (maxValueToText)
                textComponent.text = (slider.maxValue + valueToAdd).ToString();
            else if (currentValueToText)
                textComponent.text = (slider.value + valueToAdd).ToString();
        }
        if (textObject.GetComponent<Text>())
        {
            Text textComponent = GetComponent<Text>();

            if (minValueToText)
                textComponent.text = (slider.minValue + valueToAdd).ToString();
            else if (maxValueToText)
                textComponent.text = (slider.maxValue + valueToAdd).ToString();
            else if (currentValueToText)
                textComponent.text = (slider.value + valueToAdd).ToString();
        }
    }
}
