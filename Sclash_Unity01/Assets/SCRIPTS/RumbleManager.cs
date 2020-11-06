using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure; // Required in C#

// Manages all rumbles of the game, but might be changed, it's not great this way
public class RumbleManager : MonoBehaviour
{
    #region Singleton
    public static RumbleManager Instance;
    #endregion


    //bool vibrating = false;
    [SerializeField] public float menuBrowseVibrationIntensity = 0.08f;
    [SerializeField] public float menuBrowseVibrationDuration = 0.06f;
    Coroutine[] vibrationCoroutines = new Coroutine[4];


    [SerializeField] bool rumbleOn = true;





    private void Awake(){
        Instance = this;
    }

    private void Update()
    {
        if (rumbleOn && enabled && isActiveAndEnabled)
        {
            /*
            if (!vibrating)
                for (int i = 0; i < 4; i++)
                {
                    PlayerIndex index = new PlayerIndex();
                    switch (i)
                    {
                        case 0:
                            index = PlayerIndex.One;
                            break;

                        case 1:
                            index = PlayerIndex.Two;
                            break;

                        case 2:
                            index = PlayerIndex.Three;
                            break;

                        case 3:
                            index = PlayerIndex.Four;
                            break;
                    }


                    // Starts vibration
                    GamePad.SetVibration(index, 0, 0);
                }
                */
        }
    }


    public void TriggerSimpleControllerVibration(int playerIndex, float leftIntensity, float rightIntensity, float duration)
    {
        if (vibrationCoroutines[playerIndex] != null)
            StopCoroutine(vibrationCoroutines[playerIndex]);


        vibrationCoroutines[playerIndex] = StartCoroutine(SimpleVibrationCoroutine(playerIndex, leftIntensity, rightIntensity, duration));
    }

    public void TriggerSimpleControllerVibrationForEveryone(float leftIntensity, float rightIntensity, float duration)
    {
        //StopCoroutine("SimpleVibrationCoroutine");
        for (int i = 0; i < 4; i++)
        {
            if (vibrationCoroutines[i] != null)
                StopCoroutine(vibrationCoroutines[i]);


            vibrationCoroutines[i] = StartCoroutine(SimpleVibrationCoroutine(i, leftIntensity, rightIntensity, duration));
        }
    }

    public void CancelVibration(int playerIndex)
    {
        PlayerIndex index = new PlayerIndex();


        switch (playerIndex)
        {
            case 0:
                index = PlayerIndex.One;
                break;

            case 1:
                index = PlayerIndex.Two;
                break;

            case 2:
                index = PlayerIndex.Three;
                break;

            case 3:
                index = PlayerIndex.Four;
                break;
        }


        // Starts vibration
        GamePad.SetVibration(index, 0, 0);
    }


    IEnumerator SimpleVibrationCoroutine(int playerIndex, float leftIntensity, float rightIntensity, float duration)
    {
        //StopCoroutine(SimpleVibrationCoroutine(0, 0, 0, 0));
        //vibrating = false;
        //CancelVibration(playerIndex);


        // Set player index
        PlayerIndex index = new PlayerIndex();
        switch (playerIndex)
        {
            case 0:
                index = PlayerIndex.One;
                break;

            case 1:
                index = PlayerIndex.Two;
                break;

            case 2:
                index = PlayerIndex.Three;
                break;

            case 3:
                index = PlayerIndex.Four;
                break;
        }

        if (rumbleOn)
        {
            // Resets vibration
            //vibrating = false;
            GamePad.SetVibration(index, 0, 0);
        

            yield return new WaitForSeconds(0.01f);


            // Starts vibration
            //vibrating = true;
            GamePad.SetVibration(index, leftIntensity, rightIntensity);
        

            yield return new WaitForSeconds(duration);


            // Ends vibration
            //vibrating = false;
            Debug.Log("End vibration");
            GamePad.SetVibration(index, 0, 0);
        }
    }
}
