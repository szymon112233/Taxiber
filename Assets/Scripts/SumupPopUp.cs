using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SumupPopUp : MonoBehaviour {

    #region singleton
    public static SumupPopUp instance = null;      //Static instance of Globals which allows it to be accessed by any other script.

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


    public Text distanceText;
    public Text priceText;
    public Text waitingText;

    // Use this for initialization
    void Start () {
        Hide();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void Show (float distance, float price, float time)
    {
        distanceText.text = string.Format("Distance: {0}", distance);
        priceText.text = string.Format("Price: {0}", price);
        waitingText.text = string.Format("Waiting time: {0}", time);
        gameObject.SetActive(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
