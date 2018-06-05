using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class ServerCommunicator : MonoBehaviour {

    #region singleton
    public static ServerCommunicator instance = null;      //Static instance of Globals which allows it to be accessed by any other script.

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

    public const string BasicSiteUrl = "http://127.0.0.1:8082/";

    public const string CreateMapUrl = "map/create";
    public const string GetMapNodesUrl = "map/nodes/?id=";
    public const string GetMapEdgesUrl = "map/edges/?id=";
    public const string GetRandomMapNodesUrl = "map/nodes/?id=";
    public const string GetRandomMapEdgesUrl = "map/edges/?id=";

    public const string CreatePersonUrl = "person/create";
    public const string GetPersonUrl = "person/?id=";

    public const string SetSettingsUrl = "settings/start";

    public const string GetCabsUrl = "taxi/?map_id=";
    public const string GetNearestCabUrl = "taxi/call/?node_id=";
    public const string GetPathUrl = "taxi/destination?from_id={0}&to_id={1}";
    //public const string FreeCabUrl = "taxi/free";
    public const string GetCabsOnNodeUrl = "taxi/node";
    //public const string GetFreeCabsUrl = "taxi/unoccupied";

    public static System.Action<string, string> EventMapDownloaded;
    public static System.Action<string> EventCabsDownloaded;
    public static System.Action<string> EventNearestCabFound;
    public static System.Action<string> EventPathFound;
    public static System.Action EventDataDownloadComplete;


    private string mapNodesJson = string.Empty;
    private string mapEdgesJson = string.Empty;
    private bool nodesReadyToDraw = false;
    private bool edgesReadyToDraw = false;
    private bool cabsDownloaded = false;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            GetMapFromServer(8);
    }

    void InitConnection()
    {

    }

    #region Settings

    void SendSettingsToServer()
    {

    }

    #endregion

    #region Map

    void SendMapToServer()
    {

    }

    public void GetRandomMapFromServer()
    {
        GetMapNodes(true);
        GetMapEdges(true);
        StartCoroutine(WaitForMapDownloadAndDraw());
    }

    public void GetMapFromServer(int id)
    {
        GetMapNodes(false, id);
        GetMapEdges(false, id);
        StartCoroutine(WaitForMapDownloadAndDraw());
    }

    void GetMapNodes(bool isRandom, int id = 8)
    {
        StartCoroutine(GetMapNodesCOR(isRandom, id));
    }

    void GetMapEdges(bool isRandom, int id = 8)
    {
        StartCoroutine(GetMapEdgesCOR(isRandom, id));
    }

    IEnumerator GetMapNodesCOR(bool isRandom, int id = 8)
    {
        string url = string.Empty;
        if (isRandom)
            url = BasicSiteUrl + GetRandomMapNodesUrl;
        else
            url = BasicSiteUrl + GetMapNodesUrl + id.ToString();
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            mapNodesJson = www.downloadHandler.text;
            Debug.Log(www.downloadHandler.text);
            nodesReadyToDraw = true;

        }
    }

    IEnumerator GetMapEdgesCOR(bool isRandom, int id = 8)
    {
        string url = string.Empty;
        if (isRandom)
            url = BasicSiteUrl + GetRandomMapEdgesUrl;
        else
            url = BasicSiteUrl + GetMapEdgesUrl + id.ToString();
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            mapEdgesJson = www.downloadHandler.text;
            Debug.Log(www.downloadHandler.text);
            edgesReadyToDraw = true;
        }
    }

    IEnumerator WaitForMapDownloadAndDraw()
    {
        yield return new WaitUntil(() => nodesReadyToDraw && edgesReadyToDraw);
        if (EventMapDownloaded != null)
            EventMapDownloaded(mapNodesJson, mapEdgesJson);
        if (EventDataDownloadComplete != null)
            EventDataDownloadComplete();
    }

    #endregion

    #region People

    void AddPersonToServer()
    {

    }

    void GetPersonFormServer()
    {

    }

    #endregion

    #region Cabs

    void SendFreeCabRequest()
    {

    }

    public void GetNearestCabFormServer(int node_id)
    {
        StartCoroutine(GetNearestCabCOR(node_id));
    }

    public void GetCabsFormServer(int map_id)
    {
        StartCoroutine(GetCabsCOR(map_id));
    }

    void GetCabsOnNode()
    {

    }

    public void GetPath(int node_id_from, int node_id_to)
    {
        StartCoroutine(GetPathCOR(node_id_from, node_id_to));
    }

    IEnumerator GetNearestCabCOR(int node_id)
    {
        string url = BasicSiteUrl + GetNearestCabUrl + node_id.ToString();
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            cabsDownloaded = true;
            if (EventNearestCabFound != null)
                EventNearestCabFound(www.downloadHandler.text);

        }
    }

    IEnumerator GetPathCOR(int node_id_from, int node_id_to)
    {
        string url = string.Format(BasicSiteUrl + GetPathUrl, node_id_from, node_id_to);
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            cabsDownloaded = true;
            if (EventPathFound != null)
                EventPathFound(www.downloadHandler.text);

        }
    }

    IEnumerator GetCabsCOR(int map_id)
    {
        string url = BasicSiteUrl + GetCabsUrl + map_id.ToString();
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            cabsDownloaded = true;
            if (EventCabsDownloaded != null)
                EventCabsDownloaded(www.downloadHandler.text);

        }
    }

        #endregion

    }
