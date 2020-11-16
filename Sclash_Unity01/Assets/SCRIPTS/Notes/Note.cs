using UnityEngine;

// Useless script that just allows for writing a note in the editor
// OPTIMIZED
public class Note : MonoBehaviour
{
    [TextArea] [Tooltip("Note variable used to store the note")]
    [SerializeField] string note = "Note about this object";

    void RemoveThisFuckingWarning()
    {
        note = note + note;
    }
}
