using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Story : GameManager
{

    public static GameManager_Story storyInstance = null;
    public static GameManager_Story StoryInstance
    {
        get
        {
            if (storyInstance == null)
                storyInstance = new GameManager_Story();

            return storyInstance;
        }
    }

    public GameObject[] SpawnerList;
    public List<GameObject> enemyList;

    public override void Awake()
    {
        Instance = this;
    }

    public override void Start()
    {

    }

    public override void FixedUpdate()
    {

    }

    public override void Update()
    {

    }

    public void EnemySpawned()
    {
        Debug.Log("An enemy has spawned");
    }

}