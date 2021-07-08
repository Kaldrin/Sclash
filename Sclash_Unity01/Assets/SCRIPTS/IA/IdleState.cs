using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : INPCState
{
    public INPCState DoState(AIBehaviors npc)
    {
        Debug.Log("Is Close Enough ? " + IsCloseEnough(npc));

        //Get Closer or get more stamina
        if (HaveEnoughStamina(npc))
            GetCloser(npc);
        else
            RecoverStamina(npc);

        if (IsCloseEnough(npc))
        {
            if (HaveEnoughStamina(npc))
            {
                return npc.offenseState;
            }
            else
            {
                return npc.defenseState;
            }
        }

        return npc.idleState;
    }

    private void GetCloser(AIBehaviors npc)
    {
        //Move closer to target
        Debug.Log("Get Closer");
        npc.transform.position += new Vector3(0.01f, 0, 0) * -Mathf.Sign(npc.transform.position.x);
    }

    private void RecoverStamina(AIBehaviors npc)
    {
        GetFarther(npc);
    }

    private void GetFarther(AIBehaviors npc)
    {
        //Move away from target
        Debug.Log("Get Farther");
        npc.transform.position += new Vector3(0.01f, 0, 0) * Mathf.Sign(npc.transform.position.x);
    }

    private bool HaveEnoughStamina(AIBehaviors npc)
    {
        if (npc.stamina >= npc.staminaTrigger)
            return true;

        return false;
    }

    private bool IsCloseEnough(AIBehaviors npc)
    {
        if (npc.distance <= npc.closeUpDistance)
            return true;

        return false;
    }
}