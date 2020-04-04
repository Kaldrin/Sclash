using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


// Created for Unity 2019.1.1f1
public class MenuBrowser : MonoBehaviour
{
    #region VARIABLES
    #region BROWSING
    [Header("BROWSING")]
    [Tooltip("The list of elements to browse in this menu page")]
    [SerializeField] public GameObject[] elements = null;
    [Tooltip("The back element of this menu page that allows for ")]
    [SerializeField] public GameObject backElement = null;

    [Tooltip("The index of the selected menu element in the elements list")]
    [SerializeField] public int browseIndex = 0;
    [SerializeField] bool applyDefaultIndexOnEnable = false;
    [SerializeField] int defaultIndex = 0;
    int sens = 1;

    [SerializeField] bool invertVerticalAxis = false;
    [SerializeField]
    bool
        canBack = false,
        swapAxis = false,
        allowHorizontalJump = false;
    [SerializeField] int horizontalJumpAmount = 4;
    #endregion




    #region SPECIAL
    [Header("SPECIAL")]
    [SerializeField] bool callSpecialElementWHenHorizontal = false;
    [SerializeField] public GameObject positiveHorizontalSpecialElement = null;
    [SerializeField] public GameObject negativeHorizontalSpecialElement = null;
    #endregion





    #region INPUTS
    [Header("INPUTS")]
    [Tooltip("Names of the inputs axis created in the input settings")]
    [SerializeField] string horizontalAxis = "Horizontal";
    [SerializeField] string
        verticalAxis = "Vertical",
        backButton = "Back";

    bool
        vAxisInUse = false,
        hAxisInUse = false;

    [Tooltip("Dead zones for joystick browsing so that it's more comfortable")]
    [SerializeField] float
        verticalInputDetectionZone = 0.5f,
        verticalInputRestZone = 0.1f,
        horizontalInputDetectionZone = 0.5f,
        horizontalRestZone = 0.1f;
    #endregion




    #region VISUAL
    [Header("VISUAL")]
    [Tooltip("Default color of an unselected menu element's text")]
    [SerializeField] Color textDefaultColor = Color.black;
    [Tooltip("Color of an selected menu element's text")]
    [SerializeField] Color textSelectedColor = Color.white;
    [Tooltip("Color of an selected menu element's button")]
    [SerializeField] Color buttonSelectedColor = Color.white;
    [Tooltip("Color of an selected menu element's button")]
    [SerializeField] Color buttonDefaultColor = Color.black;

    [Tooltip("For each menu element, should use the script's button color change or let the button's default one ?")]
    [SerializeField] public List<bool> shouldUseButtonColorSwitch = new List<bool>();
    #endregion





    #region SOUND
    [Header("SOUND")]
    [Tooltip("The PlayRandomSoundInList script to play a random sound from the given source on it whenever browsing through menu elements")]
    [SerializeField] PlayRandomSoundInList hoverSound = null;
    #endregion
    #endregion














    #region FUNCTIONS
    #region BASE FUNCTIONS
    void Awake()
    {
        FixButtonColorUsageList();


        // BROWSING DIRECTION
        if (invertVerticalAxis)
        {
            sens = -1;
        }
        else
        {
            sens = 1;
        }


        // SWAP VERTICAL / HORIZONTAL AXIS TO BROWSE
        if (swapAxis)
        {
            string storeVerticalAxis = verticalAxis;
            verticalAxis = horizontalAxis;
            horizontalAxis = storeVerticalAxis;
        }
    }


    void Start()
    {
        VerticalBrowse(0);
        Select(false);
        Select(true);
        UpdateColors();
    }



