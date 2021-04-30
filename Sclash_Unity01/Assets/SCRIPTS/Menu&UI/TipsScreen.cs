using System.Collections;
using System.Collections.Generic;
using UnityEngine;






// For Sclash
// Bastien BERNAND

// REQUIREMENTS
// TipsDatabse scriptable object
// TextApparition script

/// <summary>
/// Script to set up the tips list menu
/// </summary>

// UNITY 2020.3
public class TipsScreen : MonoBehaviour
{
    [SerializeField] Transform tipsListParent = null;
    [SerializeField] GameObject tipObject = null;
    [SerializeField] TipsDatabase tipsKeysDatabse = null;







    private void OnEnable()                                                                                                     // ON ENABLE
    {
        GetMissingComponents();
        /*
        for (int i = 0; i < tipsListParent.transform.childCount; i++)
            if (tipsListParent.transform.GetChild(i) != tipObject)
                Destroy(tipsListParent.transform.GetChild(i));
                */

        for (int i = 0; i < tipsKeysDatabse.tipsList.Count; i++)
        {
            GameObject newTip = Instantiate(tipObject, tipsListParent);
            newTip.GetComponentInChildren<TextApparition>().textKey = tipsKeysDatabse.tipsList[i];
            newTip.GetComponentInChildren<TextApparition>().TransfersTrad();
        }

        tipObject.SetActive(false);
    }




    void GetMissingComponents()
    {
        if (tipsListParent == null)
            tipsListParent = transform;
        if (tipObject == null && transform.childCount > 0)
            tipObject = transform.GetChild(0).gameObject;
    }




    // EDITOR
    private void OnDrawGizmosSelected()
    {
        GetMissingComponents();
    }
}
