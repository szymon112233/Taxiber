using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class MapNode
{
    public MapNode(int id, int number, float x, float y)
    {
        this.id = id;
        this.number = number;
        position = new Vector2(x, y);
    }

    public int id;
    public int number;
    public Vector2 position;
}

[System.Serializable]
public class MapEdge
{
    public MapEdge(int id, int fromNodeNumber, int toNodeNumber, float distance, float occupancy, float time)
    {
        this.id = id;
        this.fromNodeNumber = fromNodeNumber;
        this.toNodeNumber = toNodeNumber;
        this.distance = distance;
        this.occupancy = occupancy;
        this.time = time;
    }

    public int id;
    public int fromNodeNumber;
    public int toNodeNumber;
    public float distance;
    public float occupancy;
    public float time;
}


public class MapDrawer : MonoBehaviour {

    public GameObject mapNodePrefab;
    public GameObject mapEdgePrefab;

    public static Dictionary<int, MapNode> nodes;
    public static Dictionary<int, MapNode> nodesByID;
    public static List<MapEdge> edges;

   
	// Use this for initialization
	void Start () {
        ServerCommunicator.EventMapDownloaded += OnMapDownloaded;
    }

    void OnMapDownloaded(string nodes, string edges)
    {
        DeserailzeNodes(nodes);
        DeserailzeEdges(edges);
        DrawMap();
    }

    void DeserailzeNodes(string json)
    {
        nodes = new Dictionary<int, MapNode>();
        nodesByID = new Dictionary<int, MapNode>();
        JArray array = JArray.Parse(json);
        Debug.Log(array.ToString());
        foreach (JObject jobject in array.Children<JObject>())
        {
            int id = -1;
            int number = -1;
            float x = -1;
            float y = -1;
            foreach (JProperty p in jobject.Properties())
            {
                if (p.Name == "id")
                    id = int.Parse(p.Value.ToString());
                else if (p.Name == "number")
                    number = int.Parse(p.Value.ToString());
                else if (p.Name == "x")
                    x = float.Parse(p.Value.ToString());
                else if (p.Name == "y")
                    y = float.Parse(p.Value.ToString());
            }
            nodes.Add(number, new MapNode(id, number, x, y));
            nodesByID.Add(id, new MapNode(id, number, x, y));
        }

    }

    void DeserailzeEdges(string json)
    {
        edges = new List<MapEdge>();
        JArray array = JArray.Parse(json);
        Debug.Log(array.ToString());
        foreach (JObject jobject in array.Children<JObject>())
        {
            int id = -1;
            int fromNodeNumber = -1;
            int toNodeNumber = -1;
            float distance = -1;
            float occupancy = -1;
            float time = -1;
            foreach (JProperty p in jobject.Properties())
            {
                if (p.Name == "id")
                    id = int.Parse(p.Value.ToString());
                else if (p.Name == "fromNodeNumber")
                    fromNodeNumber = int.Parse(p.Value.ToString());
                else if (p.Name == "toNodeNumber")
                    toNodeNumber = int.Parse(p.Value.ToString());
                else if (p.Name == "distance")
                    distance = float.Parse(p.Value.ToString());
                else if (p.Name == "time")
                    time = float.Parse(p.Value.ToString());
                else if (p.Name == "occupancy")
                    occupancy = float.Parse(p.Value.ToString());
            }
            edges.Add(new MapEdge(id, fromNodeNumber, toNodeNumber, distance, occupancy, time));
        }

    }

    public void DrawMap()
    {
        foreach (MapNode node in nodes.Values)
        {
            GameObject go = Instantiate(mapNodePrefab, new Vector3(node.position.x * Globals.instance.distanceMultiplier, node.position.y * Globals.instance.distanceMultiplier, 0.0f),
                Quaternion.identity, transform);
            go.name = string.Format("Node: {0}", node.number);
            go.GetComponent<CallNode>().node_id = node.id;
        }

        foreach (MapEdge edge in edges)
        {
            Vector2 pos = nodes[edge.fromNodeNumber].position;
            Vector2 posTo = nodes[edge.toNodeNumber].position;
            GameObject go = Instantiate(mapEdgePrefab, new Vector3(pos.x * Globals.instance.distanceMultiplier, pos.y * Globals.instance.distanceMultiplier, 0.0f),
                Quaternion.identity, transform);
            go.name = string.Format("Edge: {0} [{1}<=>{2}]", edge.id, edge.fromNodeNumber, edge.toNodeNumber);
            LineRenderer lRenderer = go.GetComponent<LineRenderer>();
            lRenderer.positionCount = 2;
            lRenderer.SetPosition(0, new Vector3(pos.x * Globals.instance.distanceMultiplier, pos.y * Globals.instance.distanceMultiplier, 0.0f));
            lRenderer.SetPosition(1, new Vector3(posTo.x * Globals.instance.distanceMultiplier, posTo.y * Globals.instance.distanceMultiplier, 0.0f));

        }

    }
}
