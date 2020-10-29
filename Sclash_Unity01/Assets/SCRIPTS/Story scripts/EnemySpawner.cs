using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        GameObject enemySpawned = Instantiate(enemyPrefab, transform.position, transform.rotation);
        enemySpawned.GetComponent<StoryPlayer>().playerNum = GameManager_Story.StoryInstance.enemyList.Count + 1;
        enemySpawned.GetComponent<IAScript_Solo>().GetPlayer();
        enemySpawned.GetComponent<IAScript>().actionsList = new List<IAScript.Actions>()
        {
            new IAScript.Actions("Wait",1),
            new IAScript.Actions("Parry", 1)
        };
        GameManager_Story.StoryInstance.EnemySpawned(enemySpawned);
    }
}
