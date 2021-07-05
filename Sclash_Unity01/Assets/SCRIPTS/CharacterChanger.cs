using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using TMPro;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;


// HEADER
// For Sclash
// OPTIMIZED

// REQUIREMENTS
// Photon Unity
// TextMeshPro package
// Player script
// GameManager script (Single instance)
// ConnetManager script (Single instance)

/// <summary>
/// This script, placed on the character, allows for changing the character before the battle, and also affects the UI elements that display the characters
/// </summary>

// VERSION
// Originally made for Unity 2019.14
public class CharacterChanger : MonoBehaviourPunCallbacks
{
    #region VARIABLES
    [Header("PLAYER COMPONENTS")]
    [SerializeField] Player playerScript = null;
    [SerializeField] Animator playerAnimator = null;
    [SerializeField] Animator legsAnimator = null;
    [SerializeField] public Animator characterChangeAnimator = null;
    [SerializeField] public SpriteRenderer mask = null;
    [SerializeField] public SpriteRenderer weapon = null;
    [SerializeField] public SpriteRenderer sheath = null;
    [SerializeField] GameObject scarfPrefab = null;
    [SerializeField] Transform scarfFollowPoint = null;
    GameObject scarfObj = null;
    // [SerializeField] GameObject scarf = null;
    bool hasScarf = false;


    [Header("DATA")]
    [SerializeField] public CharactersDatabase charactersDatabase = null;
    [SerializeField] public MasksDatabase masksDatabase = null;
    [SerializeField] public WeaponsDatabase weaponsDatabase = null;
    [SerializeField] public Player2ModesList player2ModesDatabase = null;


    // VERTICAL SELECTION
    bool canChangeVertical = true;
    [HideInInspector] public int verticalIndex = 0;
    int currentMaxVerticalIndex = 0;
    List<Animator> verticalElements = new List<Animator>();


    [Header("HORIZONTAL SWITCH")]
    [SerializeField] float switchDelay = 0.07f;
    float currentSwitchDelay = 0;
    bool canChangeHorizontal = true;
    int currentMaskIndex = 0;
    int lastChosenMaskIndex = 0;
    [SerializeField] int currentWeaponIndex = 0;
    int lastWeaponIndex = 0;
    [SerializeField] public int currentCharacterIndex = 0;
    int lastChosenCharacterIndex = 0;
    [HideInInspector] public int currentAI_Index = 0;
    int lastAI_Index = 0;


    [Header("CHARACTER CHANGE UI VISUAL OBJECTS NAMES")]
    [SerializeField] string charaSelecMenuName = "CharaSelecMenu";
    [SerializeField] string characterElementName = "Character";
    [SerializeField] string maskElementName = "Mask";
    [SerializeField] string weaponElementName = "Weapon";
    [SerializeField] string player2ElementName = "Player2";
    [SerializeField] string character2ElementName = "Character2";
    [SerializeField] string fullObjectName = "CharacterChange";
    [SerializeField] string illustrationName = "CharacterChangeIllustration";
    [SerializeField] string nameName = "CharacterChangeName";



    // UI DISPLAY
    GameObject verticalMenu = null;
    [HideInInspector] public List<Animator> fullObjectsAnimators = new List<Animator>();
    [HideInInspector] public List<Image> illustrationsUIObjects = new List<Image>();
    [HideInInspector] public List<TextMeshProUGUI> UICharacternames = new List<TextMeshProUGUI>();



    [Header("AUDIO")]
    [SerializeField] AudioSource wooshAudioSource = null;
    #endregion

    CharacterChanger o_CharacterChanger = null;
    int[] bufferedValues = new int[0];
    public const byte ApplyCosmeticChanges = 1;

    private bool IsConnected
    {
        get { return ConnectManager.Instance != null && ConnectManager.Instance.connectedToMaster; }
    }

    private bool IsLocal
    {
        get { return photonView != null && photonView.IsMine; }
    }






