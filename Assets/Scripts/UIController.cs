using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public Canvas canvasToControl;

    private bool isSetup;
    private Text InGamePCDia, InGameNPCDia, InGameUIDate, InGameMoney;

    void Awake()
    {
        isSetup = false;
        InGamePCDia = GameObject.Find("InGamePlayerDialogue").GetComponent<Text>();
        InGameNPCDia = GameObject.Find("InGameNpcDialogue").GetComponent<Text>();
        InGameUIDate = GameObject.Find("UIDate").GetComponent<Text>();
        InGameMoney = GameObject.Find("UIMoney").GetComponent<Text>();

        InGameMoney.text = "$69,420 lol";
        InGamePCDia.enabled = false;
        InGameNPCDia.enabled = false;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setUITime(int dayIn, int hourIn, int minuteIn)
    {
        int myMonth = 1; int myWeek = 1;
        while(dayIn > 28) { myMonth += 1; dayIn -= 28; }
        while(dayIn > 7) { myWeek += 1; dayIn -= 7; }
        InGameUIDate.text = "Month " + myMonth + " Week " + myWeek + " Day " + dayIn + " " + hourIn + ":" + ((minuteIn==0)? "00" : minuteIn.ToString());
    }
}
