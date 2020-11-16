using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DestructibleProfile01", menuName = "Scriptable objects/Destructible profile")]
public class DestructibleProfile : ScriptableObject
{
    public Vector2 fallingPartHorizontalRandomSpeedRange = new Vector2(3f, 4f);
    public Vector2 fallingPartVerticalRandomSpeedRange = new Vector2(4f, 6f);
    public Vector2 fallingPartAngularRandomSpeedRange = new Vector2(-3f, 3f);
    public float fallingPartMaxSpeed = -8f;
}
