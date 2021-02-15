using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


// Created for Unity 2019.1.1f1
// Menu browser script that allows browsing through elements in menus in 2D instead of 1D, and also has custom events to be called when overflowing to that it can interact with other menu browsers
// OPTIMIZED ?
public class MenuBrowser2D : MonoBehaviour
{
    #region VARIABLES
    [Header("MANAGERS")]
    [SerializeField] RumbleManager rumbleManager = null;



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
    [SerializeField] public int YIndex = 0;
    [SerializeField] public int XIndex = 0;
    [SerializeField] bool applyDefaultIndexOnEnable = false;
    [SerializeField] int defaultYIndex = 0;
    [SerializeField] int defaultXIndex = 0;
    int verticalDirection = 1;
    int horizontalDirection = 1;

    [SerializeField] bool invertVerticalAxis = false;
    //[SerializeField] bool invertHorizontalAxis = false;
    [SerializeField] bool swapAxis = false;
    #endregion



    #region SPECIAL
    [Header("SPECIAL")]
    [SerializeField] bool callSpecialElementWHenHorizontalOverflow = false;
    [SerializeField] public GameObject specialElement = null;
    [SerializeField] bool callSpecialElementWhenEnabled = true;
    [SerializeField] public Button enabledSpecialElement = null;
    #endregion



    #region INPUTS
    [Header("INPUTS")]
    [Tooltip("Names of the inputs axis created in the input settings")]
    [SerializeField] string horizontalAxis = "Horizontal";
    [SerializeField] string verticalAxis = "Vertical";
    [SerializeField] string backButton = "Back";
    [SerializeField] bool vAxisInUse = false;
    [SerializeField] bool hAxisInUse = true;

    [Tooltip("Dead zones for joystick browsing so that it's more comfortable")]
    [SerializeField] float verticalInputDetectionZone = 0.5f;
    [SerializeField] float verticalInputRestZone = 0.1f;
    [SerializeField] float horizontalInputDetectionZone = 0.5f;
    [SerializeField] float horizontalRestZone = 0.1f;
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



    [Header("RUMBLE SETTINGS")]
    [SerializeField] RumbleSettings browseRumbleSettings = null;



    [Header("SOUND")]
    [Tooltip("The PlayRandomSoundInList script to play a random sound from the given source on it whenever browsing through menu elements")]
    [SerializeField] PlayRandomSoundInList hoverSound = null;
    #endregion















    PlayerControls controls;

    #region FUNCTIONS
    #region BASE FUNCTIONS
    void Awake()
    {
        controls = GameManager.Instance.Controls;

        FixButtonColorUsageList();


        // BROWSING DIRECTION
        if (invertVerticalAxis)
            horizontalDirection = -horizontalDirection;


        if (invertVerticalAxis)
            verticalDirection = -verticalDirection;


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
        if (enabled && isActiveAndEnabled)
        {
            if (elements2D.Length > 0)
            {

                // H AXIS
                // Detects H axis let go
                if (Mathf.Abs(controls.UI.Navigate.ReadValue<Vector2>().x) <= horizontalRestZone)
                    hAxisInUse = false;


                // Move sliders with horizontal
                if (!hAxisInUse)
                {
                    if (controls.UI.Navigate.ReadValue<Vector2>().x > horizontalInputDetectionZone)
                    {
                        HorizontalBrowse(1);
                        hAxisInUse = true;
                    }
                    else if (controls.UI.Navigate.ReadValue<Vector2>().x < -horizontalInputDetectionZone)
                    {
                        HorizontalBrowse(-1);
                        hAxisInUse = true;
                    }
                }








                // V AXIS
                // Detects V axis let go
                if (Mathf.Abs(controls.UI.Navigate.ReadValue<Vector2>().y) <= verticalInputRestZone)
                    vAxisInUse = false;


                if (!vAxisInUse)
                {
                    // Detects positive V axis input
                    if (controls.UI.Navigate.ReadValue<Vector2>().y > verticalInputDetectionZone)
                    {
                        VerticalBrowse(1);
                        vAxisInUse = true;
                    }
                    // Detects negative V axis input
                    else if (controls.UI.Navigate.ReadValue<Vector2>().y < -verticalInputDetectionZone)
                    {
                        VerticalBrowse(-1);
                        vAxisInUse = true;
                    }
                }
            }


            // BACK
            if (canBack && controls.UI.Cancel.triggered)
                backElement.GetComponent<Button>().onClick.Invoke();
        }
    }