    #region FUNCTIONS
    new void OnEnable()                                                                                                                                                          // ON ENABLE
    {
        ConnectManager.PlayerJoined += FetchChanger;

        //IAChanger iachanger = GetComponent<IAChanger>();
        //iachanger.SwitchIAMode(false);
        //iachanger.enabled = false;


        // Disable AI
        GetComponent<IAScript>().enabled = false;


        // SELECTION INDEXES
        currentCharacterIndex = lastChosenCharacterIndex;
        currentMaskIndex = lastChosenMaskIndex;
        currentWeaponIndex = lastWeaponIndex;
        currentAI_Index = lastAI_Index;

        /*
        if (characterChangeAnimator != null)
            characterChangeAnimator.enabled = true;
        else
            Debug.Log("Couldn't find the character change display animator, ignoring");
        */


        // GET & ACTIVATE UI DISPLAY
        FindElements();
        //fullObjectsAnimators[playerScript.playerNum].SetBool("On", true);
        EnableVisuals(true);




        // Check if one has drawn
        if (playerScript.playerState == Player.STATE.sneathed && playerScript.oldState == Player.STATE.sneathing)
        { }
        else
        {
            // Mask
            if (verticalElements.Count > 1)
            {
                verticalIndex = 1;
                StartCoroutine(ApplyMaskChange(0));
            }


            // Weapon
            if (verticalElements.Count > 2)
            {
                verticalIndex = 2;
                StartCoroutine(ApplyWeaponChange(0));
            }
        }

        // Player 2
        if (playerScript.playerNum == 0)
        {
            verticalIndex = 3;
            StartCoroutine(ApplyPlayerChange(0));

            if (ConnectManager.Instance != null)
            {
                if (!ConnectManager.Instance.enableMultiplayer)
                {
                    verticalIndex = 4;
                    StartCoroutine(ApplyCharacter2Change(0));
                }
            }
        }

        if (playerScript.playerState == Player.STATE.sneathed && playerScript.oldState == Player.STATE.sneathing)
            verticalIndex = 0;
        else
        {
            // Character
            verticalIndex = 0;
            StartCoroutine(ApplyCharacterChange(0));
        }




        // SET UP ALL ELEMENTS OF THE MENU
        SelectVerticalElement();


        // Switch delay setup
        currentSwitchDelay = switchDelay;
    }


    new private void OnDisable()                                                                                                                                              // ON DISABLE
    {
        ConnectManager.PlayerJoined -= FetchChanger;

        /*
        if (fullObjectsAnimators[playerScript.playerNum])
            fullObjectsAnimators[playerScript.playerNum].SetBool("On", false);
        if (characterChangeAnimator)
            characterChangeAnimator.enabled = false;
            */

        if (verticalMenu != null)
            verticalMenu.GetComponent<Animator>().SetBool("On", false);
    }

    private void Awake()
    {
        playerScript = GetComponent<Player>();
    }

    void Update()                                                                                                                                                               // UPDATE
    {
        if (enabled & isActiveAndEnabled)
        {
            if (verticalMenu != null && verticalMenu.GetComponent<Animator>())
                verticalMenu.GetComponent<Animator>().SetBool("On", true);
            ManageVerticalSelectionChange();
            ManageHorizontalSwitch();
        }
    }


    private void OnDestroy()
    {
        // SCARF
        if (scarfObj != null)
        {
            Destroy(scarfObj.gameObject);
            if (playerScript != null)
                playerScript.scarfRenderer = null;
        }
    }





    // ANIMATION
    public void EnableVisuals(bool state)                                                                                                                                       // ENABLE VISUALS
    {
        if (fullObjectsAnimators != null && fullObjectsAnimators.Count > playerScript.playerNum && fullObjectsAnimators[playerScript.playerNum])
            fullObjectsAnimators[playerScript.playerNum].SetBool("On", state);
        if (characterChangeAnimator)
            characterChangeAnimator.enabled = state;


        // ACTIVATE ALL BUTTONS
        if (verticalElements != null && verticalElements.Count > 0)
            for (int i = 0; i < verticalElements.Count; i++)
                if (verticalElements[i] != null)
                    verticalElements[i].SetBool("Disabled", false);
    }







    public void FindElements()                                                                                                                                              // FIND ELEMENTS
    {
        for (int i = 0; i < 2; i++)
        {
            if (GameObject.Find(fullObjectName + i))
                fullObjectsAnimators.Add(GameObject.Find(fullObjectName + i).GetComponent<Animator>());
            if (GameObject.Find(illustrationName + i))
                illustrationsUIObjects.Add(GameObject.Find(illustrationName + i).GetComponent<Image>());
            if (GameObject.Find(nameName + i))
                UICharacternames.Add(GameObject.Find(nameName + i).GetComponent<TextMeshProUGUI>());
        }


        // VERTICAL SELECTION
        verticalElements.Clear();
        verticalMenu = GameObject.Find(charaSelecMenuName + playerScript.playerNum);


        if (verticalMenu != null && verticalMenu.transform.childCount > 0)
            for (int i = 0; i < verticalMenu.transform.childCount; i++)
            {
                verticalElements.Add(verticalMenu.transform.GetChild(i).GetComponent<Animator>());
                verticalElements[verticalElements.Count - 1].GetComponent<CharaSelecMenuElement>().characterChanger = gameObject.GetComponent<CharacterChanger>();
                verticalElements[verticalElements.Count - 1].GetComponent<CharaSelecMenuElement>().index = verticalElements.Count - 1;
            }


        //SOLO
        if (verticalMenu != null)
        {
            currentMaxVerticalIndex = verticalMenu.transform.childCount - 1;
            // ONLINE
            if (ConnectManager.Instance != null)
                if (ConnectManager.Instance.enableMultiplayer && playerScript.playerNum == 0)
                    currentMaxVerticalIndex = verticalMenu.transform.childCount - 3;
        }

    }


