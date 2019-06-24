using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Created for Unity 2019.1.1f1
public class TriggerAnimatorAfterDuration : MonoBehaviour
{
    [SerializeField] Animator animatorToTrigger = null;
    [SerializeField] Vector2 randomDelayBetween = new Vector2(0.1f, 1f);











    // BASE FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        animatorToTrigger.enabled = false;
        

        StartCoroutine(TriggerAnimatorAfter(Random.Range(randomDelayBetween.x, randomDelayBetween.y)));
    }

    // Update is called once per graphic frame
    void Update()
    {
        
    }






    // TRIGGER
    IEnumerator TriggerAnimatorAfter(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        animatorToTrigger.enabled = true;
    }
}
