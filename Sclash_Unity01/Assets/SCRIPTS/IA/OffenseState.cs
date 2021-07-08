using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffenseState : INPCState
{
    public INPCState DoState(AIBehaviors npc)
    {
        if (!HaveEnoughStamina(npc))
            return npc.defenseState;

        //KEEP CURRENT BEHAVIOUR
        return npc.offenseState;
    }

    private bool HaveEnoughStamina(AIBehaviors npc)
    {
        if (npc.stamina >= npc.staminaTrigger)
            return true;

        return false;
    }

}
