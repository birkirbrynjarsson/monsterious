using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerTest : MonoBehaviour {

	private const float FLOOR_NUDGE = 0.36f;
	private const int MAX_MONSTER_FLOOR = 5;
	private const int MAX_MONSTERS = 35; // floors * monsters/floor
	private const float X_MONSTER_START = -0.18f;
	private const float X_MONSTER_WIDTH = 0.64f;

//	private static GUIText scoreText;
	private static int score;
	public static float spawnSpeed = 1.0f;
	private static float lastSpawn;
	private static List<float> floorPosY;
	private static List<Transform> floors;
	private static System.Random rand;
	private static List<float> floorPosX;
	private static int totalMonsters = 0;
	private static List<Transform> elevators;

	// Use this for initialization
	void Start () {
		// Initialize the list of floors and y and x Coordinates
		GameObject floorList = GameObject.Find ("Floors");
		floors = new List<Transform> ();
		foreach (Transform child in floorList.transform) {
			floors.Add (child);
		}
		floorPosY = new List<float> ();
		foreach (Transform child in floorList.transform) {
			floorPosY.Add (child.transform.position.y + FLOOR_NUDGE);
		}
		floorPosX = new List<float> ();
		for (int i = 0; i < MAX_MONSTER_FLOOR; i++) {
			if (i == 0) {
				floorPosX.Add (X_MONSTER_START);
			}
			else floorPosX.Add (floorPosX[i-1] + X_MONSTER_WIDTH);
		}
		GameObject el = GameObject.Find ("Elevators");
		elevators = new List<Transform> ();
		foreach (Transform child in el.transform) {
			elevators.Add (child);
		}
		rand = new System.Random ((int)Time.time);
		totalMonsters = 0;
		score = 0;
		spawnMonster ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - lastSpawn >= spawnSpeed) {
			spawnMonster ();
		}
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {
			RaycastHit2D hit;
			hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position), Vector2.zero);
			if (hit.collider != null && hit.transform.gameObject.tag == "Monster") {
				GameObject monster = hit.transform.gameObject;
				Transform floor = monster.transform.parent;
				int floorNr = -1;
				foreach (Transform f in floors) {
					if (f == floor) {
						floorNr = floors.IndexOf (f);
					}
				}
				if (isElevatorAtFloor (floorNr) && floorNr != -1) {
					Transform el = getOpenElevatorAtFloor (floorNr);
					if (el != null) {
						if (el.GetChild (0).childCount == 0) {
							monster.transform.parent = el.GetChild (0).transform;
							monster.transform.position = new Vector2((el.GetChild(0).transform.position.x), (el.GetChild(0).transform.position.y + 0.06f));
							totalMonsters--;
							repositionMonstersAtFloor(floor);
						} else if (el.GetChild (1).childCount == 0) {
							monster.transform.parent = el.GetChild (1).transform;
							monster.transform.position = new Vector2((el.GetChild(1).transform.position.x), (el.GetChild(1).transform.position.y + 0.06f));
							totalMonsters--;
							repositionMonstersAtFloor(floor);
						} else {
							Debug.Log ("Elevator is full");
						}
					}
				}
			}
		}
	}

	public void repositionMonstersAtFloor(Transform floor){
		int i = 0;
		foreach (Transform child in floor.transform) {
			Vector3 pos = child.gameObject.transform.position;
			pos.x = floorPosX [i];
			child.gameObject.transform.position = pos;
			i++;
		}
	}

	public bool isElevatorAtFloor(int floorNr){
		foreach (Transform el in elevators) {
			ElevatorTest elscript = el.gameObject.GetComponent<ElevatorTest> ();
			if (elscript.currFloor == floorNr && !elscript.movingDown && !elscript.movingUp) {
				return true;
			}
		}
		return false;
	}

	public Transform getOpenElevatorAtFloor(int floorNr){
		foreach (Transform el in elevators) {
			ElevatorTest elscript = el.gameObject.GetComponent<ElevatorTest> ();
			if (elscript.currFloor == floorNr && !elscript.movingDown && !elscript.movingUp && elscript.doorOpen){
				return el;
			}
		}
		return null;
	}

	static void increaseScore(int s){
		score += s;
//		scoreText.text = " " + score;
	}

	public void spawnMonster(){
		if (totalMonsters >= MAX_MONSTERS) {
			return;
		}
		Debug.Log ("We are spawning a monster BITCHES");
		lastSpawn = Time.time;
		// Monster to spawn
		GameObject green = (GameObject)Resources.Load ("greenMonster");
		// Floor to spawn on
		int floorIndex = rand.Next (0, floors.Count);
		GameObject floor = floors[floorIndex].gameObject;
		while (floor.transform.childCount >= MAX_MONSTER_FLOOR && totalMonsters < MAX_MONSTERS) {
			Debug.Log ("Floor is full");
			floorIndex = rand.Next (0, floors.Count);
			floor = floors[floorIndex].gameObject;
		}
		Debug.Log ("Floor Index: " + floorIndex);
		int posX = floor.transform.childCount;
		Instantiate (green, new Vector2 (floorPosX[posX], floorPosY[floorIndex]), Quaternion.identity).transform.parent = floor.transform;
		totalMonsters++;
	}

	public void elevatorArrived (GameObject elevator){
		int elFloor = elevator.GetComponent<ElevatorTest> ().currFloor;
		if (elevators != null) {
			foreach(Transform e in elevators){
				if (e.gameObject != elevator && e.GetComponent<ElevatorTest> ().currFloor == elFloor) {
					e.GetComponent<ElevatorTest> ().closeDoor ();
				}
			}
		}
		elevator.GetComponent<ElevatorTest> ().openDoor ();
		if (elevator.transform.GetChild (0).childCount > 0) {
			Destroy (elevator.transform.GetChild (0).GetChild (0).gameObject);
		}
		if (elevator.transform.GetChild (1).childCount > 0) {
			Destroy (elevator.transform.GetChild (1).GetChild (0).gameObject);
		}
	}

	public void elevatorDeparting(GameObject elevator){
		int elFloor = elevator.GetComponent<ElevatorTest> ().currFloor;
		foreach(Transform e in elevators){
			ElevatorTest elScript = e.GetComponent<ElevatorTest> ();
			if (e.gameObject != elevator && elScript.currFloor == elFloor && !elScript.movingDown && !elScript.movingUp) {
				e.GetComponent<ElevatorTest> ().arrivedAtFloor ();
				break;
			}
		}
	}
}
