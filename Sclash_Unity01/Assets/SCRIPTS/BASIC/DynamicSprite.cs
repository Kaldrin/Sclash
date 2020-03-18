using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSpriteMask : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRendererToCopy = null;
    [SerializeField] SpriteMask spriteMaskToModify = null;
    Sprite oldSprite = null;


    // Update is called once per frame
    void Update()
    {
        if (oldSprite != spriteRendererToCopy.sprite)
        {
            oldSprite = spriteRendererToCopy.sprite;
            spriteMaskToModify.sprite = oldSprite;
        }
    }
}
