using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class Cab
{
    public Cab(int id, bool occupied, Vector2 positon)
    {
        this.id = id;
        this.occupied = occupied;
        this.positon = positon;
    }

    public Cab(int id, bool occupied, float x, float y)
    {
        this.id = id;
        this.occupied = occupied;
        this.positon = new Vector2(x, y);
    }

    public int id;
    public bool occupied;
    public Vector2 positon;
    public CabController cabController = null;
}

public class CabManager : MonoBehaviour {

    #region singleton
    public static CabManager instance = null;      //Static instance of CabManager which allows it to be accessed by any other script.

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a CabManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

    }
    #endregion

    public static Dictionary<int, Cab> cabList;
    public GameObject cabPrefab;

    private void Start()
    {
        ServerCommunicator.EventCabsDownloaded += OnCabsDownloaded;
    }

    private void OnCabsDownloaded(string json)
    {
        DeserailzeCabs(json);
        SpawnCabs();
    }

    void DeserailzeCabs(string json)
    {
        cabList = new Dictionary<int, Cab>();
        JArray array = JArray.Parse(json);
        Debug.Log(array.ToString());
        foreach (JObject jobject in array.Children<JObject>())
        {
            int id = -1;
            bool occupied = false;
            float x = -1;
            float y = -1;
            foreach (JProperty p in jobject.Properties())
            {
                if (p.Name == "id")
                    id = int.Parse(p.Value.ToString());
                else if (p.Name == "occupied")
                    occupied = bool.Parse(p.Value.ToString());
                else if (p.Name == "x")
                    x = float.Parse(p.Value.ToString());
                else if (p.Name == "y")
                    y = float.Parse(p.Value.ToString());
            }
            cabList.Add(id, new Cab(id, occupied, x, y));
        }
    }

    public CabController SpawnNewCab(MapNode node)
    {
        Vector2 pos = new Vector2(node.position.x * Globals.instance.distanceMultiplier, node.position.y * Globals.instance.distanceMultiplier);
        cabList.Add(-1, new Cab(-1, true, node.position));
        GameObject go = Instantiate(cabPrefab, new Vector3(pos.x, pos.y), Quaternion.identity, gameObject.transform);
        cabList[-1].cabController = go.GetComponent<CabController>();

        return cabList[-1].cabController;

    }

    void SpawnCabs()
    {
        foreach (Cab cab in cabList.Values)
        {
            GameObject go = Instantiate(cabPrefab, new Vector3(cab.positon.x * Globals.instance.distanceMultiplier, cab.positon.y * Globals.instance.distanceMultiplier), Quaternion.identity, gameObject.transform);
            go.name = "Cab " + cab.id.ToString();
            cab.cabController = go.GetComponent<CabController>();
        }
    }

    public Cab GetCabOnNode(MapNode node)
    {
        foreach (Cab cab in cabList.Values)
        {
            if (cab.positon == node.position)
                return cab;
        }
        return null;
    }
}
