using UnityEngine;

// Scriptable object storing which tag refers to what, for the game objects that use the tags to find stuff, so that we can edit them here instead of having to change it on every single object that uses it
// OPTIMIZED
[CreateAssetMenu(fileName = "TagsReferences01", menuName = "Scriptable objects/Tags references")]
public class TagsReferences : ScriptableObject
{
    public string playerTag = "Player";
}
