using UnityEngine;
using TMPro;
using UnityEngine.UI;


// This script is for the character selection, it's put on every element of the menu
public class CharaSelecMenuElement : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI text1 = null;
    [SerializeField] public Image image1 = null;
    [SerializeField] public Image image2 = null;


    // Dummy function, useless, but don't remove
    void RemoveFuckingWarning()
    {
        text1 = new TextMeshProUGUI();
        image1 = null;
        image2 = null;
    }
}
