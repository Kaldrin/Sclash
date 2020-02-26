using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoundStat : MonoBehaviour
{
    [Header("ROUND MAIN INFOS")]
    [SerializeField] public TextMeshProUGUI roundIndex = null;
    [SerializeField] public TextMeshProUGUI
        minute = null,
        second = null;
    [SerializeField] public TextMeshProUGUI winnerName = null;



    [Header("ACTIONS TIMELINES")]
    [SerializeField] public TextMeshProUGUI middleMinute = null;
    [SerializeField] public TextMeshProUGUI
        middleSecond = null,
        endMinute = null,
        endSecond = null;

    [SerializeField] public List<TextMeshProUGUI> playerNames = null;

    [SerializeField] Transform[] actionsTimelinesParents = null;
    

    [SerializeField] GameObject[] actionMarkerObjects;
    [SerializeField] Vector2 xLimitsForActionPlacement = new Vector2(-73, 73);
    [SerializeField] RectTransform[] xLeftLimitsObjectsForActionPlacement = new RectTransform[2];
    [SerializeField] RectTransform[] xRightLimitsObjectsForActionPlacement = new RectTransform[2];


    [Header("ACTIONS COLORS")]
    [SerializeField] Color charge;
    [SerializeField] Color forwardAttack,
        backWardsAttack,
        neutralAttack,
        death,
        forwardDash,
        backwardsDash,
        pommel,
        successfulPommel,
        parry,
        successfulParry,
        clash,
        dodge;

    [Header("ACTIONS LEGEND")]
    [SerializeField] Image chargeLegend = null;
    [SerializeField] Image forwardAttackLegend = null,
        backWardsAttackLegend = null,
        neutralAttackLegend = null,
        deathLegend = null,
        forwardDashLegend = null,
        backwardsDashLegend = null,
        pommelLegend = null,
        successfulPommelLegend = null,
        parryLegend = null,
        successfulParryLegend = null,
        clashLegend = null,
        dodgeLegend = null;





    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void UpdateLegendColors()
    {
        chargeLegend.color = charge;
        forwardAttackLegend.color = forwardAttack;
        backWardsAttackLegend.color = backWardsAttack;
        neutralAttackLegend.color = neutralAttack;
        deathLegend.color = death;
        forwardDashLegend.color = forwardDash;
        backwardsDashLegend.color = backwardsDash;
        pommelLegend.color = pommel;
        successfulPommelLegend.color = successfulPommel;
        parryLegend.color = parry;
        successfulParryLegend.color = successfulParry;
        clashLegend.color = clash;
        chargeLegend.color = dodge;
    }



    public void UpdateActionsTimelinesDisplay(Round round)
    {
        GameObject currentlyInstantiatedActionMarkerObject = null;
        Image currentlyInstantiatedActionMarkerImage = null;
        Color actionColor = Color.white;


        for (int y = 0; y < playerNames.Count; y++)
        {
            actionMarkerObjects[y].SetActive(true);


            // Empty actions timelines
            for (int i = 0; i < actionsTimelinesParents[y].childCount; i++)
            {
                if (actionsTimelinesParents[y].GetChild(i).gameObject != actionMarkerObjects[y] && actionsTimelinesParents[y].GetChild(i).gameObject.activeInHierarchy)
                    Destroy(actionsTimelinesParents[y].GetChild(i));
            }


            // Fill actions timelines
            for (int i = 0; i < round.actions[y].actionsList.Count; i++) // Problème ici, actions n'est pas set pour une raison que j'ignore
            {
                currentlyInstantiatedActionMarkerObject = Instantiate(actionMarkerObjects[y], actionsTimelinesParents[y]);
                currentlyInstantiatedActionMarkerImage = currentlyInstantiatedActionMarkerObject.GetComponent<ActionMarker>().colorImage;


                // Position in the timeline

                float actionXPos = (round.actions[y].actionsList[i].timeCode / round.duration) * (xRightLimitsObjectsForActionPlacement[y].localPosition.x - xLeftLimitsObjectsForActionPlacement[y].localPosition.x) + xLeftLimitsObjectsForActionPlacement[y].localPosition.x;
                //float actionXPos = (round.actions[y].actionsList[i].timeCode / round.duration) * (xRightLimitsObjectsForActionPlacement[y].anchoredPosition.x - xLeftLimitsObjectsForActionPlacement[y].anchoredPosition.x);
                //float actionXPos = (round.actions[y].actionsList[i].timeCode / round.duration) * (xLimitsForActionPlacement.y - xLimitsForActionPlacement.x) + xLimitsForActionPlacement.y;
                currentlyInstantiatedActionMarkerObject.GetComponent<RectTransform>().localPosition = new Vector3(actionXPos, currentlyInstantiatedActionMarkerObject.transform.localPosition.y, currentlyInstantiatedActionMarkerObject.transform.localPosition.z);


                // color
                switch (round.actions[y].actionsList[i].name)
                {
                    case ACTION.charge:
                        actionColor = charge;
                        break;

                    case ACTION.forwardAttack:
                        actionColor = forwardAttack;
                        break;

                    case ACTION.backwardsAttack:
                        actionColor = backWardsAttack;
                        break;

                    case ACTION.neutralAttack:
                        actionColor = neutralAttack;
                        break;

                    case ACTION.death:
                        actionColor = death;
                        break;

                    case ACTION.forwardDash:
                        actionColor = forwardDash;
                        break;

                    case ACTION.backwardsDash:
                        actionColor = backwardsDash;
                        break;

                    case ACTION.pommel:
                        actionColor = pommel;
                        break;

                    case ACTION.successfulPommel:
                        actionColor = successfulPommel;
                        break;

                    case ACTION.parry:
                        actionColor = parry;
                        break;

                    case ACTION.successfulParry:
                        actionColor = successfulParry;
                        break;

                    case ACTION.clash:
                        actionColor = clash;
                        break;

                    case ACTION.dodge:
                        actionColor = dodge;
                        break;
                }


                currentlyInstantiatedActionMarkerImage.color = actionColor;
                currentlyInstantiatedActionMarkerObject.GetComponent<ActionMarker>().UpdateLevelMarkers(round.actions[y].actionsList[i].level);
            }


            actionMarkerObjects[y].SetActive(false);
        }
    }
}
