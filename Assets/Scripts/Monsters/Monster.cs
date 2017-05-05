using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;

public class Monster : MonoBehaviour {

    public int currentFloor;
    public int desiredFloor;
    public GameObject patience;
    public Patience patienceScript;
    private GameControllerTest gameScript;
    private static System.Random rand;

    // Use this for initialization
    void Start () {
        // Create/Instantiate Patience Bubble above the monster with the floor number
        GameObject patienceBubble = (GameObject)Resources.Load("PatienceBubble");
        GameObject canvas = GameObject.Find("PatienceSpawn");

        patience = Instantiate(patienceBubble, new Vector2(transform.position.x-0.05f, transform.position.y+0.68f), Quaternion.identity);
        patience.transform.SetParent(canvas.transform, true);

        // Get desired floor number from the patience bubble
        patienceScript = patience.GetComponent<Patience>();
        patienceScript.setDesiredFloor(desiredFloor);

        // Get access to gamecontroller
        gameScript = GameObject.Find("GameController").GetComponent<GameControllerTest>();
    }
	
	// Update is called once per frame
	void Update () {
        // Check if the patience bubble is 100% red! If it is then remove monster.
		if(patienceScript.currentAmount >= 100f)
        {
            Transform floor = transform.parent;
            gameObject.transform.SetParent(gameObject.transform.parent.transform.parent);
            Destroy(patience);
            Destroy(gameObject);
            gameScript.monsterLeft(floor);
        }
	}

    internal void updatePos(Vector2 pos)
    {
        Vector2 newPos = patience.transform.position;
        newPos.x = pos.x - 0.05f;
        patience.transform.position = newPos;
    }
    
    internal void setCurrentFloor(int curr)
    {
        currentFloor = curr;
        rand = new System.Random((int)System.DateTime.Now.Ticks & 0x0000FFFF);
        desiredFloor = rand.Next(1, 8);
        while(desiredFloor == currentFloor)
        {
            desiredFloor = rand.Next(0, 8);
        }      
        
      
    }
}
