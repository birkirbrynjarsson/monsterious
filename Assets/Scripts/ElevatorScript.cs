﻿using System.Collections;
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

	private List<Transform> elevatorRoom;	// List of monster rooms in elevator

	// iTween parameters, global to use between functions
	Vector3 destPos; 
	float travelTime;

	ElevatorControllerScript elevatorController;		// The elevator controller script, stored in the gamecontroller
    GameControllerScript gameController;                // The game controller script
	public BoxCollider2D doorCollider;					// Elevator box collider to hit and request to open door

	// Floor Indicator gameobjects
	private GameObject indicatorPos1;
	private GameObject indicatorPos2;

	public Animator anim;

	public System.Random rand; // Used for random data such as blinking eyes.


	// Use this for initialization
	void Start () {
		init ();
		StartCoroutine (blinkEyes());
	}
	
	// Update is called once per frame
	void Update () {
		isDoorClicked ();
	}

	void init(){
		floorSpeed = 0.7f;
		currFloor = 1;
		destFloor = currFloor;
		movingUp = false;
		movingDown = false;
		doorOpen = false;

		elevatorController = GameObject.Find ("GameController").GetComponent<ElevatorControllerScript> ();
        gameController = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        doorCollider = gameObject.GetComponent<BoxCollider2D> ();

		anim = GetComponent<Animator> ();

		rand = new System.Random((int)System.DateTime.Now.Ticks & 0x0000FFFF);

		// Add elevator rooms to list
		elevatorRoom = new List<Transform> ();
		elevatorRoom.Add (transform.GetChild (0));
		elevatorRoom.Add (transform.GetChild (1));

		// Add indicators to elevator
		indicatorPos1 = gameObject.transform.FindChild ("indicatorPos1").gameObject;
		indicatorPos2 = gameObject.transform.FindChild ("indicatorPos2").gameObject;

		triggerIndicators ();
	}

	/* Check if door was clicked and requests to open if true */
	void isDoorClicked(){
		if (Input.touchCount > 0) {
			RaycastHit2D hit;
			Touch myTouch = Input.touches [0];
			if (myTouch.phase == TouchPhase.Began) {
				hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position), Vector2.zero);
				if (hit.collider == doorCollider && !movingUp && !movingDown && !doorOpen) {
					requestOpenDoor ();
				}
			}
		}
	}

	IEnumerator blinkEyes(){
		while (true) {
			// close eyes
			gameObject.transform.Find ("RightEyeLid").GetComponent<Renderer> ().enabled = true;
			gameObject.transform.Find ("LeftEyeLid").GetComponent<Renderer> ().enabled = true;
			int random = rand.Next (2) + 1;
			float closedTime = 0.1f + random/4;
			yield return new WaitForSeconds(closedTime);
			// open eyes
			float openTime = rand.Next(5) + 1;
			gameObject.transform.Find ("RightEyeLid").GetComponent<Renderer> ().enabled = false;
			gameObject.transform.Find ("LeftEyeLid").GetComponent<Renderer> ().enabled = false;
			yield return new WaitForSeconds(openTime);
		}
	}

	/* The function that takes care of starting a move state and moving the elevator */
	public void goToFloor(int floorNr){
		destFloor = floorNr;
		if (!movingUp && !movingDown) {
			leavingFloor ();
		}
		/* floorNr is adjusted so calculations return correct y-coordinates,
		 * 1st floor is at 0 * FLOOR_SIZE, 2nd at 1 * FLOOR_SIZE and etc. */
		floorNr -= 1;
		float travelDistance = Mathf.Abs((transform.position.y) - ((floorNr * FLOOR_SIZE) + transform.parent.transform.position.y));
		travelTime = travelDistance / FLOOR_SIZE * floorSpeed;
		float destPosY = floorNr * FLOOR_SIZE;
		destPos = transform.position;
		destPos.y = transform.parent.transform.position.y + destPosY;

		// Check if floorNr position is higher then current elevator position
		if (((floorNr * FLOOR_SIZE) + transform.parent.transform.position.y) > transform.position.y) {
			if (movingDown) {
				iTween.Stop (gameObject);
				anim.SetTrigger ("goUp");
			} else if (movingUp) {
				startTween ();
			} else {
				anim.SetTrigger ("goUp");
			}
			movingUp = true;
			movingDown = false;
		} else {
			if (movingUp) {
				iTween.Stop (gameObject);
				anim.SetTrigger ("goDown");
			} else if (movingDown) {
				startTween ();
			} else {
				anim.SetTrigger ("goDown");
			}
			movingUp = false;
			movingDown = true;
		}
//		iTween.MoveTo(gameObject, iTween.Hash("position", destPos, "easetype", iTween.EaseType.linear, "time", travelTime, "oncomplete", "arrivedAtFloor"));
	}

	public void startTween(){
		iTween.MoveTo(gameObject, iTween.Hash("position", destPos, "easetype", iTween.EaseType.linear, "time", travelTime, "oncomplete", "arrivedAtFloor"));
	}

	/* Takes care of notifying the elevatorController that an elevator has left its floor
	* This is done in case there is another elevator at the floor that then opens. */
	void leavingFloor(){
		if (doorOpen) {
			closeDoor ();
		}
		elevatorController.elevatorLeftFloor (currFloor);
	}
		
	void arrivedAtFloor(){
		if (movingDown) {
			anim.SetTrigger ("arriveDown");
		} else {
			anim.SetTrigger ("arriveUp");
		}
		movingUp = false;
		movingDown = false;
		currFloor = destFloor;
		// Notify the game controller that the elevator arrived at destination floor
		elevatorController.elevatorArrived(gameObject);
	}


	public void requestOpenDoor(){
		if (!movingUp && !movingDown && !doorOpen && currFloor == destFloor) {
			elevatorController.closeAllElevatorsAtFloor (gameObject, currFloor);
			openDoor ();
		}
	}

	public void openDoor(){
		if (!doorOpen) {
			anim.SetTrigger ("openDoor");
		}
		doorOpen = true;
	}

	public void closeDoor(){
		if (doorOpen) {
			anim.SetTrigger ("closeDoor");
		}
		doorOpen = false;
	}

	// Gets called by animation start of open door and end of close door
	public void triggerMonsterVisibility(){
		notifyMonsterVisibility ();
		Transform pos1 = transform.GetChild (0);
		Transform pos2 = transform.GetChild (1);
		SpriteRenderer[] allChildren = pos1.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer child in allChildren) {
			child.enabled = doorOpen;
		}
		allChildren = pos2.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer child in allChildren) {
			child.enabled = doorOpen;
		}
	}

	public void notifyMonsterVisibility(){
		Transform pos1 = transform.GetChild (0);
		if (pos1.childCount > 0) {
			if (pos1.GetChild (0).gameObject.tag == "Monster") {
				pos1.GetChild (0).GetComponent<Monster> ().triggerPatienceVisibility (doorOpen);
			}
		}
		Transform pos2 = transform.GetChild (1);
		if (pos2.childCount > 0) {
			if (pos2.GetChild (0).gameObject.tag == "Monster") {
				pos2.GetChild (0).GetComponent<Monster> ().triggerPatienceVisibility (doorOpen);
			}
		}
	}

	// Let out monsters if they are arriving at desired floor
	public void letOutMonsters(){
		foreach (Transform pos in elevatorRoom) {
			if (pos.childCount > 0) {
				Transform monster = pos.GetChild (0);
				if (monster.gameObject.tag == "Monster") {
					Monster monsterScript = monster.GetComponent<Monster> ();
					if (monsterScript.desiredFloor == currFloor) {
						spawnStarParticles (monster);
						gameController.addScore (monsterScript.monsterName);
						Destroy (monsterScript.patience);
						Destroy (monster.gameObject);
					}
				}
			}
		}
	}

	public void spawnStarParticles(Transform monster){
		GameObject stars = (GameObject)Resources.Load ("Particles/StarParticles");
		Vector2 spawnPosition = new Vector2 (monster.position.x, monster.position.y);
		GameObject starParticles = Instantiate(stars, spawnPosition, Quaternion.identity);
		Destroy (starParticles, 1.0f);
	}

	// Should be triggered by animation
	public void triggerIndicators(){
		turnOffIndicators ();
		if(!doorOpen) {
			if (gameObject.transform.FindChild ("pos1").childCount > 0) {
				GameObject monster = gameObject.transform.FindChild ("pos1").transform.GetChild (0).gameObject;
				Monster mScript = monster.GetComponent<Monster> ();
				indicatorPos1.transform.GetChild (mScript.desiredFloor - 1).GetComponent<SpriteRenderer> ().enabled = true;
			} 
			if (gameObject.transform.FindChild ("pos2").childCount > 0) {
				GameObject monster = gameObject.transform.FindChild ("pos2").transform.GetChild (0).gameObject;
				Monster mScript = monster.GetComponent<Monster> ();
				indicatorPos2.transform.GetChild (mScript.desiredFloor - 1).GetComponent<SpriteRenderer> ().enabled = true;
			}
		}
	}

	// Should be triggered by animation
	public void turnOffIndicators(){
		foreach (Transform child in indicatorPos1.transform) {
			child.GetComponent<SpriteRenderer> ().enabled = false;
		}
		foreach (Transform child in indicatorPos2.transform) {
			child.GetComponent<SpriteRenderer> ().enabled = false;
		}
	}


	// Animation notification funtions
	void closingDoors(){
		doorOpen = false;
	}

	void openingDoors(){
		doorOpen = true;
	}

	void goingUp(){
		movingUp = true;
		movingDown = false;
		doorOpen = false;
	}

	void goingDown(){
		movingDown = true;
		movingUp = false;
		doorOpen = false;
	}
}