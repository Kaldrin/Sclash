using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


// Created for Unity 2019.1.1f1
public class MenuBrowser2D : MonoBehaviour
{
    


    #region VARIABLES
    #region BROWSING
    [System.Serializable]
    public class ElementsLine
    {
        [SerializeField] public GameObject[] line;
    }

    [Header("BROWSING")]
    [SerializeField] public ElementsLine[] elements2D = new ElementsLine[4];

    [SerializeField] bool canBack = false;
    [Tooltip("The back element of this menu page that allows for ")]
    [SerializeField] public GameObject backElement = null;

    [SerializeField] public int
        YIndex = 0,
        XIndex = 0;
    [SerializeField] bool applyDefaultIndexOnEnable = false;
    [SerializeField] int
        defaultYIndex = 0,
        defaultXIndex = 0;
    int verticalDirection = 1;
    int horizontalDirection = 1;

    [SerializeField] bool
        invertVerticalAxis = false,
        invertHorizontalAxis = false;
    [SerializeField] bool swapAxis = false;
    #endregion






    #region SPECIAL
    [Header("SPECIAL")]
    [SerializeField] bool callSpecialElementWHenHorizontalOverflow = false;
    [SerializeField] public GameObject specialElement = null;
    #endregion




    #region INPUTS
    [Header("INPUTS")]
    [Tooltip("Names of the inputs axis created in the input settings")]
    [SerializeField] string horizontalAxis = "Horizontal";
    [SerializeField] string
        verticalAxis = "Vertical",
        backButton = "Back";

    [SerializeField] bool
        vAxisInUse = false,
        hAxisInUse = true;
    
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
            horizontalDirection = - horizontalDirection;


        if (invertVerticalAxis)
            verticalDirection = - verticalDirection;


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
        if (elements2D.Length > 0)
        {
            // H AXIS
            // Detects H axis let go
            if (Mathf.Abs(Input.GetAxisRaw(horizontalAxis)) <= horizontalRestZone)
                hAxisInUse = false;


            // Move sliders with horizontal
            if (!hAxisInUse)
            {
                if (Input.GetAxis(horizontalAxis) > horizontalInputDetectionZone)
                {
                    HorizontalBrowse(1);
                    hAxisInUse = true;
                }
                else if (Input.GetAxis(horizontalAxis) < -horizontalInputDetectionZone)
                {
                    HorizontalBrowse(-1);
                    hAxisInUse = true;
                }
            }
            







            // V AXIS
            // Detects V axis let go
            if (Mathf.Abs(Input.GetAxis(verticalAxis)) <= verticalInputRestZone)
                vAxisInUse = false;


            if (!vAxisInUse)
            {
                // Detects positive V axis input
                if (Input.GetAxis(verticalAxis) > verticalInputDetectionZone)
                {
                    VerticalBrowse(1);
                    vAxisInUse = true;
                }


                // Detects negative V axis input
                if (Input.GetAxis(verticalAxis) < -verticalInputDetectionZone)
                {
                    VerticalBrowse(-1);
                    vAxisInUse = true;
                }
            }
        }


