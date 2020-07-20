using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterChanger : MonoBehaviour
{
    [Header("PLAYER COMPONENTS")]
    [SerializeField] Player playerScript = null;
    [SerializeField] Animator playerAnimator = null;
    [SerializeField] Animator legsAnimator = null;
    [SerializeField] Animator characterChangeAnimator = null;
    [SerializeField] SpriteRenderer mask = null;


    [Header("DATA")]
    [SerializeField] CharactersDatabase charactersDatabase = null;
    [SerializeField] MasksDatabase masksDatabase = null;


    [Header("CHARACTER CHANGE VISUAL OBJECTS NAMES")]
    [SerializeField] string fullObjectName = "CharacterChange";
    [SerializeField] string illustrationName = "CharacterChangeIllustration";
    [SerializeField] string nameName = "CharacterChangeName";


    [Header("OTHER")]
    [SerializeField] int defaultCharacterIndex = 1;
    [SerializeField] float changeDelay = 0.1f;



    [HideInInspector] public int currentCharacter = 0;
    bool canChange = true;



    public List<Animator> fullObjectsAnimators = new List<Animator>();
    public List<Image> illustrations = new List<Image>();
    public List<TextMeshProUGUI> names = new List<TextMeshProUGUI>();






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




        /* Destroy(GetComponent<IAScript>());
         gameObject.AddComponent<IAScript>();
 */
        currentCharacter = defaultCharacterIndex;
        characterChangeAnimator.enabled = true;


        for (int i = 0; i < 2; i++)
        {
            fullObjectsAnimators.Add(GameObject.Find(fullObjectName + i).GetComponent<Animator>());
            illustrations.Add(GameObject.Find(illustrationName + i).GetComponent<Image>());
            names.Add(GameObject.Find(nameName + i).GetComponent<TextMeshProUGUI>());
        }

        fullObjectsAnimators[playerScript.playerNum].SetBool("On", true);


        ApplyCharacterChange();
    }


    private void OnDisable()
    {
        fullObjectsAnimators[playerScript.playerNum].SetBool("On", false);
        characterChangeAnimator.enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (enabled)
            ManageCharacterChange();
    }


    void ManageCharacterChange()
    {
        if (canChange && (Mathf.Abs(InputManager.Instance.playerInputs[playerScript.playerNum].horizontal) > 0.5f))
        {
            ApplyCharacterChange();
            canChange = false;
        }
        else
        {
            if (Mathf.Abs(InputManager.Instance.playerInputs[playerScript.playerNum].horizontal) < 0.5f)
                canChange = true;
        }
    }

    public void ApplyCharacterChange()
    {
        if (InputManager.Instance.playerInputs[playerScript.playerNum].horizontal > 0.5f)
        {
            currentCharacter++;


            if (currentCharacter >= charactersDatabase.charactersList.Count)
                currentCharacter = 0;


            fullObjectsAnimators[playerScript.playerNum].SetTrigger("Right");
            characterChangeAnimator.SetTrigger("Right");
        }
        else if (InputManager.Instance.playerInputs[playerScript.playerNum].horizontal < 0.5f)
        {
            currentCharacter--;


            if (currentCharacter < 0)
                currentCharacter = charactersDatabase.charactersList.Count - 1;


            fullObjectsAnimators[playerScript.playerNum].SetTrigger("Left");
            characterChangeAnimator.SetTrigger("Left");
        }

        Invoke("ApplyCharacterChange_N", changeDelay);
    }

    void ApplyCharacterChange_N()
    {
        if (!enabled)
            return;

        playerAnimator.runtimeAnimatorController = charactersDatabase.charactersList[currentCharacter].animator;
        legsAnimator.runtimeAnimatorController = charactersDatabase.charactersList[currentCharacter].legsAnimator;
        mask.sprite = masksDatabase.masksList[charactersDatabase.charactersList[currentCharacter].defaultMask].sprite;
        illustrations[playerScript.playerNum].sprite = charactersDatabase.charactersList[currentCharacter].illustration;
        names[playerScript.playerNum].text = charactersDatabase.charactersList[currentCharacter].name;
    }
}
