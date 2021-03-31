using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// For Sclash campaign mode

// REQUIREMENTS
// TextApparition script
// Player script

/// <summary>
///  Manages the display of narration texts, dialogues & cutscenes with its canvas prefab & children, giving public methods to call with narration data structs
/// </summary>

// Unity 2019.4.14
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
    Player player = null;



    [Header("CUTSCENES")]
    [SerializeField] Transform cutsceneParent = null;
    float maxCutSceneDuration = 60f;
    GameObject currentCutscene = null;
    bool inCutscene = false;


    [Header("VOICE ACTING")]
    [SerializeField] AudioSource narratorAudioSource = null;


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


    // NARRATION
    public void NarrationEvent(NarrationEventData narrationEventData, bool playStartAnim = true)                                                                                // NARRATION EVENT
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

        float duration = 0f;

        if (currentNarrationEvent.sentences.Count > currentNarrationEventIndex)
        {
            duration = currentNarrationEvent.sentences[currentNarrationEventIndex].duration;

            if (currentNarrationEvent.sentences[currentNarrationEventIndex].textKey != null)
            {
                textApparitionComponent.textKey = currentNarrationEvent.sentences[currentNarrationEventIndex].textKey;
                textApparitionComponent.TransfersTrad();
            }
            if (currentNarrationEvent.sentences[currentNarrationEventIndex].audioClipKey != null && currentNarrationEvent.sentences[currentNarrationEventIndex].audioClipKey != "")
                if (LanguageManager.Instance && narratorAudioSource)
                {
                    narratorAudioSource.clip = LanguageManager.Instance.GetAudioClip(currentNarrationEvent.sentences[currentNarrationEventIndex].audioClipKey);
                    narratorAudioSource.Play();
                    // If there's a voice clip, the duration of the clip overrides the input sentence duration
                    if (narratorAudioSource.clip)
                        duration = narratorAudioSource.clip.length;
                }
        }


        Invoke("EndSentence", duration);
    }

    void EndSentence()                                                                                                                                                          // END SENTENCE
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

    void EndNarrationEvent()                                                                                                                                                    // END NARRATION EVENT
    {
        // ANIMATION
        textSubtitlesAnimator.Play("FadeOut", PlayMode.StopAll);


        
        currentNarrationEventIndex = 0;


        if (narrationEventsQueue.Count > 0)
            Invoke("CallNextNarrationEvent", 1.5f);
        else
        {
            narrating = false;
            // ANIMATION
            generalAnimator.Play("FadeOut", PlayMode.StopAll);
        }
    }

    void CallNextNarrationEvent()                                                                                                                                               // CALL NEXT NARRATION EVENT
    {
        narrating = false;
        NarrationEvent(narrationEventsQueue[0], false);
        narrationEventsQueue.Remove(narrationEventsQueue[0]);
    }






    // CUTSCENE
    public void TriggerCutScene(GameObject cutscenePrefab = null)
    {
        currentCutscene = Instantiate(cutscenePrefab, cutsceneParent);
        inCutscene = true;
        if (player == null)
        {
            if (FindObjectOfType<Player>())
                player = FindObjectOfType<Player>();
            else
                Debug.LogWarning("Warning from NarrationEngine instance, there seems to be no player in the scene");
        }

        player.SwitchState(Player.STATE.cutscene);


        // Fail safe
        Invoke("EndCutscene", maxCutSceneDuration);
    }

    public void EndCutscene()
    {
        if (inCutscene)
        {
            if (currentCutscene)
                Destroy(currentCutscene);
            inCutscene = false;
            player.SwitchState(Player.STATE.normal);
        }
    }














    // EDITOR
    private void OnDrawGizmos()
    {
        if (player == null)
        {
            if (FindObjectOfType<Player>())
                player = FindObjectOfType<Player>();
            else
                Debug.LogWarning("Warning from NarrationEngine instance, there seems to be no player in the scene");
        }
    }
}
