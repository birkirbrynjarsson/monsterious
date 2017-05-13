using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBtnScript : MonoBehaviour {

	public int elevatorLane;		// Integer for elevator lane that was clicked
	public int elevatorFloor;		// Integer for elevator floor that was clicked

	private ElevatorControllerScript elevatorController;

	// Use this for initialization
	void Start () {
		elevatorLane = 0;
		elevatorFloor = 0;

		elevatorController = GameObject.Find ("GameController").GetComponent<ElevatorControllerScript> ();

		initBtns ();
	}
	
	// Update is called once per frame
	void Update () {
		if (checkIfBtnClicked ()) {
			elevatorController.moveElevator (elevatorLane, elevatorFloor);
			setActiveFloor (elevatorLane, elevatorFloor);
		}
	}

	void initBtns(){
		foreach (Transform lane in gameObject.transform) {
			foreach (Transform floor in lane.transform) {
				foreach (Transform btn in floor.transform) {
					btn.gameObject.SetActive(true);
				}
			}
		}
		foreach (Transform lane in gameObject.transform) {
			foreach (Transform floor in lane.transform) {
				floor.Find ("active").gameObject.GetComponent<Renderer> ().enabled = false;
			}
		}
	}

	bool checkIfBtnClicked(){
		if (Input.touchCount > 0) {
			RaycastHit2D hit;
			Touch myTouch = Input.touches[0];
			if (myTouch.phase == TouchPhase.Began) {
				hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position), Vector2.zero);

				elevatorLane = 0;

				foreach (Transform lane in gameObject.transform) {
					elevatorLane++;
					elevatorFloor = 0;
					foreach (Transform floor in lane.transform) {
						elevatorFloor++;
						if (hit.collider == floor.GetComponent<BoxCollider2D> ()) {
							Debug.Log ("FloorBtnClicked: " + elevatorLane + " " + elevatorFloor);
							return true;
						}
					}
				}

			}
		}
		return false;
	}

	void setActiveFloor(int laneNr, int floorNr){
		int currLane = 0;

		// Set all active indicators to inactive
		foreach (Transform lane in gameObject.transform) {
			currLane++;
			if (currLane == laneNr) {
				foreach (Transform floor in lane.transform) {
					floor.Find ("active").gameObject.GetComponent<Renderer> ().enabled = false;
				}
			}
		}

		currLane = 0;

		// set the correct floor indicator to active
		foreach (Transform lane in gameObject.transform) {
			currLane++;
			if (currLane == laneNr) {
				lane.Find (floorNr.ToString () + "/active").gameObject.GetComponent<Renderer> ().enabled = true;
			}
		}
	}
}
