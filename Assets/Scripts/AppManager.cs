using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class AppManager : MonoBehaviour {

    #region singleton
    public static AppManager instance = null;      //Static instance of Globals which allows it to be accessed by any other script.

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a Globals.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

    }
    #endregion

    public bool isDebugMode = false;
    public int defaultMapID = 8;

    public GameObject personPrefab;
    public Camera mainCamera;
    

    private GameObject currentPerson;
    bool dataDownloadComplete = false;
    bool firstNodeSelected = false;
    bool secondNodeSelected = false;

    List<MapNode> curretPath;
    float currentDistance = 0.0f;

    int firstNodeId = -1;
    int secondNodeId = -1;

    float moveSpeed = 5.0f;
    float zoomSpeed = 1.0f;
    float defaultZoom = 5.0f;
    float sizeFactor = 80.0f;


    // Use this for initialization
    void Start () {
        mainCamera = Camera.main;

		if (!isDebugMode)
        {
            ServerCommunicator.EventDataDownloadComplete += OnDataDownloadComplete;
            ServerCommunicator.EventPathFound += onPathFound;
            ServerCommunicator.instance.GetMapFromServer(defaultMapID);
            ServerCommunicator.instance.GetCabsFormServer(defaultMapID);
        }

	}

    public int SelectNode(int id)
    {
        if (!firstNodeSelected)
        {
            firstNodeId = id;
            firstNodeSelected = true;
            return 1;
        }
        else if (!secondNodeSelected)
        {
            secondNodeId = id;
            secondNodeSelected = true;
            return 2;
        }
        else
        {
            firstNodeId = -1;
            firstNodeSelected = false;
            secondNodeId = -1;
            secondNodeSelected = false;
            return -1;
        }
    }

    void CalculatePath()
    {
        if (firstNodeId != -1 && secondNodeId != -1)
        {
            ServerCommunicator.instance.GetPath(firstNodeId, secondNodeId);
        }
    }

    public void StartDriveSimulation()
    {
        if (CabManager.instance.GetCabOnNode(curretPath[0]) != null)
        {
            CabController controller = CabManager.instance.GetCabOnNode(curretPath[0]).cabController;
            controller.GoTo(curretPath, 2.0f);
        }
        else
        {
            Debug.LogError("Couldn't find any cab on that node!!!!");
        }
            
    }


    void OnDataDownloadComplete()
    {
        CreatePerson();
        CenterCamera();
    }

    void onPathFound(string json)
    {
        DeserializePath(json);
        SumupPopUp.instance.Show(currentDistance, 0.0f, 0.0f);

    }
	
    void CreatePerson()
    {
        int randomNodeNumber = Random.Range(1, MapDrawer.nodes.Count + 1);

        Vector3 pos = new Vector3(MapDrawer.nodes[randomNodeNumber].position.x * Globals.instance.distanceMultiplier, MapDrawer.nodes[randomNodeNumber].position.y * Globals.instance.distanceMultiplier);
        currentPerson = Instantiate(personPrefab, pos, Quaternion.identity);
    }

    void CenterCamera()
    {
        Vector3 pos = currentPerson.transform.position;
        pos.z = -10;
        mainCamera.gameObject.transform.position = pos;
        mainCamera.orthographicSize = defaultZoom;
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Mouse2))
        {
            Vector3 mouseVector = new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0.0f);
            mouseVector *= moveSpeed;
            mouseVector *= mainCamera.orthographicSize / sizeFactor;
            mainCamera.transform.position += mouseVector;
        }
        if (Input.mouseScrollDelta.y != 0.0f)
        {
            mainCamera.orthographicSize -= zoomSpeed * Input.mouseScrollDelta.y;
        }
        if (Input.GetKeyDown(KeyCode.C))
            CenterCamera();
        if (Input.GetKeyDown(KeyCode.P))
            CalculatePath();

    }


    void DeserializePath(string json)
    {
        curretPath = new List<MapNode>();
        JObject jobject = JObject.Parse(json);
        Debug.Log(jobject.ToString());
        foreach (JProperty jproperty in jobject.Properties())
        {
            if (jproperty.Name == "Route")
            {
                string[] nodes = jproperty.Value.ToString().Replace("[", string.Empty).Replace("]", string.Empty).Split(',');

                foreach (string number in nodes)
                {
                    Debug.Log(int.Parse(number));
                    
                    curretPath.Add(MapDrawer.nodesByID[int.Parse(number)]);
                }

            }
            else if (jproperty.Name == "distance")
                currentDistance = float.Parse(jproperty.Value.ToString());
        }
    }
}
