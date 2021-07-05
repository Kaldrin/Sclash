using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseState : INPCState
{
    public INPCState DoState(AIBehaviors npc)
    {
        if (npc.stamina > 2)
            return npc.offenseState;

        //KEEP CURRENT BEHAVIOUR
        return npc.defenseState;
    }

    private bool HaveEnoughStamina(AIBehaviors npc)
    {
        if (npc.stamina >= npc.staminaTrigger)
            return true;

        return false;
    }
}
