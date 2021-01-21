using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IAManager : MonoBehaviour
{
    public static IAManager Instance
    {
        get; set;
    }

    public List<GameObject> enemyList = new List<GameObject>();
    public List<EnemySpawner> IASpawner = new List<EnemySpawner>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (IASpawner.Count == 0)
        {
            EnemySpawner[] temp = FindObjectsOfType<EnemySpawner>();
            foreach (EnemySpawner t in temp)
            {
                IASpawner.Add(t);
            }
        }
    }

    private void IADied(IAScript_Solo ia)
    {
        Debug.Log(ia + "died");
        foreach (EnemySpawner t in IASpawner)
        {
            t.Invoke("SpawnEnemy", 1f);
        }
    }

    public void EnemySpawned(GameObject enemy)
    {
        Debug.Log("An enemy has spawned");
        enemyList.Add(enemy);
        enemy.GetComponent<IAScript_Solo>().OnIADeath += IADied;
    }
}