    // OnEnable is called each time the object is set from inactive to active
    void OnEnable()
    {
        if (callSpecialElementWhenEnabled && enabledSpecialElement != null)
            enabledSpecialElement.onClick.Invoke();


        if (applyDefaultIndexOnEnable)
        {
            XIndex = defaultXIndex;
            YIndex = defaultYIndex;
        }


        if (elements2D.Length > 0)
        {
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


        // VISUAL
        UpdateColors();


        // RUMBLE
        //rumbleManager.TriggerSimpleControllerVibrationForEveryone(rumbleManager.menuBrowseVibrationIntensity, rumbleManager.menuBrowseVibrationIntensity, rumbleManager.menuBrowseVibrationDuration);
        rumbleManager.Rumble(browseRumbleSettings);


        Select(true);
    }

    void HorizontalBrowse(int direction)
    {
        XIndex += horizontalDirection * direction;


        // AUDIO
        hoverSound.Play();


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


            // RUMBLE
            //rumbleManager.TriggerSimpleControllerVibrationForEveryone(rumbleManager.menuBrowseVibrationIntensity, rumbleManager.menuBrowseVibrationIntensity, rumbleManager.menuBrowseVibrationDuration);
            rumbleManager.Rumble(browseRumbleSettings);


            Select(true);
        }
    }

    public void SetXIndex(int newIndex)
    {
        XIndex = newIndex;
    }


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
            // If index out of range
            if (YIndex >= elements2D.Length)
                YIndex = elements2D.Length - 1;
            if (XIndex >= elements2D[YIndex].line.Length)
                XIndex = elements2D[YIndex].line.Length - 1;


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
        enabled = true;


        for (int i = 0; i < elements2D.Length; i++)
            if (isObjectInGameObjectArray(hoveredObject, elements2D[i].line))
            {
                XIndex = FindObjectIndexInGameObjectArray(hoveredObject, elements2D[i].line);
                YIndex = i;
            }


        if (elements2D[YIndex].line[XIndex].GetComponent<Button>() || elements2D[YIndex].line[XIndex].GetComponent<TMP_InputField>())
            Select(true);
        else
            Select(false);
    }
    #endregion




    // ADD & REMOVE ELEMENTS
    public void ChangeBackButton(GameObject newBackButton)
    {
        backElement = newBackButton;
    }





    #region COLORS / VISUAL
    public void FixButtonColorUsageList()
    { }


    // Updates the color of each element of the menu screen depending on wether it's selected or not
    void UpdateColors()
    {
        // Browses through all elements
        for (int i = 0; i < elements2D.Length; i++)
            for (int y = 0; y < elements2D[i].line.Length; y++)
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
                            if (g.transform.GetChild(j).GetComponent<TextMeshProUGUI>())
                                g.transform.GetChild(j).GetComponent<TextMeshProUGUI>().color = textSelectedColor;
                    }
                    // if the checked element is not the currently selected one
                    else
                    {
                        if (g.GetComponent<Image>())
                            g.GetComponent<Image>().color = buttonDefaultColor;


                        // Changes text color of the element's children
                        for (int j = 0; j < g.transform.childCount; j++)
                            if (g.transform.GetChild(j).GetComponent<TextMeshProUGUI>())
                                g.transform.GetChild(j).GetComponent<TextMeshProUGUI>().color = textDefaultColor;
                    }
                }
    }
    #endregion






    #region SECONDARY FUNCTIONS
    // Returns true if element is contained in referenced array
    bool isObjectInGameObjectArray(GameObject objectToFind, GameObject[] array)
    {
        for (int i = 0; i < array.Length; i++)
            if (array[i] == objectToFind)
                return true;


        return false;
    }

    int FindObjectIndexInGameObjectArray(GameObject objectToFind, GameObject[] array)
    {
        for (int i = 0; i < array.Length; i++)
            if (array[i] == objectToFind)
                return i;


        return 0;
    }
    #endregion
    #endregion
}