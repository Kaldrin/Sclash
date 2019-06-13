using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuElement : MonoBehaviour
{
    [SerializeField] SpriteRenderer selectedSprite = null;

    [SerializeField] bool button = false;
    [SerializeField] bool slider = false;
    [SerializeField] Button buttonScript = null;
    [SerializeField] Slider sliderScript = null;
    bool selected = false;

    // Start is called before the first frame update
    void Start()
    {
        Select(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select(bool state)
    {
        selected = state;
        selectedSprite.enabled = state;
    }
}
