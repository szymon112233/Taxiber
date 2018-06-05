using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabController : MonoBehaviour {


    public void GoTo(MapNode node, float time = 5.0f)
    {
        LeanTween.move(gameObject, new Vector2(node.position.x * Globals.instance.distanceMultiplier, node.position.y * Globals.instance.distanceMultiplier), time);
    }

   public void GoTo(List<MapNode> nodes, float timeMultiplaier = 1.0f)
    {
        StartCoroutine(GoAlongPath(nodes, timeMultiplaier));
    }

    IEnumerator GoAlongPath(List<MapNode> nodes, float timeMultiplaier)
    {
        nodes.RemoveAt(0);
        foreach (MapNode node in nodes)
        {
            LTDescr ltdesc = LeanTween.move(gameObject,
            new Vector2(node.position.x * Globals.instance.distanceMultiplier, node.position.y * Globals.instance.distanceMultiplier),
            timeMultiplaier);
            bool completed = false;
            ltdesc.setOnComplete(() => { completed = true; });
            yield return new WaitUntil(() => { return completed; });
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.G))
            GotoTest();
	}


    void GotoTest()
    {
        GoTo(new MapNode(20, 30, 35, 299));
    }

}
