using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script allows to trigger functions to force layout rebuild in a canvas starting from a referenced rect transform
// OPTIMIZED ?
public class ForceLayoutRebuild : MonoBehaviour
{
    [Tooltip("Force constant layout rebuilt, extremely heavy in performance, do not do that")]
    [SerializeField] bool inUpdate = false;
    [SerializeField] RectTransform rectTransformParentForRebuilding = null;





    void Update()
    {
        if (enabled && isActiveAndEnabled && inUpdate)
            ForceImmediateLayoutRebuildOnce();
    }

    public void ForceImmediateLayoutRebuildOnce()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransformParentForRebuilding);
    }

    public void ForceImmediateLayoutRebuildMultipleTimes(int iterations)
    {
        for (int i = 0; i < iterations; i++)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransformParentForRebuilding);
    }
}