    void ManageVerticalSelectionChange()                                                                                                                                   // MANAGE VERTICAL SELECTION CHANGE
    {
        // INPUT INDEX
        // Old input
        int inputPlayernUm = playerScript.playerNum;

        if (ConnectManager.Instance != null)
            if (ConnectManager.Instance.enableMultiplayer)
                inputPlayernUm = 0;


        if (canChangeVertical && (Mathf.Abs(InputManager.Instance.playerInputs[inputPlayernUm].vertical) > 0.5f))
        {
            VerticalSelectionChange(inputPlayernUm);
            canChangeVertical = false;
        }
        else if (!canChangeVertical && Mathf.Abs(InputManager.Instance.playerInputs[inputPlayernUm].vertical) < 0.5f)
            canChangeVertical = true;
    }


    public void VerticalSelectionChange(int playerIndex = 0)                                                                                                                // VERTICAL SELECTION CHANGE
    {
        if (InputManager.Instance.playerInputs[playerIndex].vertical > 0.5f)
            verticalIndex--;
        else if (InputManager.Instance.playerInputs[playerIndex].vertical < -0.5f)
            verticalIndex++;


        if (verticalIndex > currentMaxVerticalIndex)
            verticalIndex = 0;
        if (verticalIndex < 0)
            verticalIndex = currentMaxVerticalIndex;


        SelectVerticalElement();
    }


    void SelectVerticalElement()                                                                                                                                                // SELECT VERTICAL ELEMENT
    {
        for (int i = 0; i < verticalElements.Count; i++)
            if (i == verticalIndex)
                verticalElements[i].SetBool("On", true);
            else
                verticalElements[i].SetBool("On", false);
    }


    void ManageHorizontalSwitch()                                                                                                                                           // MANAGE HORIZONTAL SWITCH
    {
        // INPUT INDEX
        // Old input
        int inputPlayernUm = playerScript.playerNum;

        if (ConnectManager.Instance != null)
            if (ConnectManager.Instance.enableMultiplayer)
                inputPlayernUm = 0;


        if (canChangeHorizontal && (Mathf.Abs(InputManager.Instance.playerInputs[inputPlayernUm].horizontal) > 0.5f))
        {
            if (InputManager.Instance.playerInputs[inputPlayernUm].horizontal > 0.5f)
                HorizontalSwitch(1);
            else if (InputManager.Instance.playerInputs[inputPlayernUm].horizontal < -0.5f)
                HorizontalSwitch(-1);


            canChangeHorizontal = false;
        }
        else if (!canChangeHorizontal && Mathf.Abs(InputManager.Instance.playerInputs[inputPlayernUm].horizontal) < 0.5f)
            canChangeHorizontal = true;
    }


    // Change current property after horizontal input
    public void HorizontalSwitch(int direction)                                                                                                                                     // HORIZONTAL SWITCH
    {
        int changeDirection = 0;


        // AUDIO
        // ONLINE
        if (ConnectManager.Instance != null && ConnectManager.Instance.enableMultiplayer)
        {
            //Play the woosh only if you're the one changing character
            if (GetComponent<PhotonView>() && GetComponent<PhotonView>().IsMine)
                if (wooshAudioSource)
                    wooshAudioSource.Play();
        }
        else
        {
            if (wooshAudioSource)
                wooshAudioSource.Play();
            else
                Debug.Log("Warning, character change woosh audio source not found, can't play the sound, continuing anyway");
        }


        // Change direction
        if (direction >= 1)
        {
            changeDirection = 1;
            // ANIMATION
            if (verticalElements.Count > verticalIndex)
                verticalElements[verticalIndex].SetTrigger("Right");
        }
        else if (direction <= -1)
        {
            changeDirection = -1;
            // ANIMATION
            if (verticalElements.Count > verticalIndex)
                verticalElements[verticalIndex].SetTrigger("Left");
        }


        // CHECK WHAT TYPE OF ELEMENT
        string nameOfElement = "";
        if (verticalElements != null && verticalElements.Count > verticalIndex)
            nameOfElement = verticalElements[verticalIndex].gameObject.name;


        // CHARACTER
        if (nameOfElement == characterElementName && GameManager.Instance.playersList[playerScript.otherPlayerNum].GetComponent<CharacterChanger>().currentAI_Index == 0)
            StartCoroutine(ApplyCharacterChange(changeDirection));
        else if (nameOfElement == maskElementName)
            StartCoroutine(ApplyMaskChange(changeDirection));
        else if (nameOfElement == weaponElementName)
            StartCoroutine(ApplyWeaponChange(changeDirection));
        else if (nameOfElement == player2ElementName)
            StartCoroutine(ApplyPlayerChange(changeDirection));
        else if (nameOfElement == character2ElementName)
            StartCoroutine(ApplyCharacter2Change(changeDirection));
    }


