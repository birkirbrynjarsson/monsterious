﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour {

	// Random operator
	public static System.Random rand;

	// Elevator Controller
	private ElevatorControllerScript elevatorController;

	// -- MONSTER SPAWN -- Variables related to the spawning of monsters
	private const int MAX_MONSTERS = 24;
	private const int MAX_MONSTER_FLOOR = 4;
	private static float spawnSpeed;		// The time in seconds between every monsterspawn
	private static int totalMonsters;		// Total monsters that have been spawned and are on floors
	private static int[] monstersAtFloor;	// The amount of monsters at every floor
	private static readonly string[] monsterNames = {"MrMonster1", "MonsterMonroe1", "DrKhil1", "HulkiestHunk1"};
	private static int typesIntroduced;		// The number of monster types that have been introduced during gameplay

	// Variables related to the layout, structure of the game
	private const int FLOOR_AMOUNT = 6;
	public static List<Transform> floors;
	private static readonly float[] floorPosY = { -1840f, -1240f, -640f, -40f, 560f, 1160f };
	private static readonly float[] floorPosX = { 135f, 405f, 675f, 945f };
	private static readonly Dictionary<string, float> monsterPosY = new Dictionary<string, float> {
		{ "MrMonster1", 160f },
		{ "MonsterMonroe1", 240f },
		{ "DrKhil1", 170f },
		{ "HulkiestHunk1", 0f }
	};

	// Use this for initialization
	void Start () {
		init();
		initFloors ();
		initSpawn ();
		StartCoroutine (spawnMonster ());
	}

	// Initialize shared global variables here
	void init(){
		rand = new System.Random((int)System.DateTime.Now.Ticks & 0x0000FFFF);

		elevatorController = GetComponent<ElevatorControllerScript> ();
	}

	void initFloors(){
		// Iterate through the floors and add them to list
		GameObject floorParent = GameObject.Find ("Floors");
		floors = new List<Transform> ();
		foreach (Transform floor in floorParent.transform) {
			floors.Add (floor);
		}

	}

	// Initialize global variables for spawn here
	void initSpawn(){
		totalMonsters = 0;
		spawnSpeed = 7.0f;
		monstersAtFloor = new int[FLOOR_AMOUNT];
		for (var i = 0; i < monstersAtFloor.Length; i++) {
			monstersAtFloor [i] = 0;
		}
		typesIntroduced = 3;
	}
	
	// Update is called once per frame
	void Update () {
		monsterClicked ();
	}


	// ------------------------------------------------------------------------------
	//                     Monster Spawning and other monster functions 
	// ------------------------------------------------------------------------------


	/* Monster spawn function
	 */
	IEnumerator spawnMonster(){
		while (true) {
			if (totalMonsters < MAX_MONSTERS) {
				int floorIndex = rand.Next (FLOOR_AMOUNT);
				while (monstersAtFloor [floorIndex] >= MAX_MONSTER_FLOOR) {
					floorIndex = rand.Next (FLOOR_AMOUNT);
				}
				instantiateMonster (floorIndex);
			}
			yield return new WaitForSeconds (spawnSpeed);
		}
	}

	void instantiateMonster(int floorIndex){
		string monsterName = getRandomMonster ();
		GameObject monsterType = (GameObject)Resources.Load (monsterName);
		Vector2 spawnPosition = new Vector2(floorPosX[monstersAtFloor[floorIndex]], floorPosY[floorIndex] + monsterPosY[monsterName]);
		GameObject monster = Instantiate(monsterType, spawnPosition, Quaternion.identity);
		monster.transform.parent = floors[floorIndex];
		Monster monsterScript = monster.GetComponent<Monster>();
		monsterScript.currentFloor = floorIndex + 1;
		monsterScript.name = monsterName;
		totalMonsters++;
		monstersAtFloor[floorIndex]++;
	}

	string getRandomMonster(){
		// Needs probability calculations from Unnr
		return monsterNames [rand.Next (typesIntroduced)];
	}

	public void monsterLeft(int floorNr){
		int floorIndex = floorNr - 1;
		monstersAtFloor [floorIndex]--;
		totalMonsters--;
//		Stresser (LEAVE_PENALTY);
//		repositionMonstersAtFloor(floor);
	}

	// When a monster leaves every monsters shift left
	public void repositionMonstersAtFloor(Transform floor)
	{
		int i = 0;
		foreach (Transform child in floor.transform)
		{
			if (child.gameObject.tag == "Monster") {
				Vector3 pos = child.gameObject.transform.position;
				pos.x = floorPosX[i];
				child.gameObject.transform.position = pos;
				child.gameObject.GetComponent<Monster>().updatePos(pos);
				i++;
			}
		}
	}

	public void shakeFloor(int floorNr){
		// Shake shake, shake shake the floor
	}


	// ------------------------------------------------------------------------------
	//                              Monster Clicking
	// ------------------------------------------------------------------------------

	void monsterClicked()
	{
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			RaycastHit2D hit;
			hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
			if (hit.collider != null && (hit.transform.gameObject.tag == "Reset" || hit.transform.gameObject.tag == "MainMenu"))
			{
				Time.timeScale = 0;
//				GameObject.Find("Menu").transform.GetComponent<Canvas>().enabled = true;
//				GameObject.Find("MenuScore").GetComponent<Text>().text = "score " + score;
				return;
			}
			else if (hit.collider != null && hit.transform.gameObject.tag == "Monster")
			{
				Debug.Log ("Monster GOT CLikked BTICKKS");
				GameObject monster = hit.transform.gameObject;
				Transform floor = monster.transform.parent;
				int floorNr = -1;
				foreach (Transform f in floors)
				{
					if (f == floor)
					{
						floorNr = floors.IndexOf(f) + 1;
					}
				}
				Transform openElevator = elevatorController.getOpenElevatorAtFloor (floorNr);
				if (openElevator != null && floorNr != -1) {
					Monster monsterScript = monster.GetComponent<Monster>();
					if (openElevator.GetChild(0).childCount == 0)
					{
						if(monsterScript.name == "MonsterMonroe")
						{
//							ContinuePatience(floor);
						}

						monster.transform.parent = openElevator.GetChild(0).transform;
						monster.transform.position = new Vector2((openElevator.GetChild(0).transform.position.x), (openElevator.GetChild(0).transform.position.y + 0.06f));
//						monsterScript.patience.transform.position = new Vector2((openElevator.GetChild(0).transform.position.x) - 0.05f, (openElevator.GetChild(0).transform.position.y + 0.06f));
//						monsterScript.patienceScript.currentAmount = -1;
						totalMonsters--;
						repositionMonstersAtFloor(floor);
					}
					else if (openElevator.GetChild(1).childCount == 0)
					{
						if (monsterScript.name == "MonsterMonroe")
						{
//							ContinuePatience(floor);
						}

						monster.transform.parent = openElevator.GetChild(1).transform;
						monster.transform.position = new Vector2((openElevator.GetChild(1).transform.position.x), (openElevator.GetChild(1).transform.position.y + 0.06f));
//						monsterScript.patience.transform.position = new Vector2((openElevator.GetChild(1).transform.position.x) - 0.05f, (openElevator.GetChild(1).transform.position.y + 0.06f));
//						monsterScript.patienceScript.currentAmount = -1;
						totalMonsters--;
						repositionMonstersAtFloor(floor);
					}
					else
					{
						Debug.Log("Elevator is full");
					}
				}
			}
		}
	}
}
