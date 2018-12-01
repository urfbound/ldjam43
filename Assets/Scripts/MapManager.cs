using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class MapManager : MonoBehaviour {
    //public elements
    public GameObject[] intFloorFlavours;
    public GameObject[] intWallFlavours;

    public GameObject[] extFloorFlavours;
    public GameObject[] extWallFlavours;

    //private elements
    //game-state elements
    private World myWorld;
    private XmlDocument mapSaveOutput;
    private int homeMapId;
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
        myWorld = new World();
        homeMapId = initalMapConfigRead();
        if (initalMapConfigRead() == -1)
        {
            //TODO report error
            //TODO create fallback map
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

    void MapSetup()
    {
        mapHolder = new GameObject("CurrentMap").transform;
        itemHolder = new GameObject("CurrentItems").transform;
        npcHolder = new GameObject("CurrentNPCs").transform;

        //todo build out the map

        //todo add in the items

        //todo spawn the NPCs

    }

    void ChangeMaps(int outgoingMapId, int incomingMapId) //todo
    {

    }

    //my functions
    public XmlDocument getMapSaveOutput()
    {
        return mapSaveOutput;
    }

    private int initalMapConfigRead() //TODO tidy comments returns the ID of the first map to load at game-start
    {
        //XmlTextReader myRdr = new XmlTextReader("./Configs/map_configs.xml");
        mapSaveOutput = new XmlDocument();
        mapSaveOutput.Load("./Assets/Configs/map_configs.xml"); //TODO this path may need to change for the finished product

        //todo parse all the data
        XmlNode topWorld = mapSaveOutput.SelectSingleNode("/world");//we expect only one 'world' tag
        if (!topWorld.HasChildNodes) { Debug.Log("no world tag or it was empty");  return -1; }
        int homeNode;
        if (Int32.TryParse(topWorld.Attributes["initial-map"].Value, out homeNode) == false) { homeNode = -1; }
        //TODO check homeNode is legit
        XmlNodeList mapIn = topWorld.SelectNodes("map");
        if (mapIn.Count < 1) { Debug.Log("no map tags in world tag"); return -1; }//todo tidy debug messages //todo decide on a more useful minimum map count (e.g. 3)
        foreach(XmlNode node in mapIn)
        {
            bool myResult = myWorld.addMap(node);
            //if (!myResult) { Debug.Log("Error processing Map-ID " /*TODO ADD MAP-ID*/); }//TODO error-handling
        }

        Debug.Log("XML Parsing of map completed");
        //TODO check home-node can be drawn, if not fill it with a basic map and write a trace message
        return homeNode; //TODO add error-handling
    }

    private class World
    {
        private readonly int MAX_MAPS = 100;
        private Map[] myMaps;

        public World()
        {
            myMaps = new Map[MAX_MAPS];
            for(int i=0; i< MAX_MAPS; i++) { myMaps[i] = null; }
        }

        public bool addMap(XmlNode mapIn)
        {
            bool parseSuccess = false;
            //process mapIn ID. if there's no ID, the ID is out of range, or if it's a duplicate ID, we disregard this map
            int mapInId;
            if(Int32.TryParse(mapIn.Attributes["map-id"].Value, out mapInId) == false) { return false; }
            if(mapInId < 0 || mapInId >= MAX_MAPS) { return false; }
            if(myMaps[mapInId] != null) { return false; }
            //if successful, process Map contents into Map object //TODO

            //evaluate success of Map Processing //TODO

            //if successful, return true. else write debug log and return false //TODO
            if (parseSuccess) { return true; }
            else { Debug.Log("Late failure processing Map ID " + mapInId); myMaps[mapInId] = null; return false; }//undo any work we may have done before returning
        }
    }

    private class Map
    {
        private int xDim, yDim;
    }
}
