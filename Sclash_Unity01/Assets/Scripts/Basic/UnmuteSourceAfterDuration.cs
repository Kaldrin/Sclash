using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnmuteSourceAfterDuration : MonoBehaviour
{
    [SerializeField] AudioSource source = null;
    [SerializeField] float duration = 1;
    float baseVolume = 0;



    // BASIC FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        baseVolume = source.volume;
        source.volume = 0;
        StartCoroutine(WaitToUnmute());
    }

    // Update is called once per graphic frame
    void Update()
    {
        
    }




    IEnumerator WaitToUnmute()
    {
        yield return new WaitForSeconds(duration);

        source.volume = baseVolume;
    }
}
