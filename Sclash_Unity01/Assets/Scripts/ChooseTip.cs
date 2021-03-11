using UnityEngine;
using TMPro;


// HEADER
// For Sclash

// REQUIREMENTS
// Requires the TextMeshPro package to work
// Requires the TextApparitionComponent to make it work with a localization system with the LanguageManager

/// <summary>
/// This script randomly chooses a tip to display in the main menu when enabled
/// </summary>
public class ChooseTip : MonoBehaviour
{
    [Header("COMPONENTS")]
    [Tooltip("Reference to the text box object where to display the tips")]
    [SerializeField] TextMeshProUGUI tipsTextBox = null;
    [SerializeField] TextApparition textApparitionComponent = null;


    [Header("DATA")]
    [Tooltip("Reference to the scriptable object containing the list of the tips to display")]
    [SerializeField] TipsDatabase tipsData = null;
    [SerializeField] TipsDatabase tipsKeysData = null;


    [Header("SETTINGS")]
    [Tooltip("Instead of displaying the text in the TMPUGUI component, will set the key corresponding to the tip to display in the TextApparition component so it displays the tip in the right language")]
    [SerializeField] bool keysMode = true;
    








    void OnEnable()                                                                         // ON ENABLE
    {
        Invoke("ChangeTip", 0.1f);
    }



    // TIP CHANGE
    // Changes the displayed tip immediatly
    void ChangeTip()                                                                        // CHANGE TIP
    {
        GetReferences();

        if (!keysMode && tipsData != null)
        {
            int randomIndex = Random.Range(0, tipsData.tipsList.Count);

            if (tipsTextBox != null)
                tipsTextBox.text = tipsData.tipsList[randomIndex];
        }
        else if (keysMode && tipsKeysData != null && textApparitionComponent != null)
        {
            string randomTipKey = tipsKeysData.tipsList[Random.Range(0, tipsKeysData.tipsList.Count - 1)];

            textApparitionComponent.textKey = randomTipKey;
            textApparitionComponent.TransfersTrad();
        }
    }






    // Check if it's possible to get the missing component references
    void GetReferences()
    {
        if (tipsTextBox == null)
            if (GetComponent<TextMeshProUGUI>())
                tipsTextBox = GetComponent<TextMeshProUGUI>();

        if (textApparitionComponent == null)
            if (GetComponent<TextApparition>())
                textApparitionComponent = GetComponent<TextApparition>();
    }


    // EDITOR
    // Automatically get the references before the player has to drag'n drop them in the Serialized Fields
    private void OnDrawGizmosSelected()
    {
        GetReferences();
    }
}
