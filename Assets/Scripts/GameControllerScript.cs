using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
		{ "HulkiestHunk1", 140f }
	};
	private static readonly Dictionary<string, float> monsterElevatorPosY = new Dictionary<string, float> {
		{ "MrMonster1", 145f },
		{ "MonsterMonroe1", 190f },
		{ "DrKhil1", 160f },
		{ "HulkiestHunk1", 132f }
	};

    // Score variables
    public Text scoreText;
    public int scoreValue;
    private static int score;
    public bool scoreOn = true;

    // Use this for initialization
    void Start () {
		init();
		initFloors ();
		initSpawn ();
		StartCoroutine (spawnMonster ());
        updateScore();
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
		typesIntroduced = 4;
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
				floorIndex = 0;
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
		Debug.Log ("FloorIndex: " + floorIndex + ", MonstersAtFloor: " + monstersAtFloor[floorIndex] + ", floorPosX: " + floorPosX[monstersAtFloor[floorIndex]]);
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
		repositionMonstersAtFloor (floors [floorIndex]);
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
				GameObject monster = hit.transform.gameObject;
				Transform floor = monster.transform.parent;
				Monster monsterScript = monster.GetComponent<Monster>();
				int floorNr = monsterScript.currentFloor;
				int floorIndex = floorNr - 1;
				Transform openElevator = elevatorController.getOpenElevatorAtFloor (floorNr);
				if (openElevator != null) {
					Transform pos1 = openElevator.GetChild (0);
					Transform pos2 = openElevator.GetChild (1);
					// If empty room at pos1 in elevator
					if (pos1.childCount == 0)
					{
						if(monsterScript.name == "MonsterMonroe"){
//							ContinuePatience(floor);
						}
						monster.transform.parent = pos1.transform;
						monster.transform.position = new Vector2((pos1.transform.position.x), (pos1.transform.position.y) +  + monsterElevatorPosY[monsterScript.name]);
						Vector3 scale = monster.transform.localScale;
						scale.x *= 0.6f;
						scale.y *= 0.6f;
						monster.transform.localScale = scale;
						Destroy (monster.GetComponent<BoxCollider2D> ());
                        //						monsterScript.patience.transform.position = new Vector2((openElevator.GetChild(0).transform.position.x) - 0.05f, (openElevator.GetChild(0).transform.position.y + 0.06f));
                        //						monsterScript.patienceScript.currentAmount = -1;
                        monsterScript.monsterInsideElevator(openElevator, pos1);
						monstersAtFloor[floorIndex]--;
						totalMonsters--;
						repositionMonstersAtFloor(floor);
					}
					// If empty room at pos2 in elevator
					else if (pos2.childCount == 0)
					{
						if (monsterScript.name == "MonsterMonroe"){
//							ContinuePatience(floor);
						}
						monster.transform.parent = pos2.transform;
						monster.transform.position = new Vector2((pos2.transform.position.x), (pos2.transform.position.y) + monsterElevatorPosY[monsterScript.name]);
						Vector3 scale = monster.transform.localScale;
						scale.x *= 0.6f;
						scale.y *= 0.6f;
						monster.transform.localScale = scale;
						SpriteRenderer[] allChildren = monster.GetComponentsInChildren<SpriteRenderer>();
						foreach (SpriteRenderer child in allChildren) {
							child.sortingOrder += 10;
						}
						Destroy (monster.GetComponent<BoxCollider2D> ());
                        //						monsterScript.patience.transform.position = new Vector2((openElevator.GetChild(1).transform.position.x) - 0.05f, (openElevator.GetChild(1).transform.position.y + 0.06f));
                        //						monsterScript.patienceScript.currentAmount = -1;
                        monsterScript.monsterInsideElevator(openElevator, pos2);
                        monstersAtFloor[floorIndex]--;
						totalMonsters--;
						repositionMonstersAtFloor(floor);
					}
				}
			}
		}
	}

    // ------------------------------------------------------------------------------
    //                                 Score
    // ------------------------------------------------------------------------------

    public void addScore(string monsterName)
    {
        if (monsterName == monsterNames[0])
        {
            score += 10;
        }
        else 
        {
            score += 30;
        }
        updateScore();
    }

    void updateScore()
    {
        scoreText.text = "Score " + score;
        if (scoreOn)
        {
            GetComponent<AudioSource>().Play();
        }
    }
}
