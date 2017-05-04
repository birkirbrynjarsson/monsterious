using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;

public class Monster : MonoBehaviour {

    public int currentFloor;
    public int desiredFloor;
    public GameObject patience;
    private Patience patienceScript;
    private GameControllerTest gameScript;

    // Use this for initialization
    void Start () {
        // Create/Instantiate Patience Bubble above the monster with the floor number
        GameObject patienceBubble = (GameObject)Resources.Load("PatienceBubble");
        GameObject canvas = GameObject.Find("PatienceSpawn");

        patience = Instantiate(patienceBubble, new Vector2(transform.position.x-0.05f, transform.position.y+0.68f), Quaternion.identity);
        patience.transform.SetParent(canvas.transform, true);

        // Get desired floor number from the patience bubble
        patienceScript = patience.GetComponent<Patience>();
        desiredFloor = patienceScript.desiredFloor;

        gameScript = GameObject.Find("GameController").GetComponent<GameControllerTest>();
    }
	
	// Update is called once per frame
	void Update () {
        // Check if the patience bubble is 100% red! If it is then remove monster.
		if(patienceScript.currentAmount >= 100f)
        {
            Transform floor = transform.parent;
			gameObject.transform.SetParent (gameObject.transform.parent.transform.parent); 
            Destroy(patience);
            Destroy(gameObject);
            gameScript.monsterLeft(floor);
        }
	}

	public float getPatience(){
		return patienceScript.currentAmount;
	}
}
