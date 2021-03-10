using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


// Script that enables more function to change UI elements properties from inspector events like buttons
// Originally made for Unity 2019.1.1f1
public class ChangeUIImageProperties : MonoBehaviour
{
    [SerializeField] Image ImageToModify = null;
    [SerializeField] TextMeshProUGUI textToModify = null;
    [SerializeField] List<Color> colorsToPick = new List<Color>();




    public void ChangeImageColor(int colorIndex)
    {
        GetComponents();


        if (colorIndex <= colorsToPick.Count - 1)
        {
            if (ImageToModify != null)
                ImageToModify.color = colorsToPick[colorIndex];
            if (textToModify != null)
                textToModify.color = colorsToPick[colorIndex];
        }
    }









    // If components not here, get them
    void GetComponents()
    {
        if (ImageToModify == null)
            if (GetComponent<Image>())
                ImageToModify = GetComponent<Image>();

        if (textToModify == null)
            if (GetComponent<TextMeshProUGUI>())
                textToModify = GetComponent<TextMeshProUGUI>();
    }


    // EDITOR
    private void OnDrawGizmosSelected()
    {
        GetComponents();
    }
}
