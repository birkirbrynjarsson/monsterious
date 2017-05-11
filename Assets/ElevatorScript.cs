using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorScript : MonoBehaviour {

	private const float FLOOR_SIZE = 600f; // Pixel floor height

	public float floorSpeed; 	// Travel time between floors
	public int currFloor; 		// Current floor of the elevator, spawn floor
	public int destFloor; 		// Destination floor of the elevator
	public bool movingUp;		// Indicator, true if elevator is moving up
	public bool movingDown;		// Indicator, true if elevator is moving down
	public bool doorOpen;		// Indicator, true if elevator door is open


	// Use this for initialization
	void Start () {
			
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void init(){
	
	}
}
