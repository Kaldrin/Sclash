using UnityEngine;

public class Note : MonoBehaviour
{
    [TextArea] [Tooltip("Note variable used to store the note")]
    [SerializeField] string note = "Note about this object";
}
