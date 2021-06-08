using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawner : MonoBehaviour
{
    [Range(1,100)]
    public int spawnBeforeExhaust;

    public GameObject enemyPrefab;
    public bool spawnOnStart;

    private void Start()
    {
        if (spawnOnStart)
            SpawnEnemy();
    }

    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        if (spawnBeforeExhaust == 0)
        {
            DestroySpawner();
            return;
        }

        spawnBeforeExhaust--;

        GameObject enemySpawned = Instantiate(enemyPrefab, transform.position, transform.rotation);
        enemySpawned.GetComponent<StoryPlayer>().playerNum = IAManager.Instance.enemyList.Count + 1;
        enemySpawned.GetComponent<IAScript_Solo>().GetPlayer();
        enemySpawned.GetComponent<IAScript>().actionsList = new List<IAScript.Actions>()
        {
            new IAScript.Actions("Wait",1),
            new IAScript.Actions("Parry", 1),
            new IAScript.Actions("Attack",1),
            new IAScript.Actions("InterruptAttack", 1)
        };
        IAManager.Instance.EnemySpawned(enemySpawned);
    }

    private void DestroySpawner()
    {
        IAManager.Instance.DestroySpawnerAction(this);
    }
}