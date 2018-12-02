using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public MapManager mapMgr;
    public UIController uiMgr;
    public float timescale;
    public int minuteInterval;
    public int sleepyHour, normalWakeUpHour, lateWakeUpHour;

    private enum currentMode { PAUSE, PLAY };

    private object timeLock;
    private bool isCountingTime, killCoroutine;
    private int date;
    private static int currentHour;
    private static int currentMinute;

	// Use this for initialization
	void Awake () {
        timeLock = new object();
        killCoroutine = false; date = 1; currentHour = 7; currentMinute = 0;//TODO fix the case where saving after a pass-out lets you cheat here
        setIsCountingTime(false); /*todo remove after this comment*/setIsCountingTime(true);
        mapMgr = GetComponent<MapManager>();
        initGame();
        //StartCoroutine(dayTimer());
        //InvokeRepeating("iterateTime", 0.0f, 30f * timescale);
        InvokeRepeating("iterateTime", 3f, 15.0f*timescale);
        setIsCountingTime(true);//TODO do this programattically
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void initGame()
    {
        //set up timer to count passing of days

        //start timer

    }

    private void endPlayerDaySleep(bool didPassOut)
    {
        //stop timer

        //send request to immobilize player

        //send request to play player sleep animation

        //send request to send player to init-map

        //send request to iterate the date

        //restart timer

    }

    private void remindPlayerIsTired()
    {
        //send request to immobilize player

        //send request to play player yawn animation

        //send request to mobilize player 

    }

    private IEnumerator dayTimer()
    {
        while (!killCoroutine)
        {
            iterateTime();
            yield return new WaitForSeconds(30*timescale);
        }
    }

    private void setIsCountingTime(bool newValue) { lock (timeLock) { isCountingTime = newValue; } }
    private bool getIsCountingTime() { lock (timeLock) { return isCountingTime; } }
    private int[] getCurrentTime() { lock(timeLock) { return new int[] { currentHour, currentMinute }; } }
    private void iterateTime()
    {
        //lock (timeLock)
        //{
            if (isCountingTime)
            {
                Debug.Log("now iterating time at " + currentHour + " " + currentMinute);
                //update the date
                currentMinute += minuteInterval;
                while (currentMinute >= 60) { currentMinute -= 60; currentHour += 1; }
                //set any UI things
                uiMgr.setUITime(date, currentHour, currentMinute);
                //do any scheduled activities
                //TODO stretch goal
                //send any player requests (yawn, sleep)
                if (currentHour == 24) { endPlayerDaySleep(true); }
                else if (currentHour >= sleepyHour) { remindPlayerIsTired(); }
            }
        //}
    }
}
