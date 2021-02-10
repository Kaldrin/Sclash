using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class ChangeUIImageProperties : MonoBehaviour
{
    [SerializeField] Image ImageToModify = null;
    [SerializeField] List<Color> colorsToPick = new List<Color>();


    public void ChangeImageColor(int colorIndex)
    {
        if (ImageToModify == null)
            if (GetComponent<Image>())
                ImageToModify = GetComponent<Image>();


        if (colorIndex <= colorsToPick.Count - 1)
            ImageToModify.color = colorsToPick[colorIndex];
    }











    // EDITOR
    private void OnDrawGizmosSelected()
    {
        if (ImageToModify == null)
            if (GetComponent<Image>())
                ImageToModify = GetComponent<Image>();
    }
}
