using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DynamicValueTMP : MonoBehaviour
{
    
    [HideInInspector] public int value = 10;
    [SerializeField] GameObject textObject = null;
    [SerializeField] int
        defaultValue = 0,
        incrementation = 1,
        minimum = 0,
        maximum = 5;


    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        //value = defaultValue;
        UpdateValueVisual();
    }

    // Update is called once per frame
    void Update()
    {
        
    }






    // VALUE
    public void Increment()
    {
        value += incrementation;

        if (value >= maximum)
            value = maximum;


        UpdateValueVisual();
    }

    public void Decrement()
    {
        value -= incrementation;

        if (value <= minimum)
            value = minimum;


        UpdateValueVisual();
    }

    public void Reset()
    {
        value = defaultValue;
        UpdateValueVisual();
    }





    // DISPLAY
    void UpdateValueVisual()
    {
        if (textObject.GetComponent<TextMeshPro>())
        {
            TextMeshPro text = textObject.GetComponent<TextMeshPro>();
            text.text = value.ToString();
        }
        if (textObject.GetComponent<TextMeshProUGUI>())
        {
            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            text.text = value.ToString();
        }
        if (textObject.GetComponent<TextMesh>())
        {
            TextMesh text = textObject.GetComponent<TextMesh>();
            text.text = value.ToString();
        }
    }
}
