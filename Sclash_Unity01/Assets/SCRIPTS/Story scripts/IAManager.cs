using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAManager : MonoBehaviour
{
    public static IAManager Instance
    {
        get; set;
    }

    public List<GameObject> enemyList = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }


    public void EnemySpawned(GameObject enemy)
    {
        Debug.Log("An enemy has spawned");
        enemyList.Add(enemy);
    }

}