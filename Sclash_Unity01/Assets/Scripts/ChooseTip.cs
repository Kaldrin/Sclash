using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChooseTip : MonoBehaviour
{
    // TIPS
    [Header("TIPS")]
    [Tooltip("Reference to the scriptable object containing the list of the tips to display")]
    [SerializeField] TipsDatabase tipsData = null;
    [Tooltip("Reference to the text box object where to display the tips")]
    [SerializeField] TextMeshProUGUI tipsTextBox = null;





    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // OnEnable is called when the object is enabled
    void OnEnable()
    {
        ChangeTip();
    }

    // Update is called once per graphic frame
    void Update()
    {
    }





    // TIP CHANGE
    // Changes the displayed tip immediatly
    void ChangeTip()
    {
        int randomIndex = Random.Range(0, tipsData.tipsList.Count);

        tipsTextBox.text = tipsData.tipsList[randomIndex];
    }
}
