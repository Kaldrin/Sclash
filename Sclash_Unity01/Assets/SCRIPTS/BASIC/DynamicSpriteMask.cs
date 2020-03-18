using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSprite : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRendererToCopy = null;
    [SerializeField] SpriteRenderer spriteRendererToModify = null;
    Sprite oldSprite = null;


    // Update is called once per frame
    void Update()
    {
        if (oldSprite != spriteRendererToCopy.sprite)
        {
            oldSprite = spriteRendererToCopy.sprite;
            spriteRendererToModify.sprite = oldSprite;
        }
    }
}
