using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorScript : MonoBehaviour {

	private const float FLOOR_SIZE = 600f; // Pixel floor height

	public float floorSpeed; 	// Travel time between floors
	public int currFloor; 		// Current floor of the elevator, spawn floor
	public int destFloor; 		// Destination floor of the elevator
	public bool moving;			// Indicator, true if elevator is moving up
	public bool doorOpen;		// Indicator, true if elevator door is open

	ElevatorControllerScript elevatorController;		// The elevator controller script, stored in the gamecontroller
	public BoxCollider2D doorCollider;					// Elevator box collider to hit and request to open door


	// Use this for initialization
	void Start () {
		init ();	
	}
	
	// Update is called once per frame
	void Update () {
		isDoorClicked ();
	}

	void init(){
		floorSpeed = 1.2f;
		currFloor = 1;
		destFloor = currFloor;
		moving = false;
		doorOpen = false;

		elevatorController = GameObject.Find ("GameController").GetComponent<ElevatorControllerScript> ();
		doorCollider = gameObject.GetComponent<BoxCollider2D> ();
	}

	/* Check if door was clicked and requests to open if true */
	void isDoorClicked(){
		if (Input.touchCount > 0) {
			RaycastHit2D hit;
			Touch myTouch = Input.touches[0];
			if (myTouch.phase == TouchPhase.Began) {
				hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position), Vector2.zero);
				if (hit.collider == doorCollider) {
					requestOpenDoor ();
				}
			}
		}
	}

	/* The function that takes care of starting a move state and moving the elevator */
	public void goToFloor(int floorNr){
		Debug.Log ("I'm trying to move goddammit");
		if (!moving) {
			leavingFloor ();
		}
		destFloor = floorNr;
		/* floorNr is adjusted so calculations return correct y-coordinates,
		 * 1st floor is at 0 * FLOOR_SIZE, 2nd at 1 * FLOOR_SIZE and etc. */
		floorNr -= 1;
		float travelDistance = Mathf.Abs((transform.position.y) - ((floorNr * FLOOR_SIZE) + transform.parent.transform.position.y));
		float travelTime = travelDistance / FLOOR_SIZE * floorSpeed;
		float destPosY = floorNr * FLOOR_SIZE;
		Vector3 destPos = transform.position;
		destPos.y = transform.parent.transform.position.y + destPosY;

		// Check if floorNr position is higher then current elevator position
		if (floorNr * FLOOR_SIZE > transform.position.y) {
			// Useable for animation direction.
		}
		moving = true;
		iTween.MoveTo(gameObject, iTween.Hash("position", destPos, "easetype", iTween.EaseType.linear, "time", travelTime, "oncomplete", "arrivedAtFloor"));
	}

	/* Takes care of notifying the elevatorController that an elevator has left its floor
	* This is done in case there is another elevator at the floor that then opens. */
	void leavingFloor(){
		closeDoor ();
		elevatorController.elevatorLeftFloor (currFloor);
	}
		
	void arrivedAtFloor(){
		moving = false;
		currFloor = destFloor;
		// Notify the game controller that the elevator arrived at destination floor
		elevatorController.elevatorArrived(gameObject);
	}

	public void requestOpenDoor(){
		if (!moving) {
			elevatorController.closeOtherElevatorsAtFloor (gameObject, currFloor);
			openDoor ();
		}
	}

	public void openDoor(){
		doorOpen = true;
	}

	public void closeDoor(){
		doorOpen = false;
	}
}