    void ManageCharacterChange()                                                                                                                                                 // MANAGE CHARACTER CHANGE
    {
        // IF CORRECT INPUT DETECTED, CHANGE CHARACTER
        if (canChangeHorizontal && (Mathf.Abs(InputManager.Instance.playerInputs[playerScript.playerNum].horizontal) > 0.5f))
        {
            //StartCoroutine(ApplyCharacterChange());
            canChangeHorizontal = false;
        }
        else if (!canChangeHorizontal && Mathf.Abs(InputManager.Instance.playerInputs[playerScript.playerNum].horizontal) < 0.5f)
            canChangeHorizontal = true;
    }








    #region CHANGES
    // CHARACTER
    public IEnumerator ApplyCharacterChange(int direction)                                                                                                                      // APPLY CHARACTER CHANGE
    {

        // Get the script with the references to the displays of this element
        CharaSelecMenuElement elementScript = null;
        if (verticalElements != null && verticalElements.Count > verticalIndex)
            elementScript = verticalElements[verticalIndex].GetComponent<CharaSelecMenuElement>();


        // INDEX
        currentCharacterIndex += direction;
        if (currentCharacterIndex > charactersDatabase.charactersList.Count - 1)
            currentCharacterIndex = 0;
        if (currentCharacterIndex < 0)
            currentCharacterIndex = charactersDatabase.charactersList.Count - 1;
        lastChosenCharacterIndex = currentCharacterIndex;


        // ANIMATION
        if (direction >= 1)
        {
            if (fullObjectsAnimators.Count > 0)
                fullObjectsAnimators[playerScript.playerNum].SetTrigger("Right");
            if (characterChangeAnimator != null)
                characterChangeAnimator.SetTrigger("Right");
        }
        else if (direction <= -1)
        {
            if (fullObjectsAnimators.Count > 0)
                fullObjectsAnimators[playerScript.playerNum].SetTrigger("Left");
            if (characterChangeAnimator != null)
                characterChangeAnimator.SetTrigger("Left");
        }



        // MASK & WEAPON MENU CHANGE ANIM
        if (direction >= 1)
        {
            verticalElements[1].SetTrigger("Right");
            verticalElements[2].SetTrigger("Right");
        }
        else if (direction <= -1)
        {
            verticalElements[1].SetTrigger("Left");
            verticalElements[2].SetTrigger("Left");
        }



        // FX
        //playerScript.chargeFullKatanaFX.main.startColor.mode = ParticleSystemGradientMode.RandomColor;
        ParticleSystem.MinMaxGradient randomColor = charactersDatabase.charactersList[currentCharacterIndex].character.saberFXColor;
        randomColor.mode = ParticleSystemGradientMode.RandomColor;
        ParticleSystem.MainModule mainModule;
        //ParticleSystem particleSystem;

        //particleSystem = playerScript.chargeFullKatanaFX;
        mainModule = playerScript.chargeFullKatanaFX.main;
        mainModule.startColor = randomColor;

        mainModule = playerScript.chargeBoomKatanaFX.main;
        mainModule.startColor = randomColor;

        mainModule = playerScript.chargeKatanaFX.main;
        mainModule.startColor = randomColor;




        // WAIT FOR CHANGE ANIM
        yield return new WaitForSeconds(currentSwitchDelay);


        // NAME
        if (elementScript && charactersDatabase)
        {
            if (elementScript.textApparitionComponent && charactersDatabase.charactersList[currentCharacterIndex].character)
            {
                elementScript.textApparitionComponent.textKey = charactersDatabase.charactersList[currentCharacterIndex].character.nameKey;
                elementScript.textApparitionComponent.TransfersTrad();
            }
            else if (elementScript.text1 != null)
                elementScript.text1.text = charactersDatabase.charactersList[currentCharacterIndex].name;
        }



        // WAIT FOR CHANGE ANIM 2
        //yield return new WaitForSeconds(0.05f);


        // PLAYER ANIMATORS
        if (playerAnimator != null)
            playerAnimator.runtimeAnimatorController = charactersDatabase.charactersList[currentCharacterIndex].animator;
        if (legsAnimator != null)
            legsAnimator.runtimeAnimatorController = charactersDatabase.charactersList[currentCharacterIndex].legsAnimator;


        // CHANGE MASK & WEAPON INDEX
        if (MapLoader.Instance != null && MapLoader.Instance.halloween) // Halloween
        {
            currentMaskIndex = 6;
            currentWeaponIndex = 1;
        }
        else // Default
        {
            currentMaskIndex = charactersDatabase.charactersList[currentCharacterIndex].defaultMask;
            currentWeaponIndex = charactersDatabase.charactersList[currentCharacterIndex].defaultWeapon;
        }
        if (mask != null && masksDatabase != null) // Mask
        {
            mask.sprite = masksDatabase.masksList[currentMaskIndex].sprite;
            if (verticalElements[1] != null)
            {
                //verticalElements[1].GetComponent<CharaSelecMenuElement>().text1.text = masksDatabase.masksList[currentMaskIndex].name;
                verticalElements[1].GetComponent<CharaSelecMenuElement>().textApparitionComponent.textKey = masksDatabase.masksList[currentMaskIndex].maskScriptableObject.keyName;
                verticalElements[1].GetComponent<CharaSelecMenuElement>().textApparitionComponent.TransfersTrad();
            }
        }
        if (weaponsDatabase != null) // WEAPONS
        {
            if (weapon != null)
                weapon.sprite = weaponsDatabase.weaponsList[currentWeaponIndex].sprite;
            if (sheath != null)
                sheath.sprite = weaponsDatabase.weaponsList[currentWeaponIndex].sheathSprite;
            //verticalElements[2].GetComponent<CharaSelecMenuElement>().text1.text = weaponsDatabase.weaponsList[currentWeaponIndex].name;
            verticalElements[2].GetComponent<CharaSelecMenuElement>().textApparitionComponent.textKey = weaponsDatabase.weaponsList[currentWeaponIndex].weaponScriptableObject.nameKey;
            verticalElements[2].GetComponent<CharaSelecMenuElement>().textApparitionComponent.TransfersTrad();
        }


        if (verticalElements != null && verticalElements.Count > 2)
        {
            if (verticalElements[1] != null && GetComponent<CharaSelecMenuElement>() && verticalElements[1].GetComponent<CharaSelecMenuElement>().text1)
                verticalElements[1].GetComponent<CharaSelecMenuElement>().text1.text = masksDatabase.masksList[currentMaskIndex].name;
            if (verticalElements[2] != null && GetComponent<CharaSelecMenuElement>() && verticalElements[2].GetComponent<CharaSelecMenuElement>().text1)
                verticalElements[2].GetComponent<CharaSelecMenuElement>().text1.text = weaponsDatabase.weaponsList[currentWeaponIndex].name;
        }


        // SCARF
        if (scarfObj != null)
        {
            Destroy(scarfObj.gameObject);
            if (playerScript != null)
                playerScript.scarfRenderer = null;
        }

        if (charactersDatabase != null)
            hasScarf = charactersDatabase.charactersList[currentCharacterIndex].scarf;
        if (hasScarf)
        {
            if (scarfPrefab != null)
            {
                if (scarfPrefab != null)
                    scarfObj = Instantiate(scarfPrefab);
                if (playerScript != null)
                    playerScript.scarfRenderer = scarfObj.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();

                ConstraintSource src = new ConstraintSource();
                src.sourceTransform = scarfFollowPoint;
                src.weight = 1;

                if (scarfObj != null && scarfObj.GetComponent<ParentConstraint>())
                    scarfObj.GetComponent<ParentConstraint>().SetSource(0, src);
            }
            else
                Debug.Log("Couldn't find character scarf, ignoring");
        }
        else if (!hasScarf && scarfObj != null)
        {
            Destroy(scarfObj.gameObject);
            if (playerScript != null)
                playerScript.scarfRenderer = null;
        }

        // SCARF
        // if (scarf != null)
        // {
        //     scarf.SetActive(charactersDatabase.charactersList[currentCharacterIndex].scarf);
        // }
        // else
        //    



        // UI DISPLAY
        if (charactersDatabase != null)
        {
            if (illustrationsUIObjects != null && illustrationsUIObjects.Count > 0)
                illustrationsUIObjects[playerScript.playerNum].sprite = charactersDatabase.charactersList[currentCharacterIndex].illustration;
            if (UICharacternames != null && UICharacternames.Count > 0)
            {
                //UICharacternames[playerScript.playerNum].text = charactersDatabase.charactersList[currentCharacterIndex].name;
                UICharacternames[playerScript.playerNum].GetComponent<TextApparition>().textKey = charactersDatabase.charactersList[currentCharacterIndex].character.nameKey;
                UICharacternames[playerScript.playerNum].GetComponent<TextApparition>().TransfersTrad();
            }
        }


        // SCRIPT VALUES
        if (playerScript != null)
        {
            if (playerScript.characterNameDisplay != null)
                playerScript.characterNameDisplay.text = charactersDatabase.charactersList[currentCharacterIndex].name;
            playerScript.characterIndex = currentCharacterIndex;
            if (GameManager.Instance != null && GameManager.Instance.scoresNames != null && GameManager.Instance.scoresNames.Count > playerScript.playerNum)
                if (GameManager.Instance.scoresNames[playerScript.playerNum] != null && charactersDatabase != null && charactersDatabase.charactersList.Count > currentCharacterIndex)
                    GameManager.Instance.scoresNames[playerScript.playerNum].text = charactersDatabase.charactersList[currentCharacterIndex].name;
        }

        SendCosmetics();
    }


