using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderToText : MonoBehaviour {

    // SLIDER & TEXT
    [SerializeField] Slider slider = null;
    [SerializeField] GameObject textObject = null;




    // SLIDER PROPERTIES TO DISPLAY TO TEXT
    [SerializeField] bool
        minValueToText = false,
        maxValueToText = false,
        currentValueToText = true,
        intOnly = true;
    [SerializeField] float valueToAdd = 0;




    // BASE FUNCTIONS
    // Use this for initialization
    void Start () { 
    }
	
	// Update is called once per frame
	void Update () {
        SliderValueToText();
    }

    void SliderValueToText()
    {
        if (textObject.GetComponent<TextMeshPro>())
        {
            TextMeshPro textComponent = GetComponent<TextMeshPro>();


            if (intOnly)
            {
                if (minValueToText)
                    textComponent.text = ((int)slider.minValue + (int)valueToAdd).ToString();
                else if (maxValueToText)
                    textComponent.text = ((int)slider.maxValue + (int)valueToAdd).ToString();
                else if (currentValueToText)
                    textComponent.text = ((int)slider.value + (int)valueToAdd).ToString();
            }
            else
            {
                if (minValueToText)
                    textComponent.text = (slider.minValue + valueToAdd).ToString();
                else if (maxValueToText)
                    textComponent.text = (slider.maxValue + valueToAdd).ToString();
                else if (currentValueToText)
                    textComponent.text = (slider.value + valueToAdd).ToString();
            }
        }
        if (textObject.GetComponent<TextMeshProUGUI>())
        {
            TextMeshProUGUI textComponent = GetComponent<TextMeshProUGUI>();


            if (intOnly)
            {
                if (minValueToText)
                    textComponent.text = ((int)slider.minValue + (int)valueToAdd).ToString();
                else if (maxValueToText)
                    textComponent.text = ((int)slider.maxValue + (int)valueToAdd).ToString();
                else if (currentValueToText)
                    textComponent.text = ((int)slider.value + (int)valueToAdd).ToString();
            }
            else
            {
                if (minValueToText)
                    textComponent.text = (slider.minValue + valueToAdd).ToString();
                else if (maxValueToText)
                    textComponent.text = (slider.maxValue + valueToAdd).ToString();
                else if (currentValueToText)
                    textComponent.text = (slider.value + valueToAdd).ToString();
            }
        }
        if (textObject.GetComponent<Text>())
        {
            Text textComponent = GetComponent<Text>();


            if (intOnly)
            {
                if (minValueToText)
                    textComponent.text = ((int)slider.minValue + (int)valueToAdd).ToString();
                else if (maxValueToText)
                    textComponent.text = ((int)slider.maxValue + (int)valueToAdd).ToString();
                else if (currentValueToText)
                    textComponent.text = ((int)slider.value + (int)valueToAdd).ToString();
            }
            else
            {
                if (minValueToText)
                    textComponent.text = (slider.minValue + valueToAdd).ToString();
                else if (maxValueToText)
                    textComponent.text = (slider.maxValue + valueToAdd).ToString();
                else if (currentValueToText)
                    textComponent.text = (slider.value + valueToAdd).ToString();
            }
        }
    }
}
