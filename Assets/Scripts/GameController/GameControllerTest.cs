using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameControllerTest : MonoBehaviour {

	private const float FLOOR_NUDGE = 0.36f;
	private const int MAX_MONSTER_FLOOR = 4;
	private const int MAX_MONSTERS = 24; // floors * monsters/floor
	private const float X_MONSTER_START = 0.2f;
	private const float X_MONSTER_WIDTH = 0.64f;
	private const float LEAVE_PENALTY = 0.2f;

    // -- Monster spawn variables --
    public List<string> monsterNames;                   // List of names of the monsters that have been introduced
    private static int totalMonsters = 0;               // The total number of monsters on the floors

    public static float spawnSpeed = 7.0f;              // How many seconds between monster spawn
    private static float maximumSpawnSpeed = 2.1f;      // Minimum seconds between monster spawn
    private static float lastSpawn;                     // Last time monster spawn
	private static float incrementSpeed = 10f;          // How many seconds between next incrementation of speed
	private static float spawnIncrement = 0.5f;         // How much the spawnSpeed increments when incrementing
	private static float lastIncrement;                 // Last time we incremented spawnspeed

    // -- The WAVE --
    private static float regularStateTime;              // How long the regular state in the wave should last
    private static float highStateTime;                 // How long the HIGH state in the wave should last
    private static float sinceLastState;                // How long the state has last
    private static float regularStateIncrement;         // How much the regular state should increment when incrementing
    private static int waveState;                       // State status: 0 for regular and 1 for high
    private static float highSpawnSpeed;                // How many seconds between monster spawn in high state

    private static System.Random rand;                  // Used for generating a random floor number
    private static  System.Random randomMons;
    //Animator monsterAnim;
    private bool monsterAdding;
    private int monsterTypeCount;


    private static List<float> floorPosY;
	private static List<Transform> floors;
	private static List<float> floorPosX;
	private static List<Transform> elevators;
	private static float floorStress;
	private static float otherStress;
    Animator gameOver;
    private static int score;
    public bool scoreOn = true;
    //	private static GUIText scoreText;

    public Scrollbar StressBar;
    public float Stress = 100;
    public Text scoreText;
    public int scoreValue;
    Vector2 touchPos;
    public GraphicRaycaster GR;

    // ------------------------------------------------------------------------------
    //                                   START
    // ------------------------------------------------------------------------------
    void Start () {

        // --- Spawn settings ---
        score = 0;
		totalMonsters = 0;
		spawnSpeed = 7.0f;
        maximumSpawnSpeed = 2.1f;
		lastSpawn = Time.time;
		incrementSpeed = 14f;
		spawnIncrement = 0.25f;
		lastIncrement = Time.time;

        // --- Wave settings ---
        regularStateTime = 50f; // seconds              
        highStateTime = 20f;     // seconds               
        sinceLastState = Time.time;                
        regularStateIncrement = 10f;
        highSpawnSpeed = 1.8f;
        waveState = 0;

        // --- Stress settings ---
        Stress = 0f;
		floorStress = 0f;
		otherStress = 0f;

        Time.timeScale = 1;
        gameOver = GameObject.Find("GameOver").GetComponent<Animator>();
        rand = new System.Random((int)System.DateTime.Now.Ticks & 0x0000FFFF);
        randomMons = new System.Random((int)System.DateTime.Now.Ticks & 0x0000FFFF);

        monsterAdding = false;
        monsterTypeCount = 0;

        // Initialize the list of floors and y and x Coordinates
        GameObject floorList = GameObject.Find ("Floors");
		floors = new List<Transform> ();
		foreach (Transform child in floorList.transform)
        {
			floors.Add (child);
		}
		floorPosY = new List<float> ();
		foreach (Transform child in floorList.transform)
        {
			floorPosY.Add (child.transform.position.y + FLOOR_NUDGE);
		}
		floorPosX = new List<float> ();
		for (int i = 0; i < MAX_MONSTER_FLOOR; i++)
        {
			if (i == 0)
            {
				floorPosX.Add (X_MONSTER_START);
			}
			else floorPosX.Add (floorPosX[i-1] + X_MONSTER_WIDTH);
		}
        // Initialize the list of elevators
		GameObject el = GameObject.Find ("Elevators");
		elevators = new List<Transform> ();
		foreach (Transform child in el.transform)
        {
			elevators.Add (child);
		}

        // Initialize the list of monster names
        monsterNames = new List<string>();
        monsterNames.Add("MrMonster");
        monsterTypeCount = 1;
        monsterNames.Add("MonsterMonroe");
        monsterNames.Add("DrKhil");
        monsterNames.Add("HulkiestHunk");


        UpdateScore ();
        spawnMonster ();

        
    }

    // ------------------------------------------------------------------------------
    //                                   UPDATE
    // ------------------------------------------------------------------------------
    void Update () {
        if (Stress >= 1f)
        {
            //paused = !paused;
            Debug.Log(gameOver.runtimeAnimatorController.name);
            Time.timeScale = 0;
            GameObject.Find("GameOver").transform.GetComponent<Canvas>().enabled = true;
            GameObject.Find("GameOverScore").GetComponent<Text>().text = score + " points";
            return;
        }

        // TotalStress algorithm
        calculateFloorStress();
        calculateDisplayStress();

        wave();

        // Check if the player has clicked the monster & put it on the elevator 
        monsterClicked();

        // Check if a new monster type should be added to the world
        addMonster();
	}

    // ------------------------------------------------------------------------------
    //                              Spawning wave
    // ------------------------------------------------------------------------------

    void wave()
    {
        // Spawning Monsters - wave
        // Regular state
        if (waveState == 0)
        {
            if (Time.time - lastIncrement >= incrementSpeed)
            {
                increaseSpawnSpeed();
            }
            if (Time.time - lastSpawn >= spawnSpeed)
            {
                spawnMonster();
            }
            if (Time.time - sinceLastState >= regularStateTime)
            {
                GameObject.Find("MonsterWave").GetComponent<Animator>().SetTrigger("Wave");
                // HERE IS GOING TO BE A MONSTER WAVE TEXT @Sigrun
                sinceLastState = Time.time;
                waveState = 1;
            }
        }
        // High state
        else if (waveState == 1)
        {
            if (Time.time - lastSpawn >= highSpawnSpeed)
            {
                spawnMonster();
            }
            if (Time.time - sinceLastState >= highStateTime)
            {
                regularStateTime = regularStateTime + regularStateIncrement;
                sinceLastState = Time.time;
                waveState = 0;
            }
            Debug.Log("We are in HIGH state bitches!");
        }
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
                GameObject.Find("MenuScore").GetComponent<Text>().text = "score " + score;
                return;
            }
            else if (hit.collider != null && hit.transform.gameObject.tag == "Monster")
            {
                GameObject monster = hit.transform.gameObject;
                Transform floor = monster.transform.parent;
                int floorNr = -1;
                foreach (Transform f in floors)
                {
                    if (f == floor)
                    {
                        floorNr = floors.IndexOf(f);
                    }
                }
                if (isElevatorAtFloor(floorNr) && floorNr != -1)
                {
                    Monster monsterScript = monster.GetComponent<Monster>();
                    Transform el = getOpenElevatorAtFloor(floorNr);
                    if (el != null)
                    {
                        if (el.GetChild(0).childCount == 0)
                        {
                            if(monsterScript.name == "MonsterMonroe")
                            {
                                ContinuePatience(floor);
                            }

                            monster.transform.parent = el.GetChild(0).transform;
                            monster.transform.position = new Vector2((el.GetChild(0).transform.position.x), (el.GetChild(0).transform.position.y + 0.06f));
                            monsterScript.patience.transform.position = new Vector2((el.GetChild(0).transform.position.x) - 0.05f, (el.GetChild(0).transform.position.y + 0.06f));
                            monsterScript.patienceScript.currentAmount = -1;
                            totalMonsters--;
                            repositionMonstersAtFloor(floor);
                        }
                        else if (el.GetChild(1).childCount == 0)
                        {
                            if (monsterScript.name == "MonsterMonroe")
                            {
                                ContinuePatience(floor);
                            }

                            monster.transform.parent = el.GetChild(1).transform;
                            monster.transform.position = new Vector2((el.GetChild(1).transform.position.x), (el.GetChild(1).transform.position.y + 0.06f));
                            monsterScript.patience.transform.position = new Vector2((el.GetChild(1).transform.position.x) - 0.05f, (el.GetChild(1).transform.position.y + 0.06f));
                            monsterScript.patienceScript.currentAmount = -1;
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

    // ------------------------------------------------------------------------------
    //                                  Score
    // ------------------------------------------------------------------------------

    void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Score " + score;
        if (scoreOn)
        {
            GetComponent<AudioSource>().Play();
        }
    }

    static void increaseScore(int s)
    {
        score += s;
        //		scoreText.text = " " + score;
    }

    // ------------------------------------------------------------------------------
    //                             Stress/Patience meter
    // ------------------------------------------------------------------------------

    void calculateFloorStress()
    {
        floorStress = 0.0f;
        foreach (Transform f in floors)
        {
            foreach (Transform m in f)
            {
                floorStress += m.GetComponent<Monster>().getPatience();
            }
        }
        floorStress = floorStress / ((MAX_MONSTERS - MAX_MONSTER_FLOOR) * 100.0f);
    }

    void Stresser(float value)
    {
        otherStress += value;
    }

    void calculateDisplayStress()
    {
        if (otherStress > 0.0)
        {
            otherStress -= Time.deltaTime / 120f;
        }
        Stress = floorStress + otherStress;
        StressBar.size = 1 - Stress;
    }


    // ------------------------------------------------------------------------------
    //                              Monster Spawning 
    // ------------------------------------------------------------------------------

    public void spawnMonster()
    {
		if (totalMonsters >= MAX_MONSTERS)
        {
			return;
		}

		lastSpawn = Time.time;

		// FLOOR to spawn on
		int floorIndex = rand.Next (0, floors.Count);
		GameObject floor = floors[floorIndex].gameObject;
		while (floor.transform.childCount >= MAX_MONSTER_FLOOR && totalMonsters < MAX_MONSTERS) {
			Debug.Log ("Floor is full, the monster needs to find another one");
			floorIndex = rand.Next (0, floors.Count);
			floor = floors[floorIndex].gameObject;
		}

        // Create monster!
        // monsterArrive(posX, floorIndex, green, floor);
        instantiateMonster(floorIndex, floor);

    }

    string getRandomMonster()
    {
        int randomIndex;
        
        if (monsterNames.Count == 1)
        {
            return "MrMonster";
        }
        else if(monsterNames.Count >= 2)
        {
            rand = new System.Random((int)System.DateTime.Now.Ticks & 0x0000FFFF);
            if (rand.NextDouble() < 0.6)
            {
                return monsterNames[0];
            }
            else
            {
                rand = new System.Random((int)System.DateTime.Now.Ticks & 0x0000FFFF);
                randomIndex = rand.Next(1, 4);
                //randomIndex = 1;
                Debug.Log("Random index: "+randomIndex);
                return monsterNames[randomIndex];
            }
        }
        return "MrMonster";
    }

    // Instantiate a monster on the floor given
    void instantiateMonster(int posY, GameObject floor)
    {
        int posX = floor.transform.childCount;

        // MONSTER to spawn
        string name = getRandomMonster();
        GameObject monsType = (GameObject)Resources.Load(name);

        // Instantiate
        GameObject monster = Instantiate(monsType, new Vector2(floorPosX[posX], floorPosY[posY] + 0.1f), Quaternion.identity);
        monster.transform.parent = floor.transform;

        // Set current floor in Monster to get desired random number
        Monster monsterScript = monster.GetComponent<Monster>();
        int currFloor = posY + 1;
        monsterScript.currentFloor = currFloor;
        totalMonsters++;

        // Set monster name
        monsterScript.name = name;
    }

    // When a monster leaves every monsters shift left
    public void repositionMonstersAtFloor(Transform floor)
    {
        int i = 0;
        foreach (Transform child in floor.transform)
        {
            Vector3 pos = child.gameObject.transform.position;
            pos.x = floorPosX[i];
            child.gameObject.transform.position = pos;
            child.gameObject.GetComponent<Monster>().updatePos(pos);
            i++;
        }
    }

    public void increaseSpawnSpeed()
    {
        if (spawnSpeed >= maximumSpawnSpeed)
        {
            spawnSpeed -= spawnIncrement;
        }
        lastIncrement = Time.time;
    }

    // Adding a monster to the list of monster spawning
    void addMonster()
    {
        if(monsterTypeCount == 1 && monsterAdding)
        {
            monsterNames.Add("MonsterMonroe");
            monsterAdding = false;
            monsterTypeCount = 2;
        }
        else if(monsterTypeCount == 2 && monsterAdding)
        {
            monsterNames.Add("DrKhil");
            monsterAdding = false;
            monsterTypeCount = 3;
        }
    }

    // ------------------------------------------------------------------------------
    //                   Monster Abilites that changes the game
    // ------------------------------------------------------------------------------

    // Monster monroe makes everyone at her floor very patient
    public void PatienceCalmer(Transform floor)
    {
        foreach (Transform monster in floor.transform)
        {
            if(monster.GetComponent<Monster>().name != "MonsterMonroe")
            {
                monster.GetComponent<Monster>().anim.SetInteger("State", 3);
                monster.GetComponent<Monster>().patienceScript.patienceStopper = true;
            }
        }
    }

    public void ContinuePatience(Transform floor)
    {
        foreach (Transform monster in floor.transform)
        {
            if (monster.GetComponent<Monster>().name != "MonsterMonroe")
            {
                monster.GetComponent<Monster>().anim.SetInteger("State", 1);
                monster.GetComponent<Monster>().patienceScript.patienceStopper = false;
            }

        }
    }

    // The Hulkiest Hunk shakes the floor
    public void ShakeFloor(int floorNumber)
    {

    }

    // ------------------------------------------------------------------------------
    //                              Elevator functions
    // ------------------------------------------------------------------------------

    public bool isElevatorAtFloor(int floorNr)
    {
        foreach (Transform el in elevators)
        {
            ElevatorTest elscript = el.gameObject.GetComponent<ElevatorTest>();
            Debug.Log(elscript);
            Debug.Log(floorNr);
            Debug.Log(elscript.thisFloor);
            if ((/*elscript.currFloor == floorNr || */elscript.thisFloor == floorNr + 1) && !elscript.movingDown && !elscript.movingUp)
            {
                Debug.Log("I got this far");
                return true;
            }
        }
        return false;
    }

    public Transform getOpenElevatorAtFloor(int floorNr)
    {
        foreach (Transform el in elevators)
        {
            ElevatorTest elscript = el.gameObject.GetComponent<ElevatorTest>();
            if ((/*elscript.currFloor == floorNr || */elscript.thisFloor == floorNr + 1) && !elscript.movingDown && !elscript.movingUp && elscript.doorOpen)
            {
                return el;
            }
        }
        return null;
    }

    public void elevatorArrived (GameObject elevator)
    {
		int elFloor = elevator.GetComponent<ElevatorTest> ().thisFloor;
		if (elevators != null) {
			foreach(Transform e in elevators){
				if (e != null && e.gameObject != elevator && e.GetComponent<ElevatorTest> ().thisFloor == elFloor) {
					e.GetComponent<ElevatorTest> ().closeDoor ();
				}
			}
		}

		elevator.GetComponent<ElevatorTest> ().openDoor ();
        
        if (elevator.transform.GetChild (0).childCount > 0) {
            GameObject monster1 = elevator.transform.GetChild(0).GetChild(0).gameObject;
            Monster monster1Script = monster1.GetComponent<Monster>();
            if (monster1Script.desiredFloor == (elFloor))
            {
                Destroy(monster1);
                AddScore(scoreValue);
            }
        }
		if (elevator.transform.GetChild (1).childCount > 0) {
            GameObject monster2 = elevator.transform.GetChild(1).GetChild(0).gameObject;
            Monster monster2Script = monster2.GetComponent<Monster>();
            if(monster2Script.desiredFloor == (elFloor))
            {
                Destroy(monster2);
                AddScore(scoreValue);
            }  
		}
	}

    public void elevatorDeparting(GameObject elevator){
		int elFloor = elevator.GetComponent<ElevatorTest> ().thisFloor;
		foreach(Transform e in elevators){ 
			ElevatorTest elScript = e.GetComponent<ElevatorTest> ();
			if (e.gameObject != elevator && (/*elScript.currFloor == elFloor || */elScript.thisFloor == elFloor) && !elScript.movingDown && !elScript.movingUp) {
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


    // Removing bubble when the elevator goes up and down with the monster.
    public void removeBubble(GameObject elev)
    {
        if (elev.transform.GetChild(0).childCount > 0)
        {
            GameObject monster1 = elev.transform.GetChild(0).GetChild(0).gameObject;
            Monster monster1Script = monster1.GetComponent<Monster>();
            Destroy(monster1Script.patience); 
        }
        if (elev.transform.GetChild(1).childCount > 0)
        {
            GameObject monster2 = elev.transform.GetChild(1).GetChild(0).gameObject;
            Monster monster2Script = monster2.GetComponent<Monster>();
            Destroy(monster2Script.patience);
        }
    }

}
