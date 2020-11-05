using UnityEngine;

// This script is contained at the root of a stage prefab. It allows to reference which objects should do what in the stages
// OPTIMIZED
public class MapPrefab : MonoBehaviour
{
    [Tooltip("The reference to the objects of the stage prefab that should be deactivated during the dramatic screen")]
    [SerializeField] public GameObject[] backgroundElements = null;
}