    // MASK
    public IEnumerator ApplyMaskChange(int direction)                                                                                                                           // APPLY MASK CHANGE
    {
        // Get the script with the references to the displays of this element
        CharaSelecMenuElement elementScript = verticalElements[verticalIndex].GetComponent<CharaSelecMenuElement>();


        // INDEX
        currentMaskIndex += direction;
        if (currentMaskIndex > masksDatabase.masksList.Count - 1)
            currentMaskIndex = 0;
        if (currentMaskIndex < 0)
            currentMaskIndex = masksDatabase.masksList.Count - 1;
        lastChosenMaskIndex = currentMaskIndex;


        // ANIMATION
        if (direction >= 1) // RIGHT
        {
            if (characterChangeAnimator != null)
                characterChangeAnimator.SetTrigger("MaskRight");
            else
                Debug.Log("Character change animator not found, ignoring");
        }
        else if (direction <= -1) // LEFT
            if (characterChangeAnimator != null)
                characterChangeAnimator.SetTrigger("MaskLeft");
            else
                Debug.Log("Character change animator not found, ignoring");



        // WAIT FOR CHANGE ANIM
        yield return new WaitForSeconds(currentSwitchDelay);



        // NAME
        if (elementScript.textApparitionComponent && masksDatabase.masksList[currentMaskIndex].maskScriptableObject)
        {
            elementScript.textApparitionComponent.textKey = masksDatabase.masksList[currentMaskIndex].maskScriptableObject.keyName;
            elementScript.textApparitionComponent.TransfersTrad();
        }
        else
            elementScript.text1.text = masksDatabase.masksList[currentMaskIndex].name;


        // SPRITE
        mask.sprite = masksDatabase.masksList[currentMaskIndex].sprite;
        SendCosmetics();
    }


