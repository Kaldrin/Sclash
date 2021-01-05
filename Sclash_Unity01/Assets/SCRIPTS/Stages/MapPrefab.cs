using UnityEngine;

// References classes : Scarf02



// This script is contained at the root of a stage prefab. It allows to reference which objects should do what in the stages
// OPTIMIZED
public class MapPrefab : MonoBehaviour
{
    [SerializeField] bool previewDramaticScreenInEditor = true;
    [Tooltip("The reference to the objects of the stage prefab that should be deactivated during the dramatic screen")]
    [SerializeField] public GameObject[] backgroundElements = null;
    [SerializeField] public GameObject[] objectsToActivateOnDramaticScreen = null;
    [SerializeField] Animator animatorToEnableOnStart = null;
    [SerializeField] bool disableAnimatorAfterDuration = true;
    [SerializeField] float durationBeforeDisablingAnimator = 5f;









    #region FUNCTIONS
    // BASIC FUNCTIONS
    private void Start()
    {
        EnableClothCollision();
    }






    // DRAMATIC SCREEN
    public void TriggerDramaticScreen()
    {
        // Disable elements
        if (backgroundElements.Length > 0)
            for (int i = 0; i < backgroundElements.Length; i++)
                backgroundElements[i].SetActive(false);


        // Enable elements
        if (objectsToActivateOnDramaticScreen.Length > 0)
            for (int i = 0; i < objectsToActivateOnDramaticScreen.Length; i++)
                objectsToActivateOnDramaticScreen[i].SetActive(true);
    }







    // START ANIM
    public void TriggerStartStage()
    {
        if (animatorToEnableOnStart != null)
        {
            animatorToEnableOnStart.enabled = true;


            if (disableAnimatorAfterDuration)
                Invoke("DisableAnimator", durationBeforeDisablingAnimator);
        }
    }


    void DisableAnimator()
    {
        animatorToEnableOnStart.enabled = false;
    }





    // CLOTH
    // Find the cloths elements and tell themm to find the colliders
    void EnableClothCollision()
    {
        // SCARFS
        Scarf02[] scarfs = FindObjectsOfType<Scarf02>();


        if (scarfs != null && scarfs.Length > 0)
            for (int i = 0; i < scarfs.Length; i++)
            {
                if (scarfs[i] != null)
                    scarfs[i].FindColliders();
                else
                    Debug.Log("Couldn't find this scarf component, ignoring");
            }
    }









    // EDITOR ONLY
    private void OnDrawGizmosSelected()
    {
        if (previewDramaticScreenInEditor)
        {
            if (backgroundElements.Length > 0)
                for (int i = 0; i < backgroundElements.Length; i++)
                    if (backgroundElements[i].activeInHierarchy)
                        backgroundElements[i].SetActive(false);

            if (objectsToActivateOnDramaticScreen.Length > 0)
                for (int i = 0; i < objectsToActivateOnDramaticScreen.Length; i++)
                    if (!objectsToActivateOnDramaticScreen[i].activeInHierarchy)
                        objectsToActivateOnDramaticScreen[i].SetActive(true);
        }
        else
        {
            if (backgroundElements.Length > 0)
                for (int i = 0; i < backgroundElements.Length; i++)
                    if (!backgroundElements[i].activeInHierarchy)
                        backgroundElements[i].SetActive(true);

            if (objectsToActivateOnDramaticScreen.Length > 0)
                for (int i = 0; i < objectsToActivateOnDramaticScreen.Length; i++)
                    if (!objectsToActivateOnDramaticScreen[i].activeInHierarchy)
                        objectsToActivateOnDramaticScreen[i].SetActive(false);
        }
    }
    #endregion
}
