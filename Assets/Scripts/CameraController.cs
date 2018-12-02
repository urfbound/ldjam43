using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private GameObject player;
    private bool isSetup;
    private Vector3 offset;

	// Use this for initialization
	void Start () {
        //offset = transform.position - player.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (!isSetup) { return; }
        //if (isSetup) { transform.position = player.transform.position + offset; }
        Vector3 playerPos = player.transform.position;
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
	}

    public void cameraSetup(GameObject playerIn)
    {
        player = playerIn;
        offset = transform.position - player.transform.position;
        isSetup = true;
    }
}
