using UnityEngine;

// Settings for the cheat options of the player, stored in a scriptable object for organisation purposes
[CreateAssetMenu(fileName = "PlayerCheatsSettings01", menuName = "ScriptableObjects/Player cheats settings")]
public class PlayerCheatsParameters : ScriptableObject
{
    public KeyCode clashCheatKey = KeyCode.Alpha1;
    public KeyCode deathCheatKey = KeyCode.Alpha2;
    public KeyCode staminaCheatKey = KeyCode.Alpha4;
    public KeyCode stopStaminaRegenCheatKey = KeyCode.Alpha6;
    public KeyCode triggerStaminaRecupAnim = KeyCode.Alpha7;

    public bool useTransparencyForDodgeFrames = true;
    public bool useExtraDiegeticFX = true;
    public bool useRangeFlareFX = true;
    public bool useRangeShadow = true;
    public bool infiniteStamina = false;
}
