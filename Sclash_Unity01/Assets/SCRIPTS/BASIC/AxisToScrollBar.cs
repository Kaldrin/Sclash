using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AxisToScrollBar : MonoBehaviour
{
    [SerializeField] string axisToUse = "Scrollwheel";
    [SerializeField] Scrollbar scrollbar = null;
    [SerializeField] float multiplier = 0.2f;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scrollbar.value += Input.GetAxis(axisToUse) * multiplier;
    }
}
