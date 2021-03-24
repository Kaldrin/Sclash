using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class NarrationEngine : MonoBehaviour
{
    // SINGLETON
    public static NarrationEngine Instance;

    [Header("COMPONENTS")]
    [SerializeField] CanvasGroup generalCanvasGroup = null;
    [SerializeField] CanvasGroup textSubtitlesCanvasGroup = null;
    [SerializeField] Animation generalAnimator = null;
    [SerializeField] Animation textSubtitlesAnimator = null;
    [SerializeField] TextApparition textApparitionComponent = null;


    // DATA
    [System.Serializable]
    public struct Sentence
    {
        public string textKey;
        public float duration;
        public string audioClipKey;
        public float delay;
    }
    [System.Serializable]
    public struct NarrationEventData
    {
        public List<Sentence> sentences;   
    }



    // PARAMETERS
    List<NarrationEventData> narrationEventsQueue = new List<NarrationEventData>();
    int currentNarrationEventIndex = 0;
    NarrationEventData currentNarrationEvent;
    bool narrating = false;






    private void Awake()                                                                                                                                                        // AWAKE
    {
        Instance = this;
    }
    private void Start()                                                                                                                                                        // START
    {
        if (generalCanvasGroup)
            generalCanvasGroup.alpha = 0;
        if (textSubtitlesCanvasGroup)
            textSubtitlesCanvasGroup.alpha = 0;
    }



    public void NarrationEvent(NarrationEventData narrationEventData, bool playStartAnim = true)                                                                                  // NARRATION EVENT
    {
        if (narrating)
            narrationEventsQueue.Add(narrationEventData);
        else
        {
            // ANIMATION
            if (playStartAnim)
                generalAnimator.Play("FadeIn", PlayMode.StopAll);


            currentNarrationEvent = narrationEventData;
            currentNarrationEventIndex = 0;
            narrating = true;

            // TEXT
            if (narrationEventData.sentences != null && narrationEventData.sentences.Count > 0)
                ReadSentence();
        }
    }

    void ReadSentence()                                                                                                                                                         // READ SENTENCE
    {
        // ANIMATION
        textSubtitlesAnimator.Play("FadeIn", PlayMode.StopAll);


        if (currentNarrationEvent.sentences.Count > currentNarrationEventIndex)
            if (currentNarrationEvent.sentences[currentNarrationEventIndex].textKey != null)
            {
                textApparitionComponent.textKey = currentNarrationEvent.sentences[currentNarrationEventIndex].textKey;
                textApparitionComponent.TransfersTrad();
            }

        Invoke("EndSentence", currentNarrationEvent.sentences[currentNarrationEventIndex].duration);
    }

    void EndSentence()                                                                                                                                                              // END SENTENCE
    {
        // ANIMATION
        textSubtitlesAnimator.Play("FadeOut", PlayMode.StopAll);

        if (currentNarrationEvent.sentences.Count - 1 > currentNarrationEventIndex)
        {
            currentNarrationEventIndex++;
            Invoke("ReadSentence", currentNarrationEvent.sentences[currentNarrationEventIndex - 1].delay);
        }
        else
            Invoke("EndNarrationEvent", currentNarrationEvent.sentences[currentNarrationEventIndex].delay);
    }

    void EndNarrationEvent()                                                                                                                                                        // END NARRATION EVENT
    {
        // ANIMATION
        textSubtitlesAnimator.Play("FadeOut", PlayMode.StopAll);


        narrating = false;
        currentNarrationEventIndex = 0;


        if (narrationEventsQueue.Count > 0)
            Invoke("CallNextNarrationEvent", 1.5f);
        else
            // ANIMATION
            generalAnimator.Play("FadeOut", PlayMode.StopAll);
    }

    void CallNextNarrationEvent()                                                                                                                                                    // CALL NEXT NARRATION EVENT
    {
        NarrationEvent(narrationEventsQueue[0], false);
        narrationEventsQueue.Remove(narrationEventsQueue[0]);
    }
}
