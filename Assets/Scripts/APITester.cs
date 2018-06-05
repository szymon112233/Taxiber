using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



public class APITester : MonoBehaviour {

    string createPersonString = "http://127.0.0.1:8082/person/create";

    private string mapJson = string.Empty;
    private bool nodesReadyToDraw = false;
    private bool edgesReadyToDraw = false;


    public Dictionary<int, MapNode> nodes;
    public List<MapEdge> edges;
    public MapDrawer mapdrawer;
    public List<MapNode> testNodesPath;
    public Slider tsSlider = null;


    private void Start()
    {
        tsSlider.onValueChanged.AddListener(onTSSliderChanged);
    }

    public void ConnectToServer()
    {

    }

    public void CreateTestPerson()
    {
        StartCoroutine(CreatePerson(0, "Marian", "Super"));
    }

    IEnumerator CreatePerson(int id, string firstName, string lastName)
    {
        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);

        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.Indented;

            writer.WriteStartObject();
            writer.WritePropertyName("firstName");
            writer.WriteValue(firstName);
            writer.WritePropertyName("id");
            writer.WriteValue(id);
            writer.WritePropertyName("lastName");
            writer.WriteValue(lastName);
            writer.WriteEndObject();
        }

        Debug.LogFormat("Creating person: {0}", sb.ToString());

        WWW www;
        Hashtable postHeader = new Hashtable();
        postHeader.Add("Content-Type", "application/json");

        // convert json string to byte
        var formData = System.Text.Encoding.UTF8.GetBytes(sb.ToString());

        www = new WWW(createPersonString, formData, postHeader);
        yield return www;


        if (www.error != null)
        {
            Debug.Log("There was an error sending request: " + www.error);
        }
        else
        {
            Debug.Log("WWW Request: " + www.text);
        }




    }

	
	// Update is called once per frame
	void Update () {
    }

    public void onTSSliderChanged(float value)
    {
        Time.timeScale = value;
    }

}
