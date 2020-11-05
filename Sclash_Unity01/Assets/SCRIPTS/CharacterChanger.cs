using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


// This script, placed on the character, allows for changing the character before the battle, and also affects the UI elements that display the characters
// OPTIMIZED
public class CharacterChanger : MonoBehaviour
{
    #region VARIABLES
    [Header("PLAYER COMPONENTS")]
    [SerializeField] Player playerScript = null;
    [SerializeField] Animator playerAnimator = null;
    [SerializeField] Animator legsAnimator = null;
    [SerializeField] Animator characterChangeAnimator = null;
    [SerializeField] public SpriteRenderer mask = null;
    [SerializeField] public SpriteRenderer weapon = null;


    [Header("DATA")]
    [SerializeField] public CharactersDatabase charactersDatabase = null;
    [SerializeField] public MasksDatabase masksDatabase = null;
    [SerializeField] public WeaponsDatabase weaponsDatabase = null;


    [Header("CHARACTER CHANGE UI VISUAL OBJECTS NAMES")]
    [SerializeField] string fullObjectName = "CharacterChange";
    [SerializeField] string illustrationName = "CharacterChangeIllustration";
    [SerializeField] string nameName = "CharacterChangeName";


    // UI DISPLAY
    [HideInInspector] public List<Animator> fullObjectsAnimators = new List<Animator>();
    [HideInInspector] public List<Image> illustrationsUIObjects = new List<Image>();
    [HideInInspector] public List<TextMeshProUGUI> UICharacternames = new List<TextMeshProUGUI>();


    [Header("OTHER")]
    [SerializeField] int defaultCharacterIndex = 0;
    [SerializeField] float changeDelay = 0.1f;
    [HideInInspector] public int currentCharacter = 0;
    int lastChosenCharacterIndex = 0;
    bool canChange = true;



    [Header("AUDIO")]
    [SerializeField] AudioSource characterChangeWooshAudioSource = null;
    #endregion












    void Awake()
    {
        // GameManager.Instance.ResetGameEvent +=
    }

    void OnEnable()
    {
        IAChanger iachanger = GetComponent<IAChanger>();
        iachanger.SwitchIAMode(false);
        iachanger.enabled = false;

        GetComponent<IAScript>().enabled = false;


        currentCharacter = lastChosenCharacterIndex;
        characterChangeAnimator.enabled = true;


        FindElements();

        fullObjectsAnimators[playerScript.playerNum].SetBool("On", true);
    }


    private void OnDisable()
    {
        if (fullObjectsAnimators[playerScript.playerNum])
            fullObjectsAnimators[playerScript.playerNum].SetBool("On", false);
        if (characterChangeAnimator)
            characterChangeAnimator.enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (enabled & isActiveAndEnabled)
            ManageCharacterChange();
    }

    public void FindElements()
    {
        for (int i = 0; i < 2; i++)
        {
            fullObjectsAnimators.Add(GameObject.Find(fullObjectName + i).GetComponent<Animator>());
            illustrationsUIObjects.Add(GameObject.Find(illustrationName + i).GetComponent<Image>());
            UICharacternames.Add(GameObject.Find(nameName + i).GetComponent<TextMeshProUGUI>());
        }
    }


    void ManageCharacterChange()
    {
        // IF CORRECT INPUT DETECTED, CHANGE CHARACTER
        if (canChange && (Mathf.Abs(InputManager.Instance.playerInputs[playerScript.playerNum].horizontal) > 0.5f))
        {
            StartCoroutine(ApplyCharacterChange());
            canChange = false;
        }
        else if (!canChange && Mathf.Abs(InputManager.Instance.playerInputs[playerScript.playerNum].horizontal) < 0.5f)
                canChange = true;
    }

    public IEnumerator ApplyCharacterChange()
    {
        // AUDIO
        if (characterChangeWooshAudioSource)
            characterChangeWooshAudioSource.Play();
        else
            Debug.Log("Warning, character change woosh audio source not found, can't play the sound, continuing anyway");

        
        if (InputManager.Instance.playerInputs[playerScript.playerNum].horizontal > 0.5f)
        {
            currentCharacter++;


            if (currentCharacter >= charactersDatabase.charactersList.Count)
                currentCharacter = 0;


            // ANIMATION
            if (fullObjectsAnimators.Count > 0)
                fullObjectsAnimators[playerScript.playerNum].SetTrigger("Right");
            if (characterChangeAnimator != null)
                characterChangeAnimator.SetTrigger("Right");
        }
        else if (InputManager.Instance.playerInputs[playerScript.playerNum].horizontal < -0.5f)
        {
            currentCharacter--;


            if (currentCharacter < 0)
                currentCharacter = charactersDatabase.charactersList.Count - 1;


            // ANIMATION
            if (fullObjectsAnimators.Count > 0)
                fullObjectsAnimators[playerScript.playerNum].SetTrigger("Left");
            if (characterChangeAnimator != null)
                characterChangeAnimator.SetTrigger("Left");
        }

        lastChosenCharacterIndex = currentCharacter;


        // WAIT FOR CHANGE ANIM
        yield return new WaitForSeconds(changeDelay);
;       

        // PLAYER ANIMATORS
        if (playerAnimator != null)
            playerAnimator.runtimeAnimatorController = charactersDatabase.charactersList[currentCharacter].animator;
        if (legsAnimator != null)
            legsAnimator.runtimeAnimatorController = charactersDatabase.charactersList[currentCharacter].legsAnimator;


        // PLAYER SPRITES
        if (playerScript.gameManager.mapLoader.halloween) // Halloween mask
            mask.sprite = masksDatabase.masksList[6].sprite;
        else
            mask.sprite = masksDatabase.masksList[charactersDatabase.charactersList[currentCharacter].defaultMask].sprite;


        // UI DISPLAY
        if (illustrationsUIObjects.Count > 0)
            illustrationsUIObjects[playerScript.playerNum].sprite = charactersDatabase.charactersList[currentCharacter].illustration;
        if (UICharacternames.Count > 0)
            UICharacternames[playerScript.playerNum].text = charactersDatabase.charactersList[currentCharacter].name;

        
        // SCRIPT VALUES
        playerScript.characterNameDisplay.text = charactersDatabase.charactersList[currentCharacter].name;
        playerScript.characterIndex = currentCharacter;
        playerScript.gameManager.scoresNames[playerScript.playerNum].text = charactersDatabase.charactersList[currentCharacter].name;
    }
}
