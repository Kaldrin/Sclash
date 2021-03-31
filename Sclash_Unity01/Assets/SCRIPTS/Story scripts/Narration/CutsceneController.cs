using System.Collections;
using System.Collections.Generic;
using UnityEngine;




// For Sclash campaign mode
// OPTIMIZED

// REQUIREMENTS
// Goes with the NarrationEngine, on the NarrationCanvas prefab

/// <summary>
/// This scrip is to put on a cutscene prefab, it allows for its animation to control when to narrate stuff and end the cutscene
/// </summary>

// UNITY 2019.4.14
public class CutsceneController : MonoBehaviour
{
    [SerializeField] List<NarrationEngine.NarrationEventData> narrationEvents = new List<NarrationEngine.NarrationEventData>();


    public void TriggerNarrationEvent(int index = 0)
    {
        if (NarrationEngine.Instance && narrationEvents.Count > index)
            NarrationEngine.Instance.NarrationEvent(narrationEvents[index]);
    }

    public void EndCutscene()
    {
        if (NarrationEngine.Instance)
            NarrationEngine.Instance.EndCutscene();
    }
}
