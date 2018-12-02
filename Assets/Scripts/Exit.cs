using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour {
    public MapManager mapCtrlr;

    private int nextMapId;
    private int nextMapStartX, nextMapStartY;
    private bool targetsSet;//TODO for debugging use if req'd

	// Use this for initialization
	void Start () {
        targetsSet = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            //load the other map
            Debug.Log("Request to move to mapId " + nextMapId + " at coordinates (" + nextMapStartX + ", " + nextMapStartY + ")" );
        }
    }

    public void setParams(int remoteMapIn, int remoteStartXIn, int remoteStartYIn)
    {
        nextMapId = remoteMapIn;
        nextMapStartX = remoteStartXIn;
        nextMapStartY = remoteStartYIn;
        targetsSet = true;
    }
}
