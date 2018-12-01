using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public MapManager mapMgr;

	// Use this for initialization
	void Awake () {
        mapMgr = GetComponent<MapManager>();
        initGame();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void initGame()
    {

    }
}