    // WEAPON
    public IEnumerator ApplyWeaponChange(int direction)
    {
        // Get the script with the references to the displays of this element
        CharaSelecMenuElement elementScript = verticalElements[verticalIndex].GetComponent<CharaSelecMenuElement>();


        // INDEX
        currentWeaponIndex += direction;
        if (currentWeaponIndex > weaponsDatabase.weaponsList.Count - 1)
            currentWeaponIndex = 0;
        if (currentWeaponIndex < 0)
            currentWeaponIndex = weaponsDatabase.weaponsList.Count - 1;
        lastWeaponIndex = currentWeaponIndex;


        // ANIMATION
        if (direction >= 1)
        {
            if (characterChangeAnimator != null)
                characterChangeAnimator.SetTrigger("WeaponRight");
            else
                Debug.Log("Character change animator not found, ignoring");
        }
        else if (direction <= -1)
            if (characterChangeAnimator != null)
                characterChangeAnimator.SetTrigger("WeaponLeft");
            else
                Debug.Log("Character change animator not found, ignoring");



        // WAIT FOR CHANGE ANIM
        yield return new WaitForSeconds(currentSwitchDelay);


        // NAME
        if (weaponsDatabase.weaponsList[currentWeaponIndex].weaponScriptableObject && elementScript.textApparitionComponent)
        {
            elementScript.textApparitionComponent.textKey = weaponsDatabase.weaponsList[currentWeaponIndex].weaponScriptableObject.nameKey;
            elementScript.textApparitionComponent.TransfersTrad();
        }
        else
            elementScript.text1.text = weaponsDatabase.weaponsList[currentWeaponIndex].name;


        // SPRITE
        weapon.sprite = weaponsDatabase.weaponsList[currentWeaponIndex].sprite;
        sheath.sprite = weaponsDatabase.weaponsList[currentWeaponIndex].sheathSprite;

        SendCosmetics();
    }                                                                                                                       // APPLY WEAPON CHANGE


