using UnityEngine;
using TMPro;

// This script randomly chooses a tip to display in the main menu when enabled
public class ChooseTip : MonoBehaviour
{
    [Header("TIPS")]
    [Tooltip("Reference to the scriptable object containing the list of the tips to display")]
    [SerializeField] TipsDatabase tipsData = null;
    [Tooltip("Reference to the text box object where to display the tips")]
    [SerializeField] TextMeshProUGUI tipsTextBox = null;







    // OnEnable is called when the object is enabled
    void OnEnable()
    {
        ChangeTip();
    }



    // TIP CHANGE
    // Changes the displayed tip immediatly
    void ChangeTip()
    {
        int randomIndex = Random.Range(0, tipsData.tipsList.Count);

        tipsTextBox.text = tipsData.tipsList[randomIndex];
    }
}
