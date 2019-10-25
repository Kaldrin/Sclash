using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


// Created for Unity 2019.1.1f1
public class MenuBrowser : MonoBehaviour
{
    // BROWSING
    [Header("BROWSING")]
    [SerializeField] public GameObject[] elements = null;
    [SerializeField] public GameObject backElement = null;

    [SerializeField] int browseIndex;
    int sens = 1;

    bool vAxisInUse = false;
    [SerializeField] bool invert = false;
    [SerializeField] bool canBack = false;

    [SerializeField] string
        horizontalAxis = "Horizontal",
        verticalAxis = "Vertical",
        backButton = "Back";





    // VISUAL
    [Header("VISUAL")]
    [SerializeField] Color defaultColor = Color.black;
    [SerializeField] Color selectedColor = Color.white;




    // SOUND
    [Header("Sound")]
    [SerializeField] PlayRandomSoundInList hoverSound = null;


    









    // BASE FUNCTIONS
    void Awake()
    {
        if (invert)
        {
            sens = -1;
        }
        else
        {
            sens = 1;
        }
    }

    // Update is called once per graphic frame
    void Update()
    {
        // Move sliders with horizontal
        if (Input.GetAxisRaw(horizontalAxis) > 0)
        {
            if (elements[browseIndex].GetComponent<SliderToVolume>())
            {
                elements[browseIndex].GetComponent<SliderToVolume>().slider.value++;
            }
        }


        if (Input.GetAxisRaw(horizontalAxis) < 0)
        {
            if (elements[browseIndex].GetComponent<SliderToVolume>())
            {
                elements[browseIndex].GetComponent<SliderToVolume>().slider.value--;
            }
        }


        // V AXIS
        // Detects V axis let go
        if (Input.GetAxisRaw(verticalAxis) == 0)
        {
            vAxisInUse = false;
        }


        // Detects positive V axis input
        if (Input.GetAxisRaw(verticalAxis) > 0)
        {
            if (vAxisInUse == false)
            {
                browseIndex += sens;
                vAxisInUse = true;
                hoverSound.Play();
            }
        }


        // Detects negative V axis input
        if (Input.GetAxisRaw(verticalAxis) < 0)
        {
            if (vAxisInUse == false)
            {
                browseIndex -= sens;
                vAxisInUse = true;
                hoverSound.Play();
            }
        }


        // Loop index navigation
        if (browseIndex > elements.Length - 1)
        {
            browseIndex = 0;
        }
        else if (browseIndex < 0)
        {
            browseIndex = elements.Length - 1;
        }


        if (elements[browseIndex].GetComponent<Button>())
        {
            elements[browseIndex].GetComponent<Button>().Select();
            ResetColor();
        }
        else
        {
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
            ColorSwap();
        }


        // BACK
        if (canBack)
        {
            if (Input.GetButtonUp(backButton))
            {
                backElement.GetComponent<Button>().onClick.Invoke();
            }
        }
    }








    // COLOR
    void ResetColor()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            GameObject g = elements[i];

            if (g.transform.GetChild(0).GetComponent<TextMeshProUGUI>())
            {
                g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = defaultColor;
            }
        }
    }

    void ColorSwap()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            GameObject g = elements[i];


            if (g.transform.GetChild(0).GetComponent<TextMeshProUGUI>())
            {
                if (g == elements[browseIndex])
                {
                    g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = selectedColor;
                }
                else
                {
                    g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = defaultColor;
                }
            }
        }
    }
}