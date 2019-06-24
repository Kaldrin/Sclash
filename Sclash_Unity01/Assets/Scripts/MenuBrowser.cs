using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


// Created for Unity 2019.1.1f1
public class MenuBrowser : MonoBehaviour
{
    [SerializeField] GameObject[] elements = null;

    [SerializeField] int browseIndex;
    int sens = 1;

    bool vAxisInUse = false;
    [SerializeField] bool invert = false;

    






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

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            if (elements[browseIndex].GetComponent<SliderToVolume>())
            {
                elements[browseIndex].GetComponent<SliderToVolume>().slider.value++;
            }
        }
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            if (elements[browseIndex].GetComponent<SliderToVolume>())
            {
                elements[browseIndex].GetComponent<SliderToVolume>().slider.value--;
            }
        }


        if (Input.GetAxisRaw("Vertical") == 0)
        {
            vAxisInUse = false;
        }

        if (Input.GetAxisRaw("Vertical") > 0)
        {
            if (vAxisInUse == false)
            {

                browseIndex += sens;

                vAxisInUse = true;
            }
        }

        if (Input.GetAxisRaw("Vertical") < 0)
        {
            if (vAxisInUse == false)
            {

                browseIndex -= sens;

                vAxisInUse = true;
            }
        }

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

    }






    // COLOR
    void ResetColor()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            GameObject g = elements[i];

            if (g.transform.GetChild(0).GetComponent<TextMeshProUGUI>())
            {
                g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
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
                    g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
                }
                else
                {
                    g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
                }
            }
        }
    }
}