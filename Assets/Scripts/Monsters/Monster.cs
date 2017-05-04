using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Monster : MonoBehaviour {

    public int floor;
    public GameObject patience;

	// Use this for initialization
	void Start () {
        //TODO: Create/Instantiate Patience Bubble above the monster with the floor number
        GameObject patienceBubble = (GameObject)Resources.Load("PatienceBubble");
        GameObject canvas = GameObject.Find("PatienceSpawn");

        patience = Instantiate(patienceBubble, new Vector2(transform.position.x-0.05f, transform.position.y+0.68f), Quaternion.identity);
        patience.transform.SetParent(canvas.transform, true);
        //TODO: Change size of the patience bubble

        //TODO: Get floor number from the patience bubble
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
