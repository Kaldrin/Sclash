using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

// This is the rumble manager.
// It manages all controller rumbles in the game
// Any rumble that should occur will be called here
public class RumbleManager : MonoBehaviour
{
    [HideInInspector] public static RumbleManager Instance;

    [SerializeField] bool enableControllerRumble = true;
    PlayerIndex playerIndex = PlayerIndex.One;
    GamePadState state = new GamePadState();
    GamePadState prevState = new GamePadState();







    void Start()
    {
        Instance = this;

        // If controller rumbles are enabled, sets the current rumble value to 0 for all controllers
        if (enableControllerRumble)
            GamePad.SetVibration(playerIndex, 0, 0);
    }


    // Function to call by other scripts to make a controller rumble
    public void Rumble(RumbleSettings rumbleSettings)
    {
        // If controller rumbles are enabled, rumble
        if (enableControllerRumble)
            StartCoroutine(RumblePlayer01Coroutine(rumbleSettings.rumbleDuration, rumbleSettings.rumbleStrengthLeft, rumbleSettings.rumbleStrengthRight, rumbleSettings.rumbleNumber, rumbleSettings.betweenRumblesDuration));
    }


    IEnumerator RumblePlayer01Coroutine(float duration, float leftStrength, float rightStrength, int numberOfTimes, float betweenRumblesDuration)
    {
        //GamePad.SetVibration(playerIndex, 0, 0);


        for (int i = 0; i < numberOfTimes; i++)
        {
            // Starts the rumble
            GamePad.SetVibration(playerIndex, leftStrength, rightStrength);


            yield return new WaitForSecondsRealtime(duration);


            // Ends the rumble
            GamePad.SetVibration(playerIndex, 0, 0);


            yield return new WaitForSecondsRealtime(betweenRumblesDuration);
        }
    }



    #region EDITOR ONLY
    void RemoveFuckingWarnings()
    {
        state = prevState;
        prevState = state;
    }
    #endregion
}
