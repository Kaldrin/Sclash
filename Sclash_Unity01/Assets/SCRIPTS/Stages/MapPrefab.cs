using UnityEngine;

// References classes : Scarf02



// This script is contained at the root of a stage prefab. It allows to reference which objects should do what in the stages
// OPTIMIZED
public class MapPrefab : MonoBehaviour
{
    [SerializeField] bool previewDramaticScreenInEditor = true;
    [SerializeField] bool previewMenuInEditor = false;
    [Tooltip("The reference to the objects of the stage prefab that should be deactivated during the dramatic screen")]
    [SerializeField] public GameObject[] backgroundElements = null;
    [SerializeField] public GameObject[] objectsToActivateOnDramaticScreen = null;
    [Tooltip("The objects that won't be noticed when a menu overlays the stage, so we can disable them during menu sequences to enhance performances")]
    [SerializeField] public GameObject[] objectsToDisableDuringMenu = null;
    [SerializeField] Animator animatorToEnableOnStart = null;
    [SerializeField] bool disableAnimatorAfterDuration = true;
    [SerializeField] float durationBeforeDisablingAnimator = 5f;









    #region FUNCTIONS
    // BASIC FUNCTIONS
    private void Start()                                                                        // START
    {
        EnableClothCollision();
    }






    // DRAMATIC SCREEN
    public void TriggerDramaticScreen()
    {
        // Disable elements
        if (backgroundElements != null && backgroundElements.Length > 0)
            for (int i = 0; i < backgroundElements.Length; i++)
                backgroundElements[i].SetActive(false);
        else
            Debug.Log("No background element to disable during dramatic screen for this stage");


        // Enable elements
        if (objectsToActivateOnDramaticScreen != null && objectsToActivateOnDramaticScreen.Length > 0)
            for (int i = 0; i < objectsToActivateOnDramaticScreen.Length; i++)
                objectsToActivateOnDramaticScreen[i].SetActive(true);
        else
            Debug.Log("No element to enable during dramatic screen for this stage");
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
        Scarf02[] scarfs = FindObjectsOfType<Scarf02>();                                                        // FIND


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
        // DRAMATIC SCREEN PREVIEW
        if (previewDramaticScreenInEditor)
        {
            if (backgroundElements.Length > 0)
                for (int i = 0; i < backgroundElements.Length; i++)
                    if (backgroundElements[i] != null)
                        if (backgroundElements[i].activeInHierarchy)
                            backgroundElements[i].SetActive(false);

            if (objectsToActivateOnDramaticScreen.Length > 0)
                for (int i = 0; i < objectsToActivateOnDramaticScreen.Length; i++)
                    if (objectsToActivateOnDramaticScreen[i] != null)
                        if (!objectsToActivateOnDramaticScreen[i].activeInHierarchy)
                            objectsToActivateOnDramaticScreen[i].SetActive(true);
        }
        else
        {
            if (backgroundElements.Length > 0)
                for (int i = 0; i < backgroundElements.Length; i++)
                    if (backgroundElements[i] != null)
                        if (!backgroundElements[i].activeInHierarchy)
                            backgroundElements[i].SetActive(true);

            if (objectsToActivateOnDramaticScreen.Length > 0)
                for (int i = 0; i < objectsToActivateOnDramaticScreen.Length; i++)
                    if (objectsToActivateOnDramaticScreen[i] != null)
                        if (!objectsToActivateOnDramaticScreen[i].activeInHierarchy)
                            objectsToActivateOnDramaticScreen[i].SetActive(false);
        }


        // MENU PREVIEW
        if (objectsToDisableDuringMenu != null && objectsToDisableDuringMenu.Length > 0)
        {
            if (previewMenuInEditor)
            {
                for (int i = 0; i < objectsToDisableDuringMenu.Length; i++)
                    if (objectsToDisableDuringMenu[i] != null)
                        if (objectsToDisableDuringMenu[i].activeInHierarchy)
                            objectsToDisableDuringMenu[i].SetActive(false);
            }
            else
                for (int i = 0; i < objectsToDisableDuringMenu.Length; i++)
                    if (objectsToDisableDuringMenu[i] != null)
                        if (!objectsToDisableDuringMenu[i].activeInHierarchy)
                            objectsToDisableDuringMenu[i].SetActive(true);
        }
    }
    #endregion
}
