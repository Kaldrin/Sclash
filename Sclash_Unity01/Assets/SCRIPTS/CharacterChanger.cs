using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

// This script, placed on the character, allows for changing the character before the battle, and also affects the UI elements that display the characters
// OPTIMIZED
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
    [SerializeField] GameObject scarf = null;


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
    int currentWeaponIndex = 0;
    int lastWeaponIndex = 0;
    [HideInInspector] public int currentCharacterIndex = 0;
    int lastChosenCharacterIndex = 0;
    int currentAI_Index = 0;
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

    public const byte ApplyCosmeticChanges = 1;









    #region FUNCTIONS
    void Awake()                                                                                // AWAKE
    {
        // GameManager.Instance.ResetGameEvent +=
    }

    void OnEnable()                                                                             // ONE ENABLE
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

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



        // Mask
        if (verticalElements.Count > 1)
        {
            verticalIndex = 1;
            StartCoroutine(ApplyMaskChange(0));
        }
        else
            Debug.Log("Not enough vertical elements in character selection menu, index out of range");

        // Weapon
        if (verticalElements.Count > 2)
        {
            verticalIndex = 2;
            StartCoroutine(ApplyWeaponChange(0));
        }
        else
            Debug.Log("Not enough vertical elements in character selection menu, index out of range");

        // Player 2
        if (playerScript.playerNum == 0)
        {
            verticalIndex = 3;
            StartCoroutine(ApplyPlayerChange(0));

            verticalIndex = 4;
            StartCoroutine(ApplyCharacter2Change(0));
        }

        // Character
        verticalIndex = 0;
        StartCoroutine(ApplyCharacterChange(0));


        // SET UP ALL ELEMENTS OF THE MENU
        SelectVerticalElement();


        // Switch delay setup
        currentSwitchDelay = switchDelay;
    }


    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        /*
        if (fullObjectsAnimators[playerScript.playerNum])
            fullObjectsAnimators[playerScript.playerNum].SetBool("On", false);
        if (characterChangeAnimator)
            characterChangeAnimator.enabled = false;
            */

        if (verticalMenu != null)
            verticalMenu.GetComponent<Animator>().SetBool("On", false);
    }


    // Update is called once per frame
    void Update()
    {
        if (enabled & isActiveAndEnabled)
        {
            verticalMenu.GetComponent<Animator>().SetBool("On", true);
            ManageVerticalSelectionChange();
            ManageHorizontalSwitch();
        }
    }







    // ANIMATION
    public void EnableVisuals(bool state)
    {
        if (fullObjectsAnimators.Count > 0 && fullObjectsAnimators[playerScript.playerNum])
            fullObjectsAnimators[playerScript.playerNum].SetBool("On", state);
        if (characterChangeAnimator)
            characterChangeAnimator.enabled = state;
    }







    public void FindElements()
    {
        for (int i = 0; i < 2; i++)
        {
            fullObjectsAnimators.Add(GameObject.Find(fullObjectName + i).GetComponent<Animator>());
            illustrationsUIObjects.Add(GameObject.Find(illustrationName + i).GetComponent<Image>());
            UICharacternames.Add(GameObject.Find(nameName + i).GetComponent<TextMeshProUGUI>());
        }


        // VERTICAL SELECTION
        verticalElements.Clear();
        verticalMenu = GameObject.Find(charaSelecMenuName + playerScript.playerNum);


        for (int i = 0; i < verticalMenu.transform.childCount; i++)
        {
            verticalElements.Add(verticalMenu.transform.GetChild(i).GetComponent<Animator>());
            verticalElements[verticalElements.Count - 1].GetComponent<CharaSelecMenuElement>().characterChanger = gameObject.GetComponent<CharacterChanger>();
            verticalElements[verticalElements.Count - 1].GetComponent<CharaSelecMenuElement>().index = verticalElements.Count - 1;
        }

        currentMaxVerticalIndex = verticalMenu.transform.childCount - 1;
    }


    void ManageVerticalSelectionChange()
    {
        if (canChangeVertical && (Mathf.Abs(InputManager.Instance.playerInputs[playerScript.playerNum].vertical) > 0.5f))
        {
            VerticalSelectionChange();
            canChangeVertical = false;
        }
        else if (!canChangeVertical && Mathf.Abs(InputManager.Instance.playerInputs[playerScript.playerNum].vertical) < 0.5f)
            canChangeVertical = true;
    }


    public void VerticalSelectionChange()
    {
        if (InputManager.Instance.playerInputs[playerScript.playerNum].vertical > 0.5f)
            verticalIndex--;
        else if (InputManager.Instance.playerInputs[playerScript.playerNum].vertical < -0.5f)
            verticalIndex++;


        if (verticalIndex > currentMaxVerticalIndex)
            verticalIndex = 0;
        if (verticalIndex < 0)
            verticalIndex = currentMaxVerticalIndex;


        SelectVerticalElement();
    }


    void SelectVerticalElement()
    {
        for (int i = 0; i < verticalElements.Count; i++)
            if (i == verticalIndex)
                verticalElements[i].SetBool("On", true);
            else
                verticalElements[i].SetBool("On", false);
    }


    void ManageHorizontalSwitch()
    {
        if (canChangeHorizontal && (Mathf.Abs(InputManager.Instance.playerInputs[playerScript.playerNum].horizontal) > 0.5f))
        {
            if (InputManager.Instance.playerInputs[playerScript.playerNum].horizontal > 0.5f)
                HorizontalSwitch(1);
            else if (InputManager.Instance.playerInputs[playerScript.playerNum].horizontal < -0.5f)
                HorizontalSwitch(-1);


            canChangeHorizontal = false;
        }
        else if (!canChangeHorizontal && Mathf.Abs(InputManager.Instance.playerInputs[playerScript.playerNum].horizontal) < 0.5f)
            canChangeHorizontal = true;
    }


    // Change current property after horizontal input
    public void HorizontalSwitch(int direction)
    {
        int changeDirection = 0;


        // AUDIO
        if (wooshAudioSource)
            wooshAudioSource.Play();
        else
            Debug.Log("Warning, character change woosh audio source not found, can't play the sound, continuing anyway");


        // Change direction
        if (direction >= 1)
        {
            changeDirection = 1;
            // ANIMATION
            verticalElements[verticalIndex].SetTrigger("Right");
        }
        else if (direction <= -1)
        {
            changeDirection = -1;
            // ANIMATION
            verticalElements[verticalIndex].SetTrigger("Left");
        }


        // CHECK WHAT TYPE OF ELEMENT
        string nameOfElement = verticalElements[verticalIndex].gameObject.name;


        // CHARACTER
        if (nameOfElement == characterElementName)
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


    void ManageCharacterChange()
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
    public IEnumerator ApplyCharacterChange(int direction)
    {
        // Get the script with the references to the displays of this element
        CharaSelecMenuElement elementScript = verticalElements[verticalIndex].GetComponent<CharaSelecMenuElement>();


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



        // WAIT FOR CHANGE ANIM
        yield return new WaitForSeconds(currentSwitchDelay);


        // NAME
        elementScript.text1.text = charactersDatabase.charactersList[currentCharacterIndex].name;


        // WAIT FOR CHANGE ANIM 2
        //yield return new WaitForSeconds(0.05f);


        // PLAYER ANIMATORS
        if (playerAnimator != null)
            playerAnimator.runtimeAnimatorController = charactersDatabase.charactersList[currentCharacterIndex].animator;
        if (legsAnimator != null)
            legsAnimator.runtimeAnimatorController = charactersDatabase.charactersList[currentCharacterIndex].legsAnimator;


        // CHANGE MASK & WEAPON INDEX
        if (playerScript.gameManager.mapLoader.halloween) // Halloween
        {
            currentMaskIndex = 6;
            currentWeaponIndex = 1;
        }
        else // Default
        {
            currentMaskIndex = charactersDatabase.charactersList[currentCharacterIndex].defaultMask;
            currentWeaponIndex = charactersDatabase.charactersList[currentCharacterIndex].defaultWeapon;
        }
        mask.sprite = masksDatabase.masksList[currentMaskIndex].sprite;
        weapon.sprite = weaponsDatabase.weaponsList[currentWeaponIndex].sprite;
        sheath.sprite = weaponsDatabase.weaponsList[currentWeaponIndex].sheathSprite;
        verticalElements[1].GetComponent<CharaSelecMenuElement>().text1.text = masksDatabase.masksList[currentMaskIndex].name;
        verticalElements[2].GetComponent<CharaSelecMenuElement>().text1.text = weaponsDatabase.weaponsList[currentWeaponIndex].name;



        // SCARF
        if (scarf != null)
            scarf.SetActive(charactersDatabase.charactersList[currentCharacterIndex].scarf);
        else
            Debug.Log("Couldn't find character scarf, ignoring");



        // UI DISPLAY
        if (illustrationsUIObjects.Count > 0)
            illustrationsUIObjects[playerScript.playerNum].sprite = charactersDatabase.charactersList[currentCharacterIndex].illustration;
        if (UICharacternames.Count > 0)
            UICharacternames[playerScript.playerNum].text = charactersDatabase.charactersList[currentCharacterIndex].name;


        // SCRIPT VALUES
        playerScript.characterNameDisplay.text = charactersDatabase.charactersList[currentCharacterIndex].name;
        playerScript.characterIndex = currentCharacterIndex;
        playerScript.gameManager.scoresNames[playerScript.playerNum].text = charactersDatabase.charactersList[currentCharacterIndex].name;
    }


    // MASK
    public IEnumerator ApplyMaskChange(int direction)
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
        elementScript.text1.text = masksDatabase.masksList[currentMaskIndex].name;


        // SPRITE
        mask.sprite = masksDatabase.masksList[currentMaskIndex].sprite;
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
        elementScript.text1.text = weaponsDatabase.weaponsList[currentWeaponIndex].name;


        // SPRITE
        weapon.sprite = weaponsDatabase.weaponsList[currentWeaponIndex].sprite;
    }


    // PLAYER
    public IEnumerator ApplyPlayerChange(int direction)
    {
        // Get the script with the references to the displays of this element
        CharaSelecMenuElement elementScript = verticalElements[verticalIndex].GetComponent<CharaSelecMenuElement>();


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
        elementScript.text1.text = player2ModesDatabase.player2modes[currentAI_Index].name;
        verticalElements[verticalElements.Count - 1].GetComponent<CharaSelecMenuElement>().text1.text = charactersDatabase.charactersList[playerScript.gameManager.playersList[playerScript.otherPlayerNum].GetComponent<Player>().characterChanger.currentCharacterIndex].name;

        // ENABLE CHARACTER CHANGE
        verticalElements[verticalElements.Count - 1].SetBool("Disabled", !player2ModesDatabase.player2modes[currentAI_Index].AI);
        playerScript.gameManager.playersList[playerScript.otherPlayerNum].GetComponent<Player>().characterChanger.enabled = !player2ModesDatabase.player2modes[currentAI_Index].AI;
        playerScript.gameManager.playersList[playerScript.otherPlayerNum].GetComponent<Player>().iaScript.enabled = player2ModesDatabase.player2modes[currentAI_Index].AI;
        playerScript.iaScript.SetDifficulty(player2ModesDatabase.player2modes[currentAI_Index].difficulty);

        if (!player2ModesDatabase.player2modes[currentAI_Index].AI)
        {
            currentMaxVerticalIndex = verticalElements.Count - 2;
        }
        else
            currentMaxVerticalIndex = verticalElements.Count - 1;
    }



    // CHARACTER 2
    public IEnumerator ApplyCharacter2Change(int direction)
    {
        // Get the script with the references to the displays of this element
        CharaSelecMenuElement elementScript = verticalElements[verticalIndex].GetComponent<CharaSelecMenuElement>();
        CharacterChanger otherCharacterChanger = playerScript.gameManager.playersList[playerScript.otherPlayerNum].GetComponent<Player>().characterChanger;
        StartCoroutine(otherCharacterChanger.ApplyCharacterChange(direction));

        yield return new WaitForSeconds(currentSwitchDelay);

        elementScript.text1.text = charactersDatabase.charactersList[otherCharacterChanger.currentCharacterIndex].name;
    }
    #endregion

    #region Photon
    private void SendCosmetics()
    {
        int[] content = new int[] { currentMaskIndex, currentCharacterIndex, currentWeaponIndex };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

        PhotonNetwork.RaiseEvent(ApplyCosmeticChanges, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == ApplyCosmeticChanges)
        {
            int[] data = (int[])photonEvent.CustomData;
            currentMaskIndex = data[0];
            currentCharacterIndex = data[1];
            currentWeaponIndex = data[2];
        }
    }
    #endregion


    #endregion


}
