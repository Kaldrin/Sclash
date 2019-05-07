using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(ParticleSystem))]
public class ParticlesSortingLayer : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerID = spriteRenderer.sortingLayerID;
        GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = spriteRenderer.sortingOrder;
    }

}