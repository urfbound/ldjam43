using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class MapManager : MonoBehaviour {
    //public elements

    //private elements
    //game-state elements
    private XmlDocument mapSaveOutput;
    //map tile elements
    private int currentMapId = 0;
    private int currentMapXDim = 5;
    private int currentMapYDim = 5;
    private List<List<GameObject>> currentMap;
    private List<GameObject> currentMapExits;
    private List<GameObject> currentMapItems;
    //map npc elements
    private List<GameObject> currentMapNpcs;

    //holders are used to keep the game hierarchy cleaner while editing in Unity (they're collapsible!)
    private Transform mapHolder;
    private Transform itemHolder;
    private Transform npcHolder;

    //Unity functions
    void Awake()
    {
        if (initalMapConfigRead() != true)
        {
            //TODO report error
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //my functions
    public XmlDocument getMapSaveOutput()
    {
        return mapSaveOutput;
    }

    private bool initalMapConfigRead()
    {
        //XmlTextReader myRdr = new XmlTextReader("./Configs/map_configs.xml");
        mapSaveOutput = new XmlDocument();
        mapSaveOutput.Load("./Configs/map_configs.xml");

        //todo parse all the data

        return true; //TODO add error-handling
    }

    //TODO add save feature to dump map out to state xml when saving
}
