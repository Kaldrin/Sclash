using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenState : INPCState
{
    public INPCState DoState(AIBehaviors npc)
    {
        if (npc.opponentReady && DrawnDone(npc))
            return npc.idleState;

        return npc.frozenState;
    }

    private bool DrawnDone(AIBehaviors npc)
    {
        if (npc.playerState != Player.STATE.drawing && npc.oldPlayerState == Player.STATE.drawing)
            return true;

        return false;
    }
}
