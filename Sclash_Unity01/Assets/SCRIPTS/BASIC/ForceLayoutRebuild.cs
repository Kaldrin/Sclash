using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForceLayoutRebuild : MonoBehaviour
{
    [SerializeField] bool inUpdate = false;
    [SerializeField] RectTransform rectTransformParentForRebuilding;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inUpdate)
            ForceImmediateLayoutRebuildOnce();
    }

    public void ForceImmediateLayoutRebuildOnce()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransformParentForRebuilding);
    }

    public void ForceImmediateLayoutRebuildMultipleTimes(int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransformParentForRebuilding);
        }
    }
}
