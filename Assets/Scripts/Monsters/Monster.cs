using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;

public class Monster : MonoBehaviour
{
	private const int MAX_FLOORS = 6;

    public int currentFloor;				// The floor that the monster spawns at
    public int desiredFloor;				// The floor the monster desires to go to
    public string name;						// Monster type/name
	private static readonly string[] monsterTypes = {"MrMonster1", "MonsterMonroe", "DrKhil", "HulkiestHunk"};

	// Patience bubble
    public GameObject patience;
    public Patience patienceScript;

	// Game controller script
	//  private GameControllerTest gameScript;
	private GameControllerScript gameScript;
    
	private static System.Random rand;
    public Animator anim;

    // Use this for initialization
    void Start(){
		init ();
//		createPatienceBubble ();
    }

    // Update is called once per frame
    void Update(){
//		checkPatience ();
    }

	// Initialize variables
	void init(){
		rand = new System.Random((int)System.DateTime.Now.Ticks & 0x0000FFFF);
		desiredFloor = rand.Next(MAX_FLOORS) + 1;
		while (desiredFloor == currentFloor){
			desiredFloor = rand.Next(MAX_FLOORS) + 1;
		}

//		gameScript = GameObject.Find("GameController").GetComponent<GameControllerTest>();
		gameScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();

		anim = gameObject.GetComponent<Animator>();
	}

	void createPatienceBubble(){
		// Create/Instantiate Patience Bubble above the monster with the floor number
		GameObject patienceBubble = (GameObject)Resources.Load("PatienceBubble 1");
		GameObject canvas = GameObject.Find("PatienceSpawn");

		patience = Instantiate(patienceBubble, new Vector2(transform.position.x - 0.05f, transform.position.y + 0.68f), Quaternion.identity);
		patience.transform.SetParent(canvas.transform, true);

		// Get desired floor number from the patience bubble
		patienceScript = patience.GetComponent<Patience>();
		patienceScript.setDesiredFloor(desiredFloor);
	}

	void checkPatience(){
		// Check if the patience bubble is 100% red! If it is then remove monster.
		if (getPatience() >= 100f){
			Transform floor = transform.parent;
			gameObject.transform.SetParent(gameObject.transform.parent.transform.parent);
			Destroy(patience);
			Destroy(gameObject);
//			gameScript.monsterLeft(floor);
			gameScript.monsterLeft(currentFloor);
		}
		else if (name == "HulkiestHunk" && patienceScript.currentAmount > 85f){
			anim.SetInteger("State", 1);
			gameScript.shakeFloor(currentFloor);
		}
		else if (patienceScript.currentAmount > 90f){
			anim.SetInteger("State", 1);
		}
	}

    public float getPatience(){
        if (patienceScript != null){
            return patienceScript.currentAmount;
        }
        return 0.0f;
    }

    internal void updatePos(Vector2 pos){
        Vector2 newPos = patience.transform.position;
        newPos.x = pos.x - 0.05f;
        patience.transform.position = newPos;
    }

}