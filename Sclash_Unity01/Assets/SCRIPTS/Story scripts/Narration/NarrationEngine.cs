using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.UI;



// For Sclash campaign mode

// REQUIREMENTS
// TextApparition script
// Player script
// Input system package

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
    [SerializeField] Animation skipCutSceneIndicatorAnimation = null;
    [SerializeField] Animation cutsceneSkipAnimation = null;
    float cutsceneSkipAmount = 0;
    float cutsceneSkipFillSpeed = 0.5f;
    [SerializeField] Image cutsceneSkipFillImage = null;
    bool skipInfoAppeared = false;




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
        if (cutsceneSkipAnimation && cutsceneSkipAnimation.GetComponent<CanvasGroup>())
            cutsceneSkipAnimation.GetComponent<CanvasGroup>().alpha = 0;
        if (skipCutSceneIndicatorAnimation && skipCutSceneIndicatorAnimation.GetComponent<CanvasGroup>())
            skipCutSceneIndicatorAnimation.GetComponent<CanvasGroup>().alpha = 0;
        if (cutsceneSkipFillImage)
            cutsceneSkipFillImage.fillAmount = 0;
    }
    private void Update()
    {
        if (isActiveAndEnabled && enabled)
            ManageCutscene();
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

    void CallNextNarrationEvent()                                                                                                                                          // CALL NEXT NARRATION EVENT
    {
        narrating = false;
        NarrationEvent(narrationEventsQueue[0], false);
        narrationEventsQueue.Remove(narrationEventsQueue[0]);
    }

    void ClearNarration()
    {
        narrationEventsQueue.Clear();
        EndNarrationEvent();
        CancelInvoke("EndSentence");
        CancelInvoke("EndNarrationEvent");
        CancelInvoke("NarrationEvent");
    }




    // CUTSCENE
    public void TriggerCutScene(GameObject cutscenePrefab = null)
    {
        ClearNarration();

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

            // Skip stuff
            cutsceneSkipAmount = 0;
            cutsceneSkipFillImage.fillAmount = 0;
            skipInfoAppeared = false;
            if (skipCutSceneIndicatorAnimation)
                skipCutSceneIndicatorAnimation.Play("FadeOut", PlayMode.StopAll);
        }
    }

    public void SkipCutscene()
    {
        // ANIMATION
        if (cutsceneSkipAnimation)
            cutsceneSkipAnimation.Play("Skip", PlayMode.StopAll);
        if (skipCutSceneIndicatorAnimation)
            skipCutSceneIndicatorAnimation.Play("FadeOut", PlayMode.StopAll);

        Invoke("EndCutscene", 0.9f);
        ClearNarration();
    }

    public void ManageCutscene()
    {
        if (inCutscene)
        {
            if (skipInfoAppeared)
            {
                if (InputManager.Instance && InputManager.Instance.skip && cutsceneSkipAmount < 1)
                    cutsceneSkipAmount += cutsceneSkipFillSpeed * Time.deltaTime;
                else if (cutsceneSkipAmount > 0)
                    cutsceneSkipAmount -= cutsceneSkipFillSpeed * Time.deltaTime;

                if (cutsceneSkipAmount < 0)
                    cutsceneSkipAmount = 0;

                cutsceneSkipFillImage.fillAmount = cutsceneSkipAmount;

                if (cutsceneSkipAmount >= 1)
                    SkipCutscene();
            }
            else if (InputManager.Instance && InputManager.Instance.skip)
            {
                skipInfoAppeared = true;
                if (skipCutSceneIndicatorAnimation)
                    skipCutSceneIndicatorAnimation.Play("FadeIn", PlayMode.StopAll);
            }
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
