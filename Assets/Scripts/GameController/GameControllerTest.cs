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
	}

	static void increaseScore(int s){
		score += s;
//		scoreText.text = " " + score;
	}

	void spawnMonster(){
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
}
