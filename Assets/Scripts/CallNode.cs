using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallNode : MonoBehaviour {

    public int node_id;

    public void SelectNode()
    {
        AppManager.instance.SelectNode(node_id);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
