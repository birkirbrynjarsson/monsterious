using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorTest : MonoBehaviour {

	const float FLOOR_NUDGE = 0.925f;

	public float speed = 1.0f;
	public BoxCollider2D colliderUp;
	public BoxCollider2D colliderDown;

	public Vector3 destPos;
	public List<float> floorPosY;

	public int currFloor = 0;
    public int thisFloor = 0;
	public int destFloor = 0;
	public float arriveTime = .0f;
	public float destSpeed = .0f;
	public bool movingUp = false;
	public bool movingDown = false;
	public bool doorOpen = false;

	private GameObject door; 
	private GameObject upActive;
	private GameObject downActive;
	private GameControllerTest gameController;
	private GameObject lvl1Indicators;
	private GameObject lvl2Indicators;

	// Use this for initialization
	void Start () {
		gameController = GameObject.Find("GameController").GetComponent<GameControllerTest> ();
		floorPosY = new List<float>();
		GameObject floors = GameObject.Find ("Floors");
		foreach(Transform child in floors.transform) {
			floorPosY.Add(child.position.y + FLOOR_NUDGE);
		}
		destPos = gameObject.transform.position;
		destPos.y = floorPosY [currFloor];
		gameObject.transform.position = destPos;
		door = transform.Find ("door").gameObject; 
		upActive = transform.Find ("up-active").gameObject;
		downActive = transform.Find ("down-active").gameObject;
		upActive.SetActive (false);
		downActive.SetActive (false);
		destFloor = currFloor;
		lvl1Indicators = gameObject.transform.FindChild ("lvl1-Destinations").gameObject;
		lvl2Indicators = gameObject.transform.FindChild ("lvl2-Destinations").gameObject;
		foreach (Transform child in lvl1Indicators.transform) {
			child.gameObject.SetActive (true);
		}
		foreach (Transform child in lvl2Indicators.transform) {
			child.gameObject.SetActive (true);
		}
		iTween.Init (gameObject);
		if (gameObject.name == "Elevator1") {
			openDoor ();
		} else closeDoor ();

		disableFloorIndicator ();
	}
		
	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0) {
			RaycastHit2D hit;
			Touch myTouch = Input.touches[0];
			if (myTouch.phase == TouchPhase.Began) {
				hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position), Vector2.zero);
				if (hit.collider == colliderUp && !movingDown) {
					destFloor++;
					if (destFloor >= floorPosY.Count) {
						destFloor = floorPosY.Count - 1;
					} 
					else elevatorUp ();
				} else if (hit.collider == colliderDown && !movingUp) {
					destFloor--;
					if (destFloor < 0) {
						destFloor = 0;
					} 
					else elevatorDown ();
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.UpArrow) && !movingDown) {
			destFloor++;
			if (destFloor >= floorPosY.Count) {
				destFloor = floorPosY.Count - 1;
			} 
			else elevatorUp ();
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow) && !movingUp) {
			destFloor--;
			if (destFloor < 0) {
				destFloor = 0;
			} 
			else elevatorDown ();
		}

	}

	public void elevatorUp(){
		destPos.y = floorPosY [destFloor];
		iTween.EaseType easing = iTween.EaseType.easeInOutSine;
		// Check if arriveTime is later, means that the elevator is moving
		// Does not ease in to animation, so it doesn't look like it is starting over
		if (arriveTime > Time.time) {
			arriveTime += speed;
			easing = iTween.EaseType.easeOutSine;
		} else {
            gameController.removeBubble(gameObject);
            //gameController.addNumber(gameObject);
            closeDoor ();
			arriveTime = Time.time + speed;
			gameController.elevatorDeparting (gameObject);        
        }
		movingUp = true;
		upActive.gameObject.SetActive (true);
		destSpeed = arriveTime - Time.time;
		iTween.MoveTo(gameObject,iTween.Hash("position", destPos, "easetype", easing, "time", destSpeed, "oncomplete", "arrivedAtFloor"));

	}

	void elevatorDown(){			
		destPos.y = floorPosY [destFloor];
		iTween.EaseType easing = iTween.EaseType.easeInOutSine;
		// Check if elevator is moving
		if (arriveTime > Time.time) {
			arriveTime += speed;
			easing = iTween.EaseType.easeOutSine;
		} else {
            gameController.removeBubble(gameObject);
			checkFloorIndicator ();
            //gameController.addNumber(gameObject);
            closeDoor ();
			arriveTime = Time.time + speed;
			gameController.elevatorDeparting (gameObject);
		}
		movingDown = true;
		downActive.gameObject.SetActive (true);
		destSpeed = arriveTime - Time.time;
		iTween.MoveTo(gameObject,iTween.Hash("position", destPos, "easetype", easing, "time", destSpeed, "oncomplete", "arrivedAtFloor"));
	}

	public void arrivedAtFloor() {
		movingUp = false;
		movingDown = false;
		upActive.gameObject.SetActive (false);
		downActive.gameObject.SetActive (false);
		currFloor = destFloor;
		// Notify the Game Controller
        if (gameController != null)
        {
            gameController.elevatorArrived(gameObject);
        }
        Debug.Log("I just arrived at a floor bitch!");
    }

	public void openDoor(){
		doorOpen = true;
		disableFloorIndicator ();
		door.SetActive (false);
	}

	public void closeDoor(){
		doorOpen = false;
		door.SetActive (true);
		checkFloorIndicator ();
	}

	public void disableFloorIndicator(){
		foreach (Transform child in lvl1Indicators.transform) {
			child.gameObject.SetActive (false);
		}
		foreach (Transform child in lvl2Indicators.transform) {
			child.gameObject.SetActive (false);
		}
	}

	public void checkFloorIndicator(){
		if (gameObject.transform.FindChild ("lvl1").childCount > 0) {
			GameObject monster = gameObject.transform.FindChild ("lvl1").transform.GetChild (0).gameObject;
			Monster mScript = monster.GetComponent<Monster> ();
			activateFloorIndicator (1, mScript.desiredFloor - 1);
		} 
		if (gameObject.transform.FindChild ("lvl2").childCount > 0) {
			GameObject monster = gameObject.transform.FindChild ("lvl2").transform.GetChild (0).gameObject;
			Monster mScript = monster.GetComponent<Monster> ();
			activateFloorIndicator (2, mScript.desiredFloor - 1);
		}
	}

	public void activateFloorIndicator(int level, int floorNr){
		if (level == 1) {
			lvl1Indicators.transform.GetChild (floorNr).gameObject.SetActive (true);
		} else if (level == 2) {
			lvl2Indicators.transform.GetChild (floorNr).gameObject.SetActive (true);
		}
	}
}