using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParticleSet", menuName = "Scriptable objects/Particle set")]
public class ParticleSetScriptable : ScriptableObject
{
    public ParticleSystem AttackNeutral;
    public ParticleSystem DashBack;
    public ParticleSystem DashFront;
    public ParticleSystem AttackBack;
    public ParticleSystem AttackFront;
    public ParticleSystem WalkBack;
    public ParticleSystem WalkFront;
    public ParticleSystem Step01;
    public ParticleSystem Step02;
    public ParticleSystem BackStep01;
    public ParticleSystem BackStep02;
}
