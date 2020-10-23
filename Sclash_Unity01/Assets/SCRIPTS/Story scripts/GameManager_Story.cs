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

    public List<GameObject> enemyList = new List<GameObject>();

    public override void Awake()
    {
        Instance = this;
    }

    public override void Start()
    {
        SwitchState(GAMESTATE.game);
    }

    public override void FixedUpdate()
    {

    }

    public override void Update()
    {

    }

    public void EnemySpawned(GameObject enemy)
    {
        Debug.Log("An enemy has spawned");
        enemyList.Add(enemy);
    }

}