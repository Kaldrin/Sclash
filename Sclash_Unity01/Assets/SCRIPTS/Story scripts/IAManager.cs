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

    public List<GameObject> enemyList;
    public List<EnemySpawner> IASpawner;

    public Action<EnemySpawner> DestroySpawnerAction;

    void Awake()
    {
        Instance = this;

        enemyList = new List<GameObject>();
        IASpawner = new List<EnemySpawner>();

        DestroySpawnerAction += DestroySpawner;
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

    private void DestroySpawner(EnemySpawner s)
    {
        IASpawner.Remove(s);
        Destroy(s.gameObject);
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
        if (!enemyList.Contains(enemy))
            enemyList.Add(enemy);
        enemy.GetComponent<IAScript_Solo>().OnIADeath += IADied;
    }
}