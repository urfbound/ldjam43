using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class MapManager : MonoBehaviour {
    //public elements
    public bool testMode = false;

    public GameObject playerPrefab, mainCameraPrefab;

    public GameObject[] interiorFloorTiles;
    public GameObject[] interiorWallTiles;
    public GameObject[] interiorExitTiles;

    public GameObject[] exteriorFloorTiles;
    public GameObject[] exteriorWallTiles;
    public GameObject[] exteriorExitTiles;

    public int tileSideLength, playerTileOffset;

    public enum tileTypes { FLOOR, WALL, WATER, PLANTER, EXIT, INVALID };
    public enum mapTypes { HOUSE, FARM, RANCH, INVALID };

    //private elements
    //game-state elements
    private World myWorld; private Map currentMap;
    private XmlDocument mapSaveOutput;
    private int homeMapId;
    //map tile elements
    private int currentMapId = 0;
    private int currentMapStartX, currentMapStartY;
    private int currentMapXDim = 5;
    private int currentMapYDim = 5;
    private List<List<GameObject>> currentMapObjs;
    private List<GameObject> currentMapExits;
    private List<GameObject> currentMapItems;
    //map pc/npc elements
    private GameObject myPlayer; private PlayerController myPlayerScript; private GameObject myMainCamera; private CameraController myMainCameraController;
    private GameObject myUI; private UIController myUiController;
    private List<GameObject> currentMapNpcs;

    //holders are used to keep the game hierarchy cleaner while editing in Unity (they're collapsible!)
    private GameObject mapHolder;
    private GameObject itemHolder;
    private GameObject npcHolder;

    //Unity functions
    void Awake()
    {
        myWorld = new World();
        //set up the player
        myPlayer = Instantiate(playerPrefab, new Vector3(-200f, 200f, 0f), Quaternion.identity);
        myPlayerScript = myPlayer.GetComponent<PlayerController>();
        myPlayerScript.setCanMove(false);
        //set up the camera
        myMainCamera = Instantiate(mainCameraPrefab, new Vector3(0f, 0f, -10f), Quaternion.identity);
        myMainCameraController = myMainCamera.GetComponent<CameraController>();
        myMainCameraController.cameraSetup(myPlayer);
        //set up the UI
        myUI = GameObject.Find("Canvas"); myUiController = myUI.GetComponent<UIController>();
        myPlayer.SetActive(false);

        //figure out if working from a save file

        homeMapId = initalMapConfigRead(false, 0);//TODO add support for save files
        if (homeMapId == -1)
        {
            //TODO report error
            Debug.Log("Fatal map parse error initializing MapManager");
            //TODO create fallback map
            homeMapId = 0;
        }
        currentMapId = homeMapId;
        currentMap = myWorld.getMap(currentMapId);
        MapSetup();
        myPlayer.SetActive(true);
        myPlayerScript.setCanMove(true);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void MapSetup()
    {
        mapHolder = new GameObject("CurrentMap");
        itemHolder = new GameObject("CurrentItems");
        npcHolder = new GameObject("CurrentNPCs");
        //gather necessary info
        bool isOutdoor = currentMap.getIsOutdoor();

        //todo build out the map
        currentMapXDim = currentMap.getXSize();
        currentMapYDim = currentMap.getYSize();
        for(int i=0; i<currentMapYDim; i++)
        {
            GameObject toInstantiate;
            Tile myTile = new Tile(); MapManager.tileTypes myTileType; int myTileFlavour, xLoc, yLoc, myExitId, myExitX, myExitY;
            for (int j=0; j<currentMapXDim; j++)
            {
                //get information about this tile
                myTile = currentMap.getTile(i,j);
                myTileType = myTile.getTileType();
                myTileFlavour = myTile.getMyFlavour();
                //choose the location
                xLoc = j*tileSideLength;
                yLoc = 0-i*tileSideLength;
                //choose the prefab
                switch (myTileType)
                {
                    case MapManager.tileTypes.FLOOR:
                        if (isOutdoor)
                        {
                            //decode which prefab to use
                            myTileFlavour = (myTileFlavour<0) ? 0 : myTileFlavour;
                            myTileFlavour = (myTileFlavour >= exteriorFloorTiles.Length) ? exteriorFloorTiles.Length - 1: myTileFlavour;
                            //instantiate the prefab
                            toInstantiate = exteriorFloorTiles[myTileFlavour];
                            GameObject nextInstancedTile = Instantiate(toInstantiate, new Vector3(xLoc, yLoc, 0f), Quaternion.identity) as GameObject;
                            //add the prefab to our lists
                            nextInstancedTile.transform.SetParent(mapHolder.transform);
                        }
                        else
                        {
                            myTileFlavour = (myTileFlavour < 0) ? 0 : myTileFlavour;
                            myTileFlavour = (myTileFlavour >= interiorFloorTiles.Length) ? interiorFloorTiles.Length - 1 : myTileFlavour;
                            toInstantiate = interiorFloorTiles[myTileFlavour];
                            GameObject nextInstancedTile = Instantiate(toInstantiate, new Vector3(xLoc, yLoc, 0f), Quaternion.identity) as GameObject;
                            nextInstancedTile.transform.SetParent(mapHolder.transform);
                        }
                        break;
                    case MapManager.tileTypes.WALL:
                        if (isOutdoor)
                        {
                            myTileFlavour = (myTileFlavour < 0) ? 0 : myTileFlavour;
                            myTileFlavour = (myTileFlavour >= exteriorWallTiles.Length) ? exteriorWallTiles.Length - 1 : myTileFlavour;
                            toInstantiate = exteriorWallTiles[myTileFlavour];
                            GameObject nextInstancedTile = Instantiate(toInstantiate, new Vector3(xLoc, yLoc, 0f), Quaternion.identity) as GameObject;
                            nextInstancedTile.transform.SetParent(mapHolder.transform);
                        }
                        else
                        {
                            myTileFlavour = (myTileFlavour < 0) ? 0 : myTileFlavour;
                            myTileFlavour = (myTileFlavour >= interiorWallTiles.Length) ? interiorWallTiles.Length - 1 : myTileFlavour;
                            toInstantiate = interiorWallTiles[myTileFlavour];
                            GameObject nextInstancedTile = Instantiate(toInstantiate, new Vector3(xLoc, yLoc, 0f), Quaternion.identity) as GameObject;
                            nextInstancedTile.transform.SetParent(mapHolder.transform);
                        }
                        break;
                    case MapManager.tileTypes.EXIT:
                        myExitId = myTile.getMyExitNextMapId();
                        myExitX = myTile.getMyExitNextX();
                        myExitY = myTile.getMyExitNextY();
                        if (isOutdoor)
                        {
                            myTileFlavour = (myTileFlavour < 0) ? 0 : myTileFlavour;
                            myTileFlavour = (myTileFlavour >= exteriorExitTiles.Length) ? exteriorExitTiles.Length - 1 : myTileFlavour;
                            toInstantiate = exteriorExitTiles[myTileFlavour];
                            GameObject nextInstancedTile = Instantiate(toInstantiate, new Vector3(xLoc, yLoc, 0f), Quaternion.identity) as GameObject;
                            nextInstancedTile.transform.SetParent(mapHolder.transform);
                            Exit myExit = nextInstancedTile.GetComponent<Exit>();
                            myExit.setParams(myExitId, myExitX, myExitY);
                        }
                        else
                        {
                            myTileFlavour = (myTileFlavour < 0) ? 0 : myTileFlavour;
                            myTileFlavour = (myTileFlavour >= interiorExitTiles.Length) ? interiorExitTiles.Length - 1 : myTileFlavour;
                            toInstantiate = interiorExitTiles[myTileFlavour];
                            GameObject nextInstancedTile = Instantiate(toInstantiate, new Vector3(xLoc, yLoc, 0f), Quaternion.identity) as GameObject;
                            nextInstancedTile.transform.SetParent(mapHolder.transform);
                            Exit myExit = nextInstancedTile.GetComponent<Exit>();
                            myExit.setParams(myExitId, myExitX, myExitY);
                        }
                        //set exit details
                        //add an exit-collider to the exit for the player to hit

                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!TODO URGENTLY!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        break;
                    case MapManager.tileTypes.INVALID:
                    default://for now we treat INVALID or other unknown types as a wall. consider changing later
                        if (isOutdoor)
                        {
                            myTileFlavour = (myTileFlavour < 0) ? 0 : myTileFlavour;
                            myTileFlavour = (myTileFlavour >= exteriorWallTiles.Length) ? exteriorWallTiles.Length - 1 : myTileFlavour;
                            toInstantiate = exteriorWallTiles[myTileFlavour];
                            GameObject nextInstancedTile = Instantiate(toInstantiate, new Vector3(xLoc, yLoc, 0f), Quaternion.identity) as GameObject;
                            nextInstancedTile.transform.SetParent(mapHolder.transform);
                        }
                        else
                        {
                            myTileFlavour = (myTileFlavour < 0) ? 0 : myTileFlavour;
                            myTileFlavour = (myTileFlavour >= interiorWallTiles.Length) ? interiorWallTiles.Length - 1 : myTileFlavour;
                            toInstantiate = interiorWallTiles[myTileFlavour];
                            GameObject nextInstancedTile = Instantiate(toInstantiate, new Vector3(xLoc, yLoc, 0f), Quaternion.identity) as GameObject;
                            nextInstancedTile.transform.SetParent(mapHolder.transform);
                        }
                        break;
                }
            }
        }
        //todo add the player
        int myNextXPos = (currentMapStartX*tileSideLength)+playerTileOffset;
        int myNextYPos = 0-playerTileOffset-(currentMapStartY*tileSideLength);
        myPlayer.transform.position = new Vector3(myNextXPos, myNextYPos, 0f);
        //todo spawn the NPCs

        //todo add in the items

        //todo set map rules
        /*
        MapManager.mapTypes myMapType;
        myMapType = currentMap.getMyMapType();
        switch(myMapType)
            case MapManager.mapTypes.HOUSE:

                break;
            case MapManager.mapTypes.FARM:

                break;
            case MapManager.mapTypes.RANCH:

                break;
            case MapManager.mapTypes.INVALID:

                break;
            }
        */
    }

    public void ChangeMaps(int incomingMapId) //todo
    {
        //TODO save all the parts of the outgoing map that we have to

        //TODO unload the current map
        Destroy(mapHolder);
        Destroy(itemHolder);
        Destroy(npcHolder);
        //load the new map
        currentMapId = incomingMapId;
        currentMap = myWorld.getMap(currentMapId);
        MapSetup(); //draw the new map
    }

    //my functions
    public XmlDocument getMapSaveOutput()
    {
        return mapSaveOutput;
    }

    public int getDateFromSave()//TODO complete
    {
        return 1;
    }

    public int getMoneyFromSave()//TODO complete
    {
        return 0;
    }

    private int initalMapConfigRead(bool loadSaveFile, int saveFileIndx) //TODO tidy comments returns the ID of the first map to load at game-start
    {
        //XmlTextReader myRdr = new XmlTextReader("./Configs/map_configs.xml");
        mapSaveOutput = new XmlDocument();
        string mapFilePath;
        if (!loadSaveFile) { mapFilePath = "./Assets/Configs/map_configs.xml"; } //TODO this path may need to change for the finished product
        else if (saveFileIndx == 0) { mapFilePath = "./Assets/Configs/map_configs.xml"; }//TODO update
        else if (saveFileIndx == 1) { mapFilePath = "./Assets/Configs/map_configs.xml"; }//TODO update
        else if (saveFileIndx == 2) { mapFilePath = "./Assets/Configs/map_configs.xml"; }//TODO update
        else { mapFilePath = "./Assets/Configs/map_configs.xml"; } //save file index was bad
        mapSaveOutput.Load(mapFilePath);

        //todo parse all the data
        XmlNode topWorld = mapSaveOutput.SelectSingleNode("/world");//we expect only one 'world' tag
        if (!topWorld.HasChildNodes) { Debug.Log("no world tag or it was empty");  return -1; }
        int homeNode;
        if (Int32.TryParse(topWorld.Attributes["initial-map"].Value, out homeNode) == false) { homeNode = -1; }//check homeNode is legit
        if (Int32.TryParse(topWorld.Attributes["initial-x-pos"].Value, out currentMapStartX) == false) { currentMapStartX = 1; }
        if (Int32.TryParse(topWorld.Attributes["initial-y-pos"].Value, out currentMapStartY) == false) { currentMapStartY = 1; }
        XmlNodeList mapIn = topWorld.SelectNodes("map");
        if (mapIn.Count < 1) { Debug.Log("no map tags in world tag"); return -1; }//todo tidy debug messages //todo decide on a more useful minimum map count (e.g. 3)
        /***************************************************************
        * PROCESS MAP INPUTS
        ***************************************************************/
        foreach(XmlNode node in mapIn)
        {
            bool myResult = myWorld.addMap(node);
            //if (!myResult) { Debug.Log("Error processing Map-ID " /*TODO ADD MAP-ID*/); }//TODO error-handling
        }
        /***************************************************************
        * PROCESS NPC INPUTS
        ***************************************************************/
        //todo
        /***************************************************************
        * PROCESS ITEM INPUTS
        ***************************************************************/
        //todo

        //TODO check home-node can be drawn, if not fill it with a basic map and write a trace message
        return homeNode; //TODO add error-handling

    }

    private class World
    {
        private readonly int MAX_MAPS = 64;
        private Map[] myMaps;

        public World()
        {
            myMaps = new Map[MAX_MAPS];
            for(int i=0; i< MAX_MAPS; i++) { myMaps[i] = null; }
        }

        public Map getMap(int requestedIndx)
        {
            if (requestedIndx<0 || requestedIndx>myMaps.Length) { return null; }
            else { return myMaps[requestedIndx]; }
        }

        public bool addMap(XmlNode mapIn)
        {
            bool addMapSuccess = false;
            bool mapParseSuccess = false;
            //process mapIn ID. if there's no ID, the ID is out of range, or if it's a duplicate ID, we disregard this map
            int mapInId;
            if(Int32.TryParse(mapIn.Attributes["map-id"].Value, out mapInId) == false) { return false; }
            if(mapInId < 0 || mapInId >= MAX_MAPS) { return false; }
            if(myMaps[mapInId] != null) { return false; }
            //if successful, process Map contents into Map object //TODO
            myMaps[mapInId] = new Map(mapIn);
            mapParseSuccess = myMaps[mapInId].getXmlCreationSuccess();
            //if successful, return true. else write debug log and return false //TODO
            if (!mapParseSuccess) { Debug.Log("Late failure processing Map: Map ID " + mapInId); myMaps[mapInId] = null; return false; } //undo any work we may have done before returning
            else return true;
        }

        public bool addNpc(XmlNode npcIn)
        {

            return false;
        }

        public bool addItem(XmlNode itemIn)
        {

            return false;
        }
    }

    private class Map
    {
        private readonly int MAX_MAP_DIM = 64;
        private int xDim, yDim;
        private bool isIndoor;
        private List<List<Tile>> myMap;

        private string myName;
        private MapManager.mapTypes myType;
        private bool myEnabled, myOutdoor, myOwned;
        
        private bool xmlCreationSuccess;
        private string xmlCreationFailureReason;

        public Map(XmlNode mapIn)
        {
            myMap = new List<List<Tile>>();
            xmlCreationSuccess = false; xmlCreationFailureReason = "none given";
            //get attributes
            myName = mapIn.Attributes["map-name"].Value;
            string mapTypeIn = mapIn.Attributes["map-type"].Value;
            myType = stringToMapType(mapTypeIn);
            if(myType == MapManager.mapTypes.INVALID) //if the map type is invalid, stop processing and kick out
            {
                myEnabled = false;
                myOutdoor = false;
                myOwned = false;
                xmlCreationFailureReason = "config file contained invalid/no map-type";
                return;
            }
            string mapIsEnabledIn = mapIn.Attributes["is-enabled"].Value;
            myEnabled = stringToBool(mapIsEnabledIn);
            string mapIsOutdoorsIn = mapIn.Attributes["is-outdoors"].Value;
            myOutdoor = stringToBool(mapIsOutdoorsIn);
            string mapIsPlayerOwnedIn = mapIn.Attributes["is-player-owned"].Value;
            myOutdoor = stringToBool(mapIsPlayerOwnedIn);
            //get map layout
            XmlNodeList rows = mapIn.SelectNodes("row");
            yDim = rows.Count; int xDimCount;
            List<Tile> myRow; Tile myTile;
            foreach (XmlNode row in rows)
            {
                myRow = new List<Tile>();
                xDimCount = 0;
                XmlNodeList tiles = row.SelectNodes("tile");
                xDimCount = tiles.Count;
                if(xDimCount > xDim) { xDim = xDimCount; }

                string myTypeInStr, myFlavourInStr;
                string myExitNextMapStr, myExitNextXPosStr, myExitNextYPosStr;
                MapManager.tileTypes myTileType;
                int myFlavour;
                int myExitMapId, myExitMapNextX, myExitMapNextY;
                foreach (XmlNode tile in tiles)
                {
                    //process tile type
                    if (tile.Attributes != null && tile.Attributes["type"] != null) { myTypeInStr = tile.Attributes["type"].Value; }
                    else { myTypeInStr = "invalid"; }
                    myTileType = stringToTileType(myTypeInStr);
                    //process tile flavour
                    if(tile.Attributes != null && tile.Attributes["flavour"] != null) { myFlavourInStr = tile.Attributes["flavour"].Value; }
                    else { myFlavourInStr = "0"; }
                    myFlavour = stringToInt(myFlavourInStr);
                    //process exit tile (if any)
                    XmlNode exit = tile.SelectSingleNode("exit");
                    if (myTileType==MapManager.tileTypes.EXIT && exit != null)
                    {
                        if(exit.Attributes != null && exit.Attributes["next-map-id"] != null){ myExitNextMapStr = exit.Attributes["next-map-id"].Value; }
                        else { myExitNextMapStr = "0"; }

                        if(exit.Attributes != null && exit.Attributes["next-x-pos"] != null) { myExitNextXPosStr = exit.Attributes["next-x-pos"].Value; }
                        else { myExitNextXPosStr = "0"; }

                        if(exit.Attributes != null && exit.Attributes["next-y-pos"] != null) { myExitNextYPosStr = exit.Attributes["next-y-pos"].Value; }
                        else { myExitNextYPosStr = "0"; }
                        myExitMapId = stringToInt(myExitNextMapStr);
                        myExitMapNextX = stringToInt(myExitNextXPosStr);
                        myExitMapNextY = stringToInt(myExitNextYPosStr);
                    }
                    else
                    {
                        myExitMapId = 0;
                        myExitMapNextX = 0;
                        myExitMapNextY = 0;
                    }
                    //finally, process this tile
                    myTile = new Tile(myTileType, myFlavour, myExitMapId, myExitMapNextX, myExitMapNextY);
                    myRow.Add(myTile);
                }
                myMap.Add(myRow);
            }
            xmlCreationSuccess = true;
        }

        private bool stringToBool(string myIn)
        {
            string tempStr = myIn.ToLower();
            if (string.Equals(tempStr, "true")) { return true; }
            else return false;
        }

        private int stringToInt(string myIn)
        {
            int myInt = -1;
            bool parseSuccess = Int32.TryParse(myIn, out myInt);
            return myInt;
        }

        private MapManager.mapTypes stringToMapType(string stringMyType)
        {
            string tempStr = stringMyType.ToLower();
            if (string.Equals(tempStr, "farm"))
            {
                return MapManager.mapTypes.FARM;
            }
            else if (string.Equals(tempStr, "house"))
            {
                return MapManager.mapTypes.HOUSE;
            }
            else
            {
                return MapManager.mapTypes.INVALID;
            }
        }

        private MapManager.tileTypes stringToTileType(string stringMyType)
        {
            string tempStr = stringMyType.ToLower();
            if (string.Equals(tempStr, "exit"))
            {
                return MapManager.tileTypes.EXIT;
            }
            else if (string.Equals(tempStr, "floor"))
            {
                return MapManager.tileTypes.FLOOR;
            }
            else if (string.Equals(tempStr, "wall"))
            {
                return MapManager.tileTypes.WALL;
            }
            else
            {
                return MapManager.tileTypes.INVALID;
            }
        }

        public bool getXmlCreationSuccess() { return xmlCreationSuccess; }

        public Tile getTile(int xAddrIn, int yAddrIn)
        {
            if ((xAddrIn < myMap[0].Count && xAddrIn >= 0) && (yAddrIn < myMap[1].Count && yAddrIn >= 0))
            {
                return myMap[xAddrIn][yAddrIn];
            }
            else return new Tile();
        }
        
        public int getXSize() { return xDim; }
        public int getYSize() { return yDim; }
        public bool getIsEnabled() { return myEnabled; }
        public bool getIsOutdoor() { return myOutdoor; }
        public bool getIsPlayerOwned() { return myOwned; }
        public MapManager.mapTypes getMyMapType() { return myType; }
    }

    private class Tile
    {
        private MapManager.tileTypes myTileType;
        private int myFlavour;
        private int exitId, exitTgtX, exitTgtY;

        public Tile()
        {
            exitId = 0;
            myTileType = MapManager.tileTypes.FLOOR;
            myFlavour = 0;
        }

        public Tile(MapManager.tileTypes tileTypeIn, int flavourIn, int exitIdIn, int exitTgtXIn, int exitTgtYIn)
        {
            myTileType = tileTypeIn;
            if(myTileType == MapManager.tileTypes.EXIT)
            {//most of this error handling needs to be done at the MapManager level
                exitId = (exitIdIn<0)? 0 : exitIdIn;
                exitTgtX = (exitTgtXIn<0 ) ? 0 : exitTgtXIn;
                exitTgtY = (exitTgtYIn<0) ? 0 : exitTgtYIn;
            }
            else { exitId = 0; exitTgtX = 0; exitTgtY = 0; }
            myFlavour = flavourIn;
        }

        public MapManager.tileTypes getTileType() { return myTileType; }
        public int getMyFlavour() { return myFlavour; }
        public int getMyExitNextMapId() { return exitId; }
        public int getMyExitNextX() { return exitTgtX; }
        public int getMyExitNextY() { return exitTgtY; }
    }
}
