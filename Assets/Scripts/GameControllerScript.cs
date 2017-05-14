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
	private const float MIN_SPAWN_SPEED = 7.0f;
	private static float spawnSpeed;		// The time in seconds between every monsterspawn
	private static int totalMonsters;		// Total monsters that have been spawned and are on floors
	private static int[] monstersAtFloor;	// The amount of monsters at every floor
	private static readonly string[] monsterNames = {"MrMonster1", "MonsterMonroe1", "DrKhil1", "HulkiestHunk1"};
	private static int typesIntroduced;		// The number of monster types that have been introduced during gameplay

	public bool wave;

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

	// Patience or Life metering
	public float totalDamage;
	public float accumulatedDamage;
	private const float LEAVE_PENALTY = 0.2f;
	private CloudCoverScript cloudCoverScript;

    // Coin variables
    private static int coins;
    public Text coinText;

    // Use this for initialization
    void Start () {
		init();
		initFloors ();
		initSpawn ();
		initLife ();
		StartCoroutine (spawnMonster ());
		StartCoroutine (waveScheduler ());
		StartCoroutine (moveClouds());
        updateScore();
        updateCoins();
	}

	// Initialize shared global variables here
	void init(){
		rand = new System.Random((int)System.DateTime.Now.Ticks & 0x0000FFFF);
        coins += score;
        score = 0;

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
		wave = false;
		totalMonsters = 0;
		spawnSpeed = MIN_SPAWN_SPEED;
		monstersAtFloor = new int[FLOOR_AMOUNT];
		for (var i = 0; i < monstersAtFloor.Length; i++) {
			monstersAtFloor [i] = 0;
		}
		typesIntroduced = 4;
	}

	void initLife(){
		accumulatedDamage = 0f;
		cloudCoverScript = GameObject.Find ("cloudCover").GetComponent<CloudCoverScript>();
		cloudCoverScript.move (80f);
	}
	
	// Update is called once per frame
	void Update () {
		monsterClicked ();
		life (); // Calculation of left monsters and their patience
	}


	// ------------------------------------------------------------------------------
	//                     Total patience life 
	// ------------------------------------------------------------------------------

	public void life(){
		regenerateLife ();
		totalDamage = getTotalFloorPatience () + accumulatedDamage;
		if (gameOver ()) {
            Time.timeScale = 0;
            GameObject.Find("GameOver").transform.GetComponent<Canvas>().enabled = true;
            GameObject.Find("GameOverScore").GetComponent<Text>().text = score + " points";
      
            return;
        }

	}

	public float getTotalFloorPatience(){
		float totalFloorPatience = 0.0f;
		foreach (Transform floor in floors)
		{
			foreach (Transform child in floor.transform)
			{
				if(child.gameObject.tag == "Monster"){
					totalFloorPatience += child.GetComponent<Monster>().getPatience();
				}
			}
		}
		return totalFloorPatience / ((MAX_MONSTERS - MAX_MONSTER_FLOOR) * 100.0f);
	}

	// Magic numbers, should be decleared as constants
	public void regenerateLife(){
		if (accumulatedDamage > 0.0 && totalMonsters < 6){
			accumulatedDamage -= Time.deltaTime / 80f;
		}
	}

	IEnumerator moveClouds(){
		while(true){
			cloudCoverScript.move(totalDamage);
			yield return new WaitForSeconds(0.2f);
		}
	}

	bool gameOver(){
		return cloudCoverScript.isMaxed ();
	}


	// ------------------------------------------------------------------------------
	//                     Monster Spawning and other monster functions 
	// ------------------------------------------------------------------------------


	IEnumerator waveScheduler(){
		float timeBetweenWaves = 50f;
		float waveLength = 20f;
		while (true) {
			yield return new WaitForSeconds (timeBetweenWaves);
			// Wave is started (High state)
			wave = true;
			spawnSpeed = 1.8f;
			yield return new WaitForSeconds (waveLength);
			wave = false;
			spawnSpeed = MIN_SPAWN_SPEED;
		}
	}

	IEnumerator regularSpawn(){
		float incrementSpeed = 14f;
		float maxSpawnSpeed = 2.1f;
		float spawnIncrement = 0.25f;
		while (true) {
			if (!wave) {
				if (spawnSpeed > maxSpawnSpeed) {
					spawnSpeed += spawnIncrement;
				}
			}
			yield return new WaitForSeconds (incrementSpeed);
		}
	}
        
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
		Debug.Log ("FloorIndex: " + floorIndex + ", MonstersAtFloor: " + monstersAtFloor[floorIndex] + ", floorPosX: " + floorPosX[monstersAtFloor[floorIndex]]);
		Vector2 spawnPosition = new Vector2(floorPosX[monstersAtFloor[floorIndex]], floorPosY[floorIndex] + monsterPosY[monsterName]);
		GameObject monster = Instantiate(monsterType, spawnPosition, Quaternion.identity);
		monster.transform.parent = floors[floorIndex];
		Monster monsterScript = monster.GetComponent<Monster>();
		monsterScript.currentFloor = floorIndex + 1;
		monsterScript.monsterName = monsterName;
		totalMonsters++;
		monstersAtFloor[floorIndex]++;
	}

	string getRandomMonster(){
        // Needs probability calculations from Unnr
        int randomIndex;
        rand = new System.Random((int)System.DateTime.Now.Ticks & 0x0000FFFF);
        if (rand.NextDouble() < 0.6)
        {
            return monsterNames[0];
        }
        else
        {
            rand = new System.Random((int)System.DateTime.Now.Ticks & 0x0000FFFF);
            randomIndex = rand.Next(typesIntroduced);
            randomIndex = 1;
            Debug.Log("Random index: " + randomIndex);
            return monsterNames[randomIndex];
        }
	}

	public void monsterLeft(int floorNr){
		int floorIndex = floorNr - 1;
		monstersAtFloor [floorIndex]--;
		totalMonsters--;
		accumulatedDamage += LEAVE_PENALTY;
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

    public void destroyMe(GameObject monster)
    {
        Destroy(monster);
    }

    // ------------------------------------------------------------------------------
    //                   Monster Abilites that changes the game
    // ------------------------------------------------------------------------------

    // Monster monroe makes everyone at her floor very patient
    public void patienceCalmer(Transform floor)
    {
        foreach (Transform monster in floor.transform)
        {
            if (monster.gameObject.tag == "Monster" && monster.GetComponent<Monster>().monsterName != monsterNames[1])
            {
                monster.GetComponent<Monster>().anim.SetInteger("State", 3);
                monster.GetComponent<Monster>().patienceScript.patienceStopper = true;
            }
        }
    }

    public void continuePatience(Transform floor)
    {
        foreach (Transform monster in floor.transform)
        {
            if (monster.gameObject.tag == "Monster")
            {
                Debug.Log("CONTINUE PATIENCE");
                monster.GetComponent<Monster>().patienceScript.patienceStopper = false;
                monster.GetComponent<Monster>().anim.SetInteger("State", 1);
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
				GameObject.Find("Menu").transform.GetComponent<Canvas>().enabled = true;
				//GameObject.Find("MenuScore").GetComponent<Text>().text = "score " + score;
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
						if(monsterScript.monsterName == monsterNames[1])
                        {
							continuePatience(floor);
						}
						monster.transform.parent = pos1.transform;
						monster.transform.position = new Vector2((pos1.transform.position.x), (pos1.transform.position.y) +  + monsterElevatorPosY[monsterScript.monsterName]);
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
						if (monsterScript.monsterName == monsterNames[1])
                        {
							continuePatience(floor);
						}
						monster.transform.parent = pos2.transform;
						monster.transform.position = new Vector2((pos2.transform.position.x), (pos2.transform.position.y) + monsterElevatorPosY[monsterScript.monsterName]);
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

    public void updateCoins()
    {
        coinText.text = " " + coins;
    }
}
