using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float playerSpeed = 1f;

    private bool canMove = true; //TODO add conditional activation of canMove. Should be false on wake until it's OK to move.
    private float xAxisDeadZone = 0f;
    private float yAxisDeadZone = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (canMove)
        {
            float horizIn = Input.GetAxis("Horizontal");
            if(Math.Abs(horizIn) < xAxisDeadZone) { horizIn = 0f; }
            float vertIn = Input.GetAxis("Vertical");
            if (Math.Abs(vertIn) < xAxisDeadZone) { vertIn = 0f; }
            //TODO add speed smoothing (e.g. max speed if horiz/vert are maxed)
            float xPos = transform.position.x + (horizIn * playerSpeed);
            float yPos = transform.position.y + (vertIn * playerSpeed);
            
            Vector3 playerPos = new Vector3(xPos, yPos);
            transform.position = playerPos;
        }
	}
}
