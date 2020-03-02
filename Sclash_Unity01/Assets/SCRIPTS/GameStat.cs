using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameStat : MonoBehaviour
{
    [SerializeField] Stats stats = null;

    [Header("MAIN GAME INFOS")]
    [SerializeField] public TextMeshProUGUI gameIndex = null;
    [SerializeField] public Image stageIllustration = null;
    [SerializeField] public TextMeshProUGUI gameType = null;

    [Header("DATE")]
    [SerializeField] public TextMeshProUGUI hour = null;
    [SerializeField] public TextMeshProUGUI
        minute = null,
        day = null,
        month = null,
        year = null;

    [Header("CHARACTERS")]
    [SerializeField] public TextMeshProUGUI character1Name = null;
    [SerializeField] public TextMeshProUGUI character2Name = null;
    [SerializeField] public Image
        characterIllustration1 = null,
        characterIllustration2 = null;

    [Header("DETAILED INFOS")]
    [SerializeField] public TextMeshProUGUI finalScore = null;
    [SerializeField] public TextMeshProUGUI
        gameFinished = null,
        duration = null,
        roundsPlayed = null,
        scoreToWin = null;

    [Header("ROUNDS")]
    [SerializeField] Transform roundsListParent = null;
    [SerializeField] GameObject roundStatObject = null;
    List<RoundStat> roundsList = new List<RoundStat>();









    public void HideAllRounds()
    {
        for (int i = 0; i < roundsList.Count; i++)
        {
            roundsList[i].actionsInfosGameObject.SetActive(false);
        }
    }



    public void UpdateRoundsList(Game game)
    {
        roundStatObject.SetActive(true);


        GameObject currentlyInstantiatedRoundStatObject = null;
        RoundStat currentlyInstantiatedRoundStat = null;

        // Empty the list
        for (int i = 0; i < roundsListParent.childCount; i++)
        {
            if (roundsListParent.GetChild(i).gameObject != roundStatObject)
                Destroy(roundsListParent.GetChild(i));
        }
        roundsList.Clear();


        // Fill the list
        for (int i = 0; i < game.rounds.Count; i++)
        {
            currentlyInstantiatedRoundStatObject = Instantiate(roundStatObject, roundsListParent);
            currentlyInstantiatedRoundStat = currentlyInstantiatedRoundStatObject.GetComponent<RoundStat>();
            roundsList.Add(currentlyInstantiatedRoundStat);

            currentlyInstantiatedRoundStat.roundIndex.text = (i + 1).ToString();
            currentlyInstantiatedRoundStat.winnerName.text = "Jinmu";





            // Duration
            int
                minutes = 0,
                seconds = 0;

            minutes = (int)((game.rounds[i].duration - game.rounds[i].duration % 60) / 60);
            seconds = (int)(game.rounds[i].duration % 60);

            
            if (minutes < 10)
                currentlyInstantiatedRoundStat.minute.text = "0" + minutes.ToString();
            else
                currentlyInstantiatedRoundStat.minute.text = minutes.ToString();

            if (seconds < 10)
                currentlyInstantiatedRoundStat.second.text = "0" + seconds.ToString();
            else
                currentlyInstantiatedRoundStat.second.text = seconds.ToString();





            // Middle timecode
            int
                middleMinutes = 0,
                middleSeconds = 0;

            middleMinutes = (int)(((game.rounds[i].duration / 2) - game.rounds[i].duration % 60) / 60);
            middleSeconds = (int)((game.rounds[i].duration / 2) % 60);

            if (middleMinutes < 10)
                currentlyInstantiatedRoundStat.middleMinute.text = "0" + middleMinutes.ToString();
            else
                currentlyInstantiatedRoundStat.middleMinute.text = middleMinutes.ToString();

            if (middleSeconds < 10)
                currentlyInstantiatedRoundStat.middleSecond.text = "0" + middleSeconds.ToString();
            else
                currentlyInstantiatedRoundStat.middleSecond.text = middleSeconds.ToString();




            // End timecode
            currentlyInstantiatedRoundStat.endSecond.text = currentlyInstantiatedRoundStat.second.text;
            currentlyInstantiatedRoundStat.endMinute.text = currentlyInstantiatedRoundStat.minute.text;





            // Timelines
            currentlyInstantiatedRoundStat.playerNames[0].text = stats.characters[game.character1].name;
            currentlyInstantiatedRoundStat.playerNames[1].text = stats.characters[game.character1].name;


            currentlyInstantiatedRoundStat.UpdateLegendColors();
            currentlyInstantiatedRoundStat.UpdateActionsTimelinesDisplay(game.rounds[i]);
        }


        roundStatObject.SetActive(false);
    }
}
