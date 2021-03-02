using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Allow to give  gameobject a max life duration
// OPTIMIZED
public class LastAndDestroy : MonoBehaviour
{
    [Header("ANIMATION")]
    [SerializeField] Animation animationComponent = null;
    [SerializeField] string animationName = "animationName";
    [SerializeField] float animationStartTime = 2f;
    bool animationTriggered = false;


    [Header("DESTROY / DEACTIVATE")]
    [SerializeField] bool deactivateInsteadOfDestroy = false;
    [SerializeField] public float lastDuration;
    [SerializeField] bool activate = true;
    float spawnTimecode;

    //Game object to spawn
    [SerializeField] public List<GameObject> gameObjectsToSpawnOnDestroy = new List<GameObject>();










    #region FUNCTIONS
    void Start()                                               // START
    {
        spawnTimecode = Time.time;
        GetComponents();
    }

    void Update()                                                      // UPDATE
    {
        if (enabled && isActiveAndEnabled)
        {
            if (activate && Time.time >= spawnTimecode + lastDuration)
                End();

            if (activate && !animationTriggered && Time.time >= spawnTimecode + animationStartTime)
                TriggerAnimation();
        }
    }

    private void OnEnable()
    {
        spawnTimecode = Time.time;
        animationTriggered = false;
    }




    // ANIMATION
    void TriggerAnimation()
    {
        animationTriggered = true;

        if (animationComponent != null && animationName != null && animationName != "")
            animationComponent.Play(animationName, PlayMode.StopAll);
    }


    // END
    void End()
    {
        // SPAWN
        if (gameObjectsToSpawnOnDestroy != null && gameObjectsToSpawnOnDestroy.Count > 0)
            for (int i = 0; i < gameObjectsToSpawnOnDestroy.Count; i++)
                if (gameObjectsToSpawnOnDestroy[i] != null)
                    Instantiate(gameObjectsToSpawnOnDestroy[i], gameObject.transform.position, gameObject.transform.rotation);


        // END
        if (deactivateInsteadOfDestroy)
            gameObject.SetActive(false);
        else
            Destroy(gameObject);  
    }








    // GET COMPONENTS
    void GetComponents()
    {
        if (animationComponent == null)
            if (GetComponent<Animation>())
                animationComponent = GetComponent<Animation>();
    }






    // EDITOR ONLY
    private void OnDrawGizmosSelected()
    {
        GetComponents();
    }
    #endregion
}