        // BACK
        if (canBack)
        {
            if (Input.GetButtonUp(backButton))
                backElement.GetComponent<Button>().onClick.Invoke();
        }
    }

    // OnEnable is called each time the object is set from inactive to active
    void OnEnable()
    {
        if (applyDefaultIndexOnEnable)
        {
            XIndex = defaultXIndex;
            YIndex = defaultYIndex;
        }


        if (elements2D.Length > 0)
        {
            /*
            if (elements2D[YIndex].line[XIndex].GetComponent<Button>())
                Select(true);
            else
                Select(false);
            */


            VerticalBrowse(0);
            HorizontalBrowse(0);


            Select(true);
            UpdateColors();
        }
    }
    #endregion










    #region BROWSING
    void VerticalBrowse(int direction)
    {
        YIndex += verticalDirection * direction;


        // AUDIO
        hoverSound.Play();


        // Loop index navigation
        if (YIndex > elements2D.Length - 1)
            YIndex = 0;
        else if (YIndex < 0)
            YIndex = elements2D.Length - 1;


        if (XIndex > elements2D[YIndex].line.Length - 1)
            XIndex = elements2D[YIndex].line.Length - 1;

        //HorizontalBrowse(0);


        // VISUAL
        UpdateColors();


        Select(true);
    }

    void HorizontalBrowse(int direction)
    {
        XIndex += horizontalDirection * direction;


        // AUDIO
        hoverSound.Play();


        //VerticalBrowse(0);


        // Loop index navigation
        if (XIndex > elements2D[YIndex].line.Length - 1)
        {
            if (callSpecialElementWHenHorizontalOverflow)
            {
                specialElement.GetComponent<Button>().onClick.Invoke();
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
                XIndex = elements2D[YIndex].line.Length - 1;
                
            }
            else
                XIndex = 0;
        }
        else if (XIndex < 0)
        {
            if (callSpecialElementWHenHorizontalOverflow)
            {
                specialElement.GetComponent<Button>().onClick.Invoke();
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
                XIndex = 0;
            }
            else
                XIndex = elements2D[YIndex].line.Length - 1; 
        }

        if (enabled)
        {
            // VISUAL
            UpdateColors();


            Select(true);
        }
    }

    public void SetXIndex(int newIndex)
    {
        XIndex = newIndex;
    }

    /*
    public void SetYIndexMinMax(bool minOrMax)
    {
        if (minOrMax)
            YIndex = ;
    }
    */

    public void SetYIndex(int newIndex)
    {
        YIndex = newIndex;
    }
    #endregion








    #region SELECTION
    // Selects or unselects not the specified object
    public void Select(bool state)
    {
        if (!state)
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
        else
        {
            if (elements2D[YIndex].line[XIndex])
            {
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(elements2D[YIndex].line[XIndex]);


                if (elements2D[YIndex].line[XIndex].GetComponent<Button>())
                    elements2D[YIndex].line[XIndex].GetComponent<Button>().Select();
                else if (elements2D[YIndex].line[XIndex].GetComponent<TMP_InputField>())
                    elements2D[YIndex].line[XIndex].GetComponent<TMP_InputField>().Select();
                else if (elements2D[YIndex].line[XIndex].GetComponent<EventTrigger>()) { }
            }
        }


        UpdateColors();
    }


    // Sets the browse index to the mouse hovered element
    public void SelectHoveredElement(GameObject hoveredObject)
    {
        for (int i = 0; i < elements2D.Length; i++)
        {
            if (isObjectInGameObjectArray(hoveredObject, elements2D[i].line))
            {
                XIndex = FindObjectIndexInGameObjectArray(hoveredObject, elements2D[i].line);
                YIndex = i;
            }
        }
        

        if (elements2D[YIndex].line[XIndex].GetComponent<Button>() || elements2D[YIndex].line[XIndex].GetComponent<TMP_InputField>())
            Select(true);
        else
            Select(false);
    }
    #endregion





    #region ADD & REMOVE ELEMENTS
    public void AddElement(GameObject newButton)
    { }

    public void RemoveElement(GameObject buttonToRemove)
    { }

    public void ChangeBackButton(GameObject newBackButton)
    {
        backElement = newBackButton;
    }
    #endregion







    #region COLORS / VISUAL
    public void FixButtonColorUsageList()
    { }

    // Updates the color of each element of the menu screen depending on wether it's selected or not
    void UpdateColors()
    {
        // Browses through all elements
        for (int i = 0; i < elements2D.Length; i++)
        {
            for (int y = 0; y < elements2D[i].line.Length; y++)
            {
                if (elements2D[i].line[y])
                {
                    GameObject g = elements2D[i].line[y];


                    // If the checked element is the currently selected one
                    if (i == YIndex && y == XIndex)
                    {
                        if (g.GetComponent<Image>())
                            g.GetComponent<Image>().color = buttonSelectedColor;


                        // Changes text color of the element's children
                        for (int j = 0; j < g.transform.childCount; j++)
                        {
                            if (g.transform.GetChild(j).GetComponent<TextMeshProUGUI>())
                                g.transform.GetChild(j).GetComponent<TextMeshProUGUI>().color = textSelectedColor;
                        }
                    }
                    // if the checked element is not the currently selected one
                    else
                    {
                        if (g.GetComponent<Image>())
                            g.GetComponent<Image>().color = buttonDefaultColor;


                        // Changes text color of the element's children
                        for (int j = 0; j < g.transform.childCount; j++)
                        {
                            if (g.transform.GetChild(j).GetComponent<TextMeshProUGUI>())
                                g.transform.GetChild(j).GetComponent<TextMeshProUGUI>().color = textDefaultColor;
                        }
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
                return true;
        }


        return false;
    }

    int FindObjectIndexInGameObjectArray(GameObject objectToFind, GameObject[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == objectToFind)
                return i;
        }


        return 0;
    }
    #endregion
    #endregion
}