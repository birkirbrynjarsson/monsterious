using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorTest : MonoBehaviour {

	const float FLOOR_NUDGE = 0.925f;

	public float speed;
	public BoxCollider2D colliderUp;
	public BoxCollider2D colliderDown;

	private Vector3 destPos;
	private List<float> floorPosY;

	public int currFloor = 0;
	public int destFloor = 0;
	private float arriveTime = .0f;
	private float destSpeed = .0f;
	private bool movingUp = false;
	private bool movingDown = false;
	private bool doorOpen = false;

	private GameObject door; 
	private GameObject upActive;
	private GameObject downActive;

	// Use this for initialization
	void Start () {
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
		iTween.Init (gameObject);
		openDoor ();
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

	void elevatorUp(){
		destPos.y = floorPosY [destFloor];
		iTween.EaseType easing = iTween.EaseType.easeInOutSine;
		// Check if arriveTime is later, means that the elevator is moving
		// Does not ease in to animation, so it doesn't look like it is starting over
		if (arriveTime > Time.time) {
			arriveTime += speed;
			easing = iTween.EaseType.easeOutSine;
		} else {
			closeDoor ();
			arriveTime = Time.time + speed;
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
			closeDoor ();
			arriveTime = Time.time + speed;
		}
		movingDown = true;
		downActive.gameObject.SetActive (true);
		destSpeed = arriveTime - Time.time;
		iTween.MoveTo(gameObject,iTween.Hash("position", destPos, "easetype", easing, "time", destSpeed, "oncomplete", "arrivedAtFloor"));
	}

	void arrivedAtFloor() {
		openDoor ();
		currFloor = destFloor;
		movingUp = false;
		movingDown = false;
		upActive.gameObject.SetActive (false);
		downActive.gameObject.SetActive (false);
		Debug.Log ("I just arrived at a floor bitch!");
	}

	void openDoor(){
		doorOpen = true;
		door.SetActive (false);

	}

	void closeDoor(){
		doorOpen = false;
		door.SetActive (true);
	}
}