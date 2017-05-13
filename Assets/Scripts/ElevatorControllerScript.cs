using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorControllerScript : MonoBehaviour {

	/* ElevatorController mainly manages the opening and closing
	 * of elevators at floors Making sure that there is only
	 * one elevator open at any floor.
	 * Elevetors notify the controller when they arrive and depart and
	 * when their doors are opened manually.
	 */

	private List<Transform> elevators;		// List of the 3x elevator gameObjects

	private GameControllerScript gameController;

	// Use this for initialization
	void Start () {
		Time.timeScale = 1;
		init ();	
	}
	
	// Update is called once per frame
	//	void Update () {}

	void init(){
		// Add all elevators to a list
		GameObject el = GameObject.Find ("Elevators");
		elevators = new List<Transform> ();
		foreach (Transform elevator in el.transform){
			elevators.Add (elevator);
		}
		elevators [elevators.Count-1].GetComponent<ElevatorScript> ().openDoor ();

		// Add the gameController for score handling and other stuff
		gameController = GetComponent<GameControllerScript>();
	}

	public void moveElevator(int elevNr, int floorNr){
		elevators [elevNr - 1].GetComponent<ElevatorScript> ().closeDoor ();
		elevators [elevNr - 1].GetComponent<ElevatorScript> ().goToFloor (floorNr);
	}

	/* Takes care of closing other elevators at the floor,
	 * when an elevator wants to open its door */
	public void closeOtherElevatorsAtFloor(GameObject elToOpen, int floorNr){
		Transform openElevator = getOpenElevatorAtFloor (floorNr);
		if (openElevator != null) {
			openElevator.gameObject.GetComponent<ElevatorScript> ().closeDoor ();
		}
	}

	/* Takes care of opening the elevator that just arrived if there is
	 * no other elevator at the floor */
	public void elevatorArrived(GameObject elevator){
		ElevatorScript e = elevator.GetComponent<ElevatorScript> ();
		Debug.Log("Elevetor arrived at floor number: " + e.currFloor);
		if (getOpenElevatorAtFloor (e.currFloor) == null) {
			e.openDoor ();
		}
		// letOutMonsters is triggered by open door animation
		// e.letOutMonsters ();
	}

	/* Takes care of opening another elevator if there is one at the floor
	* that the elevator is leaving from. */
	public void elevatorLeftFloor(int floorNr){
		if (getOpenElevatorAtFloor (floorNr) == null) {
			foreach (Transform elevator in elevators){
				ElevatorScript e = elevator.gameObject.GetComponent<ElevatorScript>();
				if (e.currFloor == floorNr && e.destFloor == floorNr && !e.movingUp && !e.movingDown && !e.doorOpen) {
					e.openDoor ();
					return;
				}
			}
		}
	}

	/* Find the elevator that is open at requested floor, return null if none. */
	public Transform getOpenElevatorAtFloor(int floorNr)
	{
		foreach (Transform elevator in elevators){
			ElevatorScript e = elevator.gameObject.GetComponent<ElevatorScript>();
			if (e.currFloor == floorNr && e.destFloor == floorNr && !e.movingUp && !e.movingDown && e.doorOpen) {
				return elevator;
			}
		}
		return null;
	}
}
