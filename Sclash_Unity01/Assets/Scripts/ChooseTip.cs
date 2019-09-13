using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChooseTip : MonoBehaviour
{
    [SerializeField] TipsDatabase tips = null;
    [SerializeField] TextMeshProUGUI tipsTextBox = null;
    [SerializeField] float tipUpdateDuration = 20;


    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChangeTip());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator ChangeTip()
    {
        while (true)
        {
            Debug.Log("Change tip");
            int randomIndex = Random.Range(0, tips.tipsList.Count);


            tipsTextBox.text = tips.tipsList[randomIndex];


            yield return new WaitForSecondsRealtime(tipUpdateDuration);
        }
    }
}
