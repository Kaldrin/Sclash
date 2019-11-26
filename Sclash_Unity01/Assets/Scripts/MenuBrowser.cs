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
    [Tooltip("The list of elements to browse in this menu page")]
    [SerializeField] public GameObject[] elements = null;
    [Tooltip("The back element of this menu page that allows for ")]
    [SerializeField] public GameObject backElement = null;

    [Tooltip("The index of the selected menu element in the elements list")]
    [SerializeField] public int browseIndex;
    int sens = 1;

    [SerializeField] bool invert = false;
    [SerializeField] bool canBack = false;
    bool
        vAxisInUse = false,
        hAxisInUse = false;

    [Tooltip("Names of the inputs axis created in the input settings")]
    [SerializeField] string
        horizontalAxis = "Horizontal",
        verticalAxis = "Vertical",
        backButton = "Back";

    [SerializeField] float
        verticalInputDetectionZone = 0.5f,
        verticalInputRestZone = 0.1f,
        horizontalInputDetectionZone = 0.5f,
        horizontalRestZone = 0.1f;





    // VISUAL
    [Header("VISUAL")]
    [Tooltip("Default color of an unselected menu element")]
    [SerializeField] Color defaultColor = Color.black;
    [Tooltip("Color of an selected menu element")]
    [SerializeField] Color selectedColor = Color.white;






    // SOUND
    [Header("Sound")]
    [Tooltip("The PlayRandomSoundInList script to play a random sound from the given source on it whenever browsing through menu elements")]
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
        if (elements.Length > 0)
        {
            // H AXIS
            // Move sliders with horizontal
            if (Input.GetAxisRaw(horizontalAxis) > horizontalInputDetectionZone && !hAxisInUse)
            {
                if (elements[browseIndex].GetComponent<SliderToVolume>())
                {
                    elements[browseIndex].GetComponent<SliderToVolume>().slider.value++;
                    hAxisInUse = true;
                }
            }
            else if (Input.GetAxisRaw(horizontalAxis) < -horizontalInputDetectionZone & !hAxisInUse)
            {
                if (elements[browseIndex].GetComponent<SliderToVolume>())
                {
                    elements[browseIndex].GetComponent<SliderToVolume>().slider.value--;
                    hAxisInUse = true;
                }
            }

            // Detects H axis let ho
            if (Mathf.Abs(Input.GetAxisRaw(horizontalAxis)) <= horizontalRestZone)
            {
                hAxisInUse = false;
            }






            // V AXIS
            // Detects V axis let go
            if (Mathf.Abs(Input.GetAxisRaw(verticalAxis)) <= verticalInputRestZone)
            {
                vAxisInUse = false;
            }


            // Detects positive V axis input
            if (Input.GetAxisRaw(verticalAxis) > verticalInputDetectionZone)
            {
                if (vAxisInUse == false)
                {
                    browseIndex += sens;
                    vAxisInUse = true;
                    hoverSound.Play();
                }
            }


            // Detects negative V axis input
            if (Input.GetAxisRaw(verticalAxis) < -verticalInputDetectionZone)
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
                Select(true);
            }
            else
            {
                Select(false);
            }
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

    // Deselects the currently selected element
    

    // OnEnable is called each time the object is set from inactive to active
    void OnEnable()
    {
        if (elements.Length > 0)
        {
            browseIndex = 0;


            if (elements[browseIndex].GetComponent<Button>())
            {
                Select(true);
            }
            else
            {
                Select(false);
            }
        }
    }






    // SELECTION
    // Selects or unselects not the specified object
    public void Select(bool state)
    {
        if (!state)
        {
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
            ColorSwap();
        }
        else
        {
            elements[browseIndex].GetComponent<Button>().Select();
            ResetColor();
        }
    }
    
    // Sets the browse index to the mouse hovered element
    public void SelectHoveredElement(GameObject hoveredObject)
    {
        if (ArrayContainsGameObject(hoveredObject, elements))
        {
            browseIndex = IndexOfGameObjectInArray(hoveredObject, elements);
        }

        
        if (elements[browseIndex].GetComponent<Button>())
        {
            Select(true);
        }
        else
        {
            Select(false);
        }
    }

    // Returns true if element is contained in referenced array
    bool ArrayContainsGameObject(GameObject objectToCheck, GameObject[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (objectToCheck == array[i])
            {
                return true;
            }
        }


        return false;
    }

    int IndexOfGameObjectInArray(GameObject objectToCheck, GameObject[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (objectToCheck == array[i])
            {
                return i;
            }
        }


        return 0;
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