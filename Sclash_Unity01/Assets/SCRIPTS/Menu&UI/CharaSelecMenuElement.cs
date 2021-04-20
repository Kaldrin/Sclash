using UnityEngine;
using TMPro;
using UnityEngine.UI;


// For Sclas

// REQUIREMENTS
// TextMeshPro package
// TextApparition script
// CharacterChanger script

/// <summary>
/// This script is for the character selection, it's put on every element of the menu and facilitate the modification of the display
/// </summary>

// UNITY 2019.4
public class CharaSelecMenuElement : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI text1 = null;
    [SerializeField] public Image image1 = null;
    [SerializeField] public Image image2 = null;
    [SerializeField] public TextApparition textApparitionComponent = null;
    [HideInInspector] public CharacterChanger characterChanger = null;
    [HideInInspector] public int index = 0;



    public void SelecSelf()
    {
        if (characterChanger != null)
        {
            characterChanger.verticalIndex = index;
            characterChanger.VerticalSelectionChange();
        }
        else
            Debug.Log("Can't find character changer, ignoring");
    }


    public void HorizontalChange(int direction)
    {
        SelecSelf();
        characterChanger.HorizontalSwitch(direction);
    }











    // EDITOR ONLY
    // Dummy function, useless, but don't remove
    void RemoveFuckingWarning()
    {
        text1 = new TextMeshProUGUI();
        image1 = null;
        image2 = null;
    }
}
