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
        if (colorIndex <= colorsToPick.Count - 1)
            ImageToModify.color = colorsToPick[colorIndex];
    }
}
