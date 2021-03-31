﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// HEADER
// For Sclash
// Reusable script

// REQUIREMENTS
// Requires the LanguageManager script to work as a language selector but you can remove this part it will still work as a selector
// Requires the TextMeshPro package

/// <summary>
/// Script that manages a left / right UI selection, like for a language
/// </summary>

// VERSION
// Originally made for Unity 2019.14
public class Selector : MonoBehaviour
{
    [Header("COMPONENTS")]
    [SerializeField] TextMeshProUGUI tmpTextRef = null;


    [Header("ANIMATION")]
    [SerializeField] Animation leftArrow = null;
    [SerializeField] Animation rightArrow = null;
    [SerializeField] Animation main = null;
    [SerializeField] string arrowAnimName = "SelectorArrow01";
    [SerializeField] string rightAnimName = "SelectortextSwipeRight01";
    [SerializeField] string leftAnimName = "SelectortextSwipeLeft01";


    [Header("SETTINGS")]
    [SerializeField] bool isLanguageSelector = false;
    [SerializeField] bool isVoicesLanguageSelector = false;

    [SerializeField] List<string> elements = new List<string>();
    int currentIndex = 0;



    [Header("AUDIO")]
    [SerializeField] AudioSource wooshSFX = null;








    private void OnEnable()                                                                                         // ON ENABLE
    {
        SetUpElements();
    }





    void SetUpElements()                                                                                        // SET UP ELEMENTS
    {
        if (isLanguageSelector && LanguageManager.Instance != null)
            elements = LanguageManager.Instance.availableLanguages;
        if (isVoicesLanguageSelector && LanguageManager.Instance != null)
            elements = LanguageManager.Instance.availableVoicesLanguages;

        Invoke("SetUpIndex", 0.2f);
    }

    void SetUpIndex()                                                                                           // SET UP INDEX
    {
        if (elements != null && elements.Count > 0 && tmpTextRef != null)
            for (int i = 0; i < elements.Count; i++)
                if (tmpTextRef.text == elements[i])
                    currentIndex = i;
    }






    // SWITCH
    public void Switch(bool side = true)                                                                    // SWITCH
    {
        if (!side)                      //--
        {
            // ANIMATION
            if (leftArrow != null)
                leftArrow.Play(arrowAnimName);
            if (main != null)
            {
                if (main.isPlaying)
                    main.Stop();
                main.Play(leftAnimName);
            }

            // INDEX
            currentIndex--;
        }
        else                                 //++
        {
            // ANIMATION
            if (rightArrow != null)
                rightArrow.Play(arrowAnimName);
            if (main != null)
            {
                if (main.isPlaying)
                    main.Stop();
                main.Play(rightAnimName);
            }

            // INDEX
            currentIndex++;
        }
        

        // Index looping
        if (currentIndex >= elements.Count)
            currentIndex = 0;
        if (currentIndex < 0)
            currentIndex = elements.Count - 1;



        // AUDIO
        if (wooshSFX != null)
            wooshSFX.Play();


        // Wait for the animation to play and then change the text
        Invoke("ChangeText", 0.09f);
    }


    void ChangeText()                                                                                       // CHANGE TEXT
    {
        if (elements != null && elements.Count > currentIndex)
            tmpTextRef.text = elements[currentIndex];

        if (isLanguageSelector && LanguageManager.Instance != null)
        {
            LanguageManager.Instance.language = elements[currentIndex];
            MenuManager.Instance.SaveErgonomySettingsInScriptableObject();
            LanguageManager.Instance.RefreshTexts();
        }
        if (isVoicesLanguageSelector && LanguageManager.Instance)
        {
            LanguageManager.Instance.voicesLanguage = elements[currentIndex];
            MenuManager.Instance.SaveErgonomySettingsInScriptableObject();
        }
    }
}
