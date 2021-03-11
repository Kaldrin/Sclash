using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;




// HEADER
// Reusable script
// For Sclash

// REQUIREMENTS
// Requires the XInputDotNet package to work
// Requires the RumbleSettings scriptable object to work

/// <summary>
/// Manages all controller rumbles in the game. Any rumble that should occur will be called here
/// </summary>

// VERSION
// Originally created for Unity 2019.1.14
public class RumbleManager : MonoBehaviour
{
    [HideInInspector] public static RumbleManager Instance;

    [SerializeField] bool enableControllerRumble = true;
    PlayerIndex playerIndex = PlayerIndex.One;
    GamePadState state = new GamePadState();
    GamePadState prevState = new GamePadState();







    void Start()                                                                                                // START
    {
        Instance = this;

        // If controller rumbles are enabled, sets the current rumble value to 0 for all controllers
        if (enableControllerRumble)
            GamePad.SetVibration(playerIndex, 0, 0);
    }


    // Function to call by other scripts to make a controller rumble
    public void Rumble(RumbleSettings rumbleSettings)                                                                               // RUMBLE
    {
        // If controller rumbles are enabled, rumble
        if (enableControllerRumble && MenuManager.Instance.menuParametersSaveScriptableObject.enableRumbles)
        {
            if (rumbleSettings != null)
            {
                if (!rumbleSettings.muteRumble)
                    StartCoroutine(RumblePlayer01Coroutine(rumbleSettings.rumbleDuration, rumbleSettings.rumbleStrengthLeft, rumbleSettings.rumbleStrengthRight, rumbleSettings.rumbleNumber, rumbleSettings.betweenRumblesDuration));
            }
            else
                Debug.Log("Rumble settings not found, ignoring");
        }
    }


    IEnumerator RumblePlayer01Coroutine(float duration, float leftStrength, float rightStrength, int numberOfTimes, float betweenRumblesDuration)                               // RUMBLE PLAYER 01 COROUTINE
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
    // Useless but don't remove it
    void RemoveFuckingWarnings()
    {
        state = prevState;
        prevState = state;
    }
    #endregion
}
