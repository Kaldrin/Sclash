using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class AsynTest : MonoBehaviour
{
    void Update()
    {
        // Press the space key to start coroutine
        if (Keyboard.current.spaceKey.isPressed)
        {
            // Use a coroutine to load the Scene in the background
            Debug.Log("Start async load");
            StartCoroutine(LoadYourAsyncScene());

        }
    }

    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SclashMainScene01");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("Async load finished");
    }
}