    // Update is called once per graphic frame
    void Update()
    {
        if (elements.Length > 0)
        {
            // H AXIS
            // Detects H axis let go
            if (Mathf.Abs(Input.GetAxisRaw(horizontalAxis)) <= horizontalRestZone)
            {
                hAxisInUse = false;
            }


            // Move sliders with horizontal
            
            if (Input.GetAxis(horizontalAxis) > horizontalInputDetectionZone && !hAxisInUse)
            {
                if (callSpecialElementWHenHorizontal)
                {
                    positiveHorizontalSpecialElement.GetComponent<Button>().onClick.Invoke();
                }
                else if (allowHorizontalJump)
                {
                    VerticalBrowse(horizontalJumpAmount);
                }
                else if (elements[browseIndex].GetComponent<SliderToVolume>())
                {
                    elements[browseIndex].GetComponent<SliderToVolume>().slider.value++;
                }
                

                hAxisInUse = true;
            }
            else if (Input.GetAxis(horizontalAxis) < -horizontalInputDetectionZone & !hAxisInUse)
            {
                if (callSpecialElementWHenHorizontal)
                {
                    negativeHorizontalSpecialElement.GetComponent<Button>().onClick.Invoke();
                }
                else if (allowHorizontalJump)
                {
                    VerticalBrowse(- horizontalJumpAmount);
                }
                else if (elements[browseIndex].GetComponent<SliderToVolume>())
                {
                    elements[browseIndex].GetComponent<SliderToVolume>().slider.value--;
                }


                hAxisInUse = true;
            }







            // V AXIS
            // Detects V axis let go
            if (Mathf.Abs(Input.GetAxis(verticalAxis)) <= verticalInputRestZone)
            {
                vAxisInUse = false;
            }

            
            if (!vAxisInUse)
            {
                // Detects positive V axis input
                if (Input.GetAxis(verticalAxis) > verticalInputDetectionZone)
                {
                    VerticalBrowse(1);
                }


                // Detects negative V axis input
                if (Input.GetAxis(verticalAxis) < -verticalInputDetectionZone)
                {
                    VerticalBrowse(-1);
                }
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

    // OnEnable is called each time the object is set from inactive to active
    void OnEnable()
    {
        if (applyDefaultIndexOnEnable)
            browseIndex = defaultIndex;


        VerticalBrowse(0);
        Select(true);
        UpdateColors();
    }

    private void OnDisable()
    {
        UpdateColors();
    }
    #endregion










    #region BROWSING
    void VerticalBrowse(int direction)
    {
        vAxisInUse = true;
        browseIndex += sens * direction;


        // AUDIO
        hoverSound.Play();
        

        // Loop index navigation
        if (browseIndex > elements.Length - 1)
        {
            if (allowHorizontalJump)
            {
                browseIndex = 0 + (browseIndex - (elements.Length));
            }
            else
            {
                browseIndex = 0;
            }
        }
        else if (browseIndex < 0)
        {
            if (allowHorizontalJump)
            {
                browseIndex = (elements.Length) - Mathf.Abs(browseIndex);
            }
            else
            {
                browseIndex = elements.Length - 1;
            }
        }


        // VISUAL
        UpdateColors();


        Select(true);
    }
    #endregion








    #region SELECTION
    // Selects or unselects not the specified object
    public void Select(bool state)
    {
        if (!state)
        {
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
        }
        else
        {
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(elements[browseIndex]);


            if (elements[browseIndex].GetComponent<Button>())
            {
                elements[browseIndex].GetComponent<Button>().Select();
            }
            else if (elements[browseIndex].GetComponent<TMP_InputField>())
            {
                elements[browseIndex].GetComponent<TMP_InputField>().Select();
            }
            else if (elements[browseIndex].GetComponent<EventTrigger>())
            {
            }
        }


        UpdateColors();
    }

    // Sets the browse index to the mouse hovered element
    public void SelectHoveredElement(GameObject hoveredObject)
    {
        if (enabled)
        {
            if (isObjectInGameObjectArray(hoveredObject, elements))
            {
                browseIndex = FindObjectIndexInGameObjectArray(hoveredObject, elements);
            }

        
            if (elements[browseIndex].GetComponent<Button>() || elements[browseIndex].GetComponent<TMP_InputField>())
            {
                Select(true);
            }
            else
            {
                Select(false);
            }
        }
    }
    #endregion





    #region ADD & REMOVE ELEMENTS
    public void AddElement(GameObject newButton)
    {
        GameObject[] newElementsList = new GameObject[elements.Length + 1];

        newElementsList[newElementsList.Length - 1] = newButton;


        for (int i = 0; i < elements.Length; i++)
        {
            newElementsList[i] = elements[i];
        }


        elements = newElementsList;
        shouldUseButtonColorSwitch.Add(true);
    }

    public void RemoveElement(GameObject buttonToRemove)
    {
        if (isObjectInGameObjectArray(buttonToRemove, elements))
        {
            int indexToRemove = FindObjectIndexInGameObjectArray(buttonToRemove, elements);
            GameObject[] newElements = new GameObject[elements.Length - 1];

            int indexOffset = 0;


            for (int i = 0; i < newElements.Length; i++)
            {
                if (i == indexToRemove)
                    indexOffset = 1;


                newElements[i] = elements[i + indexOffset];
            }


            elements = newElements;
        }
        else {}
    }

    public void ChangeBackButton(GameObject newBackButton)
    {
        backElement = newBackButton;
    }
    #endregion







    #region COLORS / VISUAL
    public void FixButtonColorUsageList()
    {
        if (shouldUseButtonColorSwitch.Count < elements.Length)
        {
            for (int i = 0; i <= elements.Length - shouldUseButtonColorSwitch.Count; i++)
            {
                shouldUseButtonColorSwitch.Add(true);
            }
        }
    }

    // Updates the color of each element of the menu screen depending on wether it's selected or not
    void UpdateColors()
    {
        // Browses through all elements
        for (int i = 0; i < elements.Length; i++)
        {
            GameObject g = elements[i];


            // If the checked element is the currently selected one
            if (i == browseIndex && enabled)
            {
                try
                {
                    // If the element is set to change its color when selected, changes it
                    if (shouldUseButtonColorSwitch[i])
                    {
                        if (g.GetComponent<Image>())
                            g.GetComponent<Image>().color = buttonSelectedColor;
                    }
                }
                catch {}


                // Changes text color of the element's children
                for (int j = 0; j < g.transform.childCount; j++)
                {
                    if (g.transform.GetChild(j).GetComponent<TextMeshProUGUI>())
                    {
                        g.transform.GetChild(j).GetComponent<TextMeshProUGUI>().color = textSelectedColor;
                    }
                }
            }
            // if the checked element is not the currently selected one
            else
            {
                try
                {
                    // If the element is set to change its color when selected, changes it
                    if (shouldUseButtonColorSwitch[i] && g.GetComponent<Button>())
                    {
                        g.GetComponent<Image>().color = buttonDefaultColor;
                    }
                }
                catch {}


                // Changes text color of the element's children
                for (int j = 0; j < g.transform.childCount; j++)
                {
                    if (g.transform.GetChild(j).GetComponent<TextMeshProUGUI>())
                    {
                        g.transform.GetChild(j).GetComponent<TextMeshProUGUI>().color = textDefaultColor;
                    }
                }
            }
        }
    }
    #endregion






    #region SECONDARY FUNCTIONS
    // Returns true if element is contained in referenced array
    bool isObjectInGameObjectArray(GameObject objectToFind, GameObject[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == objectToFind)
            {
                return true;
            }
        }


        return false;
    }

    int FindObjectIndexInGameObjectArray(GameObject objectToFind, GameObject[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == objectToFind)
            {
                return i;
            }
        }


        return 0;
    }
    #endregion
    #endregion
}