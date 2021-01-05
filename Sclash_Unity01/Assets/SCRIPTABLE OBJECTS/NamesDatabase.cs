using UnityEngine;

[CreateAssetMenu(fileName = "NamesDatabse01", menuName = "Scriptable objects/Names database")]
public class NamesDatabase : ScriptableObject
{
    public string levelManagerName = "LevelManagerCanvas";
    public string rumbleManagerName = "RumbleManager";
    public string groundColliderName = "GroundCollider2D";
}
