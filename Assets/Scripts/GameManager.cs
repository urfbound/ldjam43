using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public MapManager mapMgr;
    public float timescale;
    public int minuteInterval;

    private enum currentMode { PAUSE, PLAY };

    private object timeLock;
    private bool isCountingTime;
    private int date;
    private static int currentHour;
    private static int currentMinute;

	// Use this for initialization
	void Awake () {
        timeLock = new object();
        mapMgr = GetComponent<MapManager>();
        initGame();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void initGame()
    {
        //set up timer to count passing of days

    }

    private void endPlayerDay()
    {

    }

    //private IEnumerator dayTimer()
    //{
    //
    //}

    private void setIsCountingTime(bool newValue) { lock (timeLock) { isCountingTime = newValue; } }
    private bool getIsCountingTime() { lock (timeLock) { return isCountingTime; } }
    private int[] getCurrentTime() { lock(timeLock) { return new int[] { currentHour, currentMinute }; } }
    private void iterateTime()
    {
        lock (timeLock)
        {
            currentMinute += minuteInterval;
            while (currentMinute >= 60) { currentMinute -= 60; currentHour += 1; }

            //set any UI things

        }
    }
}