    // PLAYER   
    public IEnumerator ApplyPlayerChange(int direction)                                                                                                                         // APPLY PLAYER CHANGE
    {
        // Get the script with the references to the displays of this element
        CharaSelecMenuElement elementScript = null;
        if (verticalElements.Count > verticalIndex)
            elementScript = verticalElements[verticalIndex].GetComponent<CharaSelecMenuElement>();


        // INDEX
        currentAI_Index += direction;
        if (currentAI_Index > player2ModesDatabase.player2modes.Count - 1)
            currentAI_Index = 0;
        if (currentAI_Index < 0)
            currentAI_Index = player2ModesDatabase.player2modes.Count - 1;
        lastAI_Index = currentAI_Index;


        // WAIT FOR CHANGE ANIM
        yield return new WaitForSeconds(currentSwitchDelay);



        // NAMES
        if (elementScript != null)
        {
            if (elementScript.textApparitionComponent)
            {
                elementScript.textApparitionComponent.textKey = player2ModesDatabase.player2modes[currentAI_Index].nameKey;
                elementScript.textApparitionComponent.TransfersTrad();
            }
            else
                elementScript.text1.text = player2ModesDatabase.player2modes[currentAI_Index].name;
        }
        if (GameManager.Instance != null && verticalElements != null && verticalElements.Count > 0 && verticalElements[verticalElements.Count - 1].GetComponent<CharaSelecMenuElement>())
        {
            //verticalElements[verticalElements.Count - 1].GetComponent<CharaSelecMenuElement>().text1.text = charactersDatabase.charactersList[GameManager.Instance.playersList[playerScript.otherPlayerNum].GetComponent<Player>().characterChanger.currentCharacterIndex].name;
            verticalElements[verticalElements.Count - 1].GetComponent<CharaSelecMenuElement>().textApparitionComponent.textKey = charactersDatabase.charactersList[GameManager.Instance.playersList[playerScript.otherPlayerNum].GetComponent<Player>().characterChanger.currentCharacterIndex].character.nameKey;
            verticalElements[verticalElements.Count - 1].GetComponent<CharaSelecMenuElement>().textApparitionComponent.TransfersTrad();
        }



        // ENABLE CHARACTER CHANGE
        if (verticalElements != null && verticalElements.Count > 0)
            verticalElements[verticalElements.Count - 1].SetBool("Disabled", !player2ModesDatabase.player2modes[currentAI_Index].AI);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.playersList[playerScript.otherPlayerNum].GetComponent<Player>().characterChanger.enabled = !player2ModesDatabase.player2modes[currentAI_Index].AI;
            GameManager.Instance.playersList[playerScript.otherPlayerNum].GetComponent<AIBehaviors>().enabled = player2ModesDatabase.player2modes[currentAI_Index].AI;
        }
        playerScript.iaScript.SetDifficulty(player2ModesDatabase.player2modes[currentAI_Index].difficulty);


