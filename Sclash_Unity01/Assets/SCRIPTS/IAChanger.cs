using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAChanger : MonoBehaviour
{
    [SerializeField] IAScript iaScript = null;
    
    Animator IAindicatorAnimator = null;

    bool
        iaOn = false,
        canSwitch = false,
        canChangeDifficulty = false;
    int currentDifficulty = 0; 
    [SerializeField] float iaChangeDeadZone = 0.4f;



    [Header("DISPLAY OBJECTS")]
    [SerializeField] string IAIndicatorName = "IAIndicator";
    [SerializeField] string
        easyDifficultyTextName = "easyDifficultyText",
        mediumDifficultyTextName = "mediumDifficultyText",
        hardDifficultyTextName = "hardDifficultyText";
    List<GameObject> diggicultyTextObjects = new List<GameObject>();










    void OnEnable()
    {
        IAindicatorAnimator = GameObject.Find(IAIndicatorName).GetComponent<Animator>();

        diggicultyTextObjects.Add(GameObject.Find(easyDifficultyTextName));
        diggicultyTextObjects.Add(GameObject.Find(mediumDifficultyTextName));
        diggicultyTextObjects.Add(GameObject.Find(hardDifficultyTextName));
    }

    void Update()
    {
        if (InputManager.Instance.playerInputs[1].reallyanykey && iaOn)
            SwitchIAMode(false);


        // Change IA / Player
        if (canSwitch && InputManager.Instance.playerInputs[0].vertical >= iaChangeDeadZone)
        {
            canSwitch = false;
            SwitchIAMode(!iaOn);
        }
        if (!canSwitch && InputManager.Instance.playerInputs[0].vertical < InputManager.Instance.axisDeadZone)
            canSwitch = true;


        // Change difficulty
        if (iaOn && canChangeDifficulty && InputManager.Instance.playerInputs[0].vertical <= -iaChangeDeadZone)
        {
            canChangeDifficulty = false;
            ChangeIADifficulty(1);
        }
        if (!canChangeDifficulty && InputManager.Instance.playerInputs[0].vertical > -InputManager.Instance.axisDeadZone)
            canChangeDifficulty = true;
    }






    void ChangeIADifficulty(int incrementation)
    {
        currentDifficulty += incrementation;


        if (currentDifficulty > 2)
            currentDifficulty = 0;
        else if (currentDifficulty < 0)
            currentDifficulty = 2;


        switch(currentDifficulty)
        {
            case 0:
                iaScript.SetDifficulty(IAScript.Difficulty.Easy);
                break;

            case 1:
                iaScript.SetDifficulty(IAScript.Difficulty.Medium);
                break;

            case 2:
                iaScript.SetDifficulty(IAScript.Difficulty.Hard);
                break;
        }


        for (int i = 0; i < diggicultyTextObjects.Count; i++)
        {
            if (i == currentDifficulty)
                diggicultyTextObjects[i].SetActive(true);
            else
                diggicultyTextObjects[i].SetActive(false);
        }


        Debug.Log("IA is now " + currentDifficulty);
    }

    void SwitchIAMode(bool state)
    {
        iaOn = state;
        iaScript.enabled = state;
        IAindicatorAnimator.SetBool("AI", state);
        ChangeIADifficulty(0);
    }
}
