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

	// iTween parameters, global to use between functions
	Vector3 destPos; 
	float travelTime;

	ElevatorControllerScript elevatorController;		// The elevator controller script, stored in the gamecontroller
	public BoxCollider2D doorCollider;					// Elevator box collider to hit and request to open door

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
		floorSpeed = 1.2f;
		currFloor = 1;
		destFloor = currFloor;
		movingUp = false;
		movingDown = false;
		doorOpen = false;

		elevatorController = GameObject.Find ("GameController").GetComponent<ElevatorControllerScript> ();
		doorCollider = gameObject.GetComponent<BoxCollider2D> ();

		anim = GetComponent<Animator> ();

		rand = new System.Random((int)System.DateTime.Now.Ticks & 0x0000FFFF);
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
		if (!movingUp && !movingDown && !doorOpen) {
			elevatorController.closeOtherElevatorsAtFloor (gameObject, currFloor);
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
}
