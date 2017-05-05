using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameControllerTest : MonoBehaviour {

	private const float FLOOR_NUDGE = 0.36f;
	private const int MAX_MONSTER_FLOOR = 5;
	private const int MAX_MONSTERS = 35; // floors * monsters/floor
	private const float X_MONSTER_START = -0.18f;
	private const float X_MONSTER_WIDTH = 0.64f;
	private const float LEAVE_PENALTY = 0.2f;

//	private static GUIText scoreText;
	private static int score;
	public static float spawnSpeed = 7.0f;
	private static float lastSpawn;
	private static float incrementSpeed = 10f;
	private static float spawnIncrement = 0.5f;
	private static float lastIncrement;
	private static List<float> floorPosY;
	private static List<Transform> floors;
	private static System.Random rand;
	private static List<float> floorPosX;
	private static int totalMonsters = 0;
	private static List<Transform> elevators;
	private static float floorStress;
	private static float otherStress;

    public Scrollbar StressBar;
    public float Stress = 100;
    public Text scoreText;
    public int scoreValue;
    Vector2 touchPos;
    public GraphicRaycaster GR;

    void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Score " + score;
        GetComponent<AudioSource>().Play();
    }

	void calculateFloorStress(){
		floorStress = 0.0f;
		foreach (Transform f in floors) {
			foreach (Transform m in f) {
				floorStress += m.GetComponent<Monster> ().getPatience();
			}
		}
		floorStress = floorStress / ((MAX_MONSTERS - MAX_MONSTER_FLOOR) * 100.0f);
	}

    void Stresser(float value){
        otherStress += value;
    }

	void calculateDisplayStress (){
		if (otherStress > 0.0) {
			otherStress -= Time.deltaTime / 20f;
		}
		Stress = floorStress + otherStress;
		StressBar.size = 1 - Stress;
	}

    // Use this for initialization
    void Start () {
		score = 0;
		totalMonsters = 0;
		spawnSpeed = 7.0f;
		lastSpawn = Time.time;
		incrementSpeed = 10f;
		spawnIncrement = 0.5f;
		lastIncrement = Time.time;
		Stress = 0f;
		floorStress = 0f;
		otherStress = 0f;
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
		rand = new System.Random ((int)System.DateTime.Now.Ticks & 0x0000FFFF);
        UpdateScore ();
        spawnMonster ();
        Time.timeScale = 1;
    }

	// Update is called once per frame
	void Update () {
        if (Stress >= 1f)
        {
            Time.timeScale = 0;
            GameObject.Find("GameOver").transform.GetComponent<Canvas>().enabled = true;
            return;
        }
        // TotalStress algorithm
        calculateFloorStress();
		calculateDisplayStress ();
		if (Time.time - lastIncrement >= incrementSpeed) {
			increaseSpawnSpeed ();
		}
		if (Time.time - lastSpawn >= spawnSpeed) {
			spawnMonster ();
		}
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {
            RaycastHit2D hit;
			hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position), Vector2.zero);
            if (hit.collider != null && hit.transform.gameObject.tag == "Reset")
            {
                Time.timeScale = 0;
                GameObject.Find("Menu").transform.GetComponent<Canvas>().enabled = true;
                return;
            }
            else if (hit.collider != null && hit.transform.gameObject.tag == "Monster") {
				GameObject monster = hit.transform.gameObject;
				Transform floor = monster.transform.parent;
				int floorNr = -1;
				foreach (Transform f in floors) {
					if (f == floor) {
						floorNr = floors.IndexOf (f);
					}
				}
				if (isElevatorAtFloor (floorNr) && floorNr != -1) {
                    Monster monsterScript = monster.GetComponent<Monster>();
					Transform el = getOpenElevatorAtFloor (floorNr);
					if (el != null) {
						if (el.GetChild (0).childCount == 0) {
							monster.transform.parent = el.GetChild (0).transform;
							monster.transform.position = new Vector2((el.GetChild(0).transform.position.x), (el.GetChild(0).transform.position.y + 0.06f));
                            monsterScript.patience.transform.position = new Vector2((el.GetChild(0).transform.position.x) - 0.05f, (el.GetChild(0).transform.position.y + 0.06f));
                            monsterScript.patienceScript.currentAmount = -1;
                            totalMonsters--;
							repositionMonstersAtFloor(floor);
                            //Stresser(scoreValue);
                            //AddScore(scoreValue);
                        } else if (el.GetChild (1).childCount == 0) {
							monster.transform.parent = el.GetChild (1).transform;
							monster.transform.position = new Vector2((el.GetChild(1).transform.position.x), (el.GetChild(1).transform.position.y + 0.06f));
                            monsterScript.patience.transform.position = new Vector2((el.GetChild(1).transform.position.x) - 0.05f, (el.GetChild(1).transform.position.y + 0.06f));
                            monsterScript.patienceScript.currentAmount = -1;
                            totalMonsters--;
							repositionMonstersAtFloor(floor);
                            //Stresser(scoreValue);
                            //AddScore(scoreValue);
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
            child.gameObject.GetComponent<Monster>().updatePos(pos);
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
        GameObject monster = Instantiate(green, new Vector2(floorPosX[posX], floorPosY[floorIndex]), Quaternion.identity);
        monster.transform.parent = floor.transform;

        // Set current floor in Monster to get desired random number
        Monster monsterScript = monster.GetComponent<Monster>();
        int currFloor = floorIndex + 1;
        monsterScript.setCurrentFloor(currFloor);
        totalMonsters++;
	}

	public void elevatorArrived (GameObject elevator){
		int elFloor = elevator.GetComponent<ElevatorTest> ().currFloor;
		if (elevators != null) {
			foreach(Transform e in elevators){
				if (e != null && e.gameObject != elevator && e.GetComponent<ElevatorTest> ().currFloor == elFloor) {
					e.GetComponent<ElevatorTest> ().closeDoor ();
				}
			}
		}

		elevator.GetComponent<ElevatorTest> ().openDoor ();
        
        if (elevator.transform.GetChild (0).childCount > 0) {
            GameObject monster1 = elevator.transform.GetChild(0).GetChild(0).gameObject;
            Monster monster1Script = monster1.GetComponent<Monster>();
            if (monster1Script.desiredFloor == (elFloor+1))
            {
                Destroy(monster1);
                AddScore(scoreValue);
            }
        }
		if (elevator.transform.GetChild (1).childCount > 0) {
            GameObject monster2 = elevator.transform.GetChild(1).GetChild(0).gameObject;
            Monster monster2Script = monster2.GetComponent<Monster>();
            if(monster2Script.desiredFloor == (elFloor+1))
            {
                Destroy(monster2);
                AddScore(scoreValue);
            }  
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

    public void monsterLeft(Transform floor){
        totalMonsters--;
		Stresser (LEAVE_PENALTY);
        repositionMonstersAtFloor(floor);
    }

    public void removeBubble(GameObject elev)
    {
        if (elev.transform.GetChild(0).childCount > 0)
        {
            GameObject monster1 = elev.transform.GetChild(0).GetChild(0).gameObject;
            Monster monster1Script = monster1.GetComponent<Monster>();
            Destroy(monster1Script.patience); 
            //if
            /*
            GameObject floorPic = (GameObject)Resources.Load(monster1Script.desiredFloor.ToString());
            GameObject floorNumber = Instantiate(floorPic, new Vector2(elev.transform.position.x, elev.transform.position.y), Quaternion.identity);
            floorNumber.transform.parent = elev.transform;
            */
        }
        if (elev.transform.GetChild(1).childCount > 0)
        {
            GameObject monster2 = elev.transform.GetChild(1).GetChild(0).gameObject;
            Monster monster2Script = monster2.GetComponent<Monster>();
            Destroy(monster2Script.patience);

            //if
            /*
            GameObject floorPic = (GameObject)Resources.Load(monster2Script.desiredFloor.ToString());
            GameObject floorNumber = Instantiate(floorPic, new Vector2(elev.transform.position.x, elev.transform.position.y), Quaternion.identity);
            floorNumber.transform.parent = elev.transform;
            */

        }
    }

	public void increaseSpawnSpeed(){
		if (spawnSpeed >= 2.0f) {
			spawnSpeed -= spawnIncrement;
		}
		lastIncrement = Time.time;
	}
}