        // ONLINE, NO IA CHOICE
        if (ConnectManager.Instance.enableMultiplayer)
        {
            if (verticalElements != null && verticalElements.Count > 0)
                verticalElements[verticalElements.Count - 2].SetBool("Disabled", true);
        }
        else
        {
            if (!player2ModesDatabase.player2modes[currentAI_Index].AI)
                currentMaxVerticalIndex = verticalElements.Count - 2;
            else
                currentMaxVerticalIndex = verticalElements.Count - 1;
        }
    }



    // CHARACTER 2
    public IEnumerator ApplyCharacter2Change(int direction)                                                                                                                     // APPLY CHARACTER 2 CHANGE
    {
        // Get the script with the references to the displays of this element
        CharaSelecMenuElement elementScript = verticalElements[verticalIndex].GetComponent<CharaSelecMenuElement>();
        CharacterChanger otherCharacterChanger = null;
        if (GameManager.Instance != null)
            otherCharacterChanger = GameManager.Instance.playersList[playerScript.otherPlayerNum].GetComponent<Player>().characterChanger;
        if (otherCharacterChanger)
            StartCoroutine(otherCharacterChanger.ApplyCharacterChange(direction));

        yield return new WaitForSeconds(currentSwitchDelay);

        // NAME
        if (elementScript != null)
        {
            if (elementScript.textApparitionComponent && charactersDatabase.charactersList[otherCharacterChanger.currentCharacterIndex].character)
            {
                elementScript.textApparitionComponent.textKey = charactersDatabase.charactersList[otherCharacterChanger.currentCharacterIndex].character.nameKey;
                elementScript.textApparitionComponent.TransfersTrad();
            }
            else if (elementScript.text1 != null)
                elementScript.text1.text = charactersDatabase.charactersList[otherCharacterChanger.currentCharacterIndex].name;
        }
    }
    #endregion

    #region Photon
    private void SendCosmetics()
    {
        if (ConnectManager.Instance == null)
            return;

        if (ConnectManager.Instance != null && !ConnectManager.Instance.connectedToMaster)
            return;

        int[] content = new int[] { currentMaskIndex, currentCharacterIndex, currentWeaponIndex };

        Debug.LogFormat("Sending : {0} {1} {2}", content[0], charactersDatabase.charactersList[content[1]].name, content[2]);

        // RaiseEventOptions raiseEventOptions = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.Others };

        if (photonView != null)
            photonView.RPC("ApplyCosmetics", RpcTarget.OthersBuffered, content);
    }

    private void FetchChanger()
    {
        //Fetch other player Character changer
        CharacterChanger[] changers = FindObjectsOfType<CharacterChanger>();

        if (changers != null && changers.Length > 0)
            foreach (CharacterChanger c in changers)
                if (c != null && c != this)
                {
                    o_CharacterChanger = c;
                    Debug.Log("Character changer found!");
                    SendCosmetics();
                    break;
                }

        if (o_CharacterChanger == null)
        {
            Invoke("FetchChanger", 0.5f);
            Debug.LogWarning("Character changer not found, still researching");
        }
    }

    [PunRPC]
    private void ApplyCosmetics(int[] data)
    {
        if (photonView != null && !photonView.IsMine)
        {
            ReceiveCosmetics(data[0], data[1], data[2]);
        }
    }

    public void ReceiveCosmetics(int m, int c, int w)
    {
        currentMaskIndex = m;
        currentCharacterIndex = c;
        currentWeaponIndex = w;

        Debug.LogFormat("Received : {0} {1} {2}", m, charactersDatabase.charactersList[c].name, w);

        if (playerAnimator != null)
            playerAnimator.runtimeAnimatorController = charactersDatabase.charactersList[currentCharacterIndex].animator;
        if (legsAnimator != null)
            legsAnimator.runtimeAnimatorController = charactersDatabase.charactersList[currentCharacterIndex].legsAnimator;


        // CHANGE MASK & WEAPON INDEX
        if (MapLoader.Instance != null && MapLoader.Instance.halloween) // Halloween
        {
            currentMaskIndex = 6;
            currentWeaponIndex = 1;
        }

        if (mask != null && masksDatabase != null)
            mask.sprite = masksDatabase.masksList[currentMaskIndex].sprite;
        if (weaponsDatabase != null)
        {
            if (weapon != null)
                weapon.sprite = weaponsDatabase.weaponsList[currentWeaponIndex].sprite;
            if (sheath != null)
                sheath.sprite = weaponsDatabase.weaponsList[currentWeaponIndex].sheathSprite;
        }

        // SCARF
        if (charactersDatabase != null)
            hasScarf = charactersDatabase.charactersList[currentCharacterIndex].scarf;
        if (hasScarf)
        {
            if (scarfPrefab != null)
            {
                if (scarfPrefab != null)
                    scarfObj = Instantiate(scarfPrefab);
                if (playerScript != null)
                    playerScript.scarfRenderer = scarfObj.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();

                ConstraintSource src = new ConstraintSource();
                if (scarfFollowPoint != null)
                    src.sourceTransform = scarfFollowPoint;
                src.weight = 1;

                if (scarfObj != null && scarfObj.GetComponent<ParentConstraint>())
                    scarfObj.GetComponent<ParentConstraint>().SetSource(0, src);
            }
            else
                Debug.Log("Couldn't find character scarf, ignoring");
        }
        else if (!hasScarf && scarfObj != null)
        {
            Destroy(scarfObj);
            playerScript.scarfRenderer = null;
        }

        GameManager.Instance.UpdateNameAndColors(playerScript.playerNum);
    }
    #endregion


    #endregion
}
