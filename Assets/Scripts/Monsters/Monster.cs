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
    public string monsterName;						// Monster type/name
	private static readonly string[] monsterNames = {"MrMonster1", "MonsterMonroe1", "DrKhil1", "HulkiestHunk1"};

	// Patience bubble
    public GameObject patience;
    public Patience patienceScript;

	// Game controller script
	//  private GameControllerTest gameScript;
	private GameControllerScript gameScript;
    
	private static System.Random rand;
    public Animator anim;

    public bool insideElevator;

    // For Hulkiest Hunk
    private bool hasShakedFloor;

    // Use this for initialization
    void Start(){
		init ();
		createPatienceBubble ();
        insideElevator = false;
        hasShakedFloor = false;
    }

    // Update is called once per frame
    void Update(){
		checkPatience ();
        if (insideElevator)
        {
            movePatienceBubble();
        }
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

		patience = Instantiate(patienceBubble, new Vector2(transform.position.x, transform.position.y + 250f), Quaternion.identity);
		patience.transform.SetParent(canvas.transform, true);

		// Get desired floor number from the patience bubble
		patienceScript = patience.GetComponent<Patience>();
		patienceScript.setDesiredFloor(desiredFloor);
	}

	void checkPatience(){
        // Check if the patience bubble is 100% red! If it is then remove monster.
        Transform floor = transform.parent;
        if (getPatience() >= 100f){
            if (monsterName == monsterNames[1])
            {
                gameScript.continuePatience(floor);
            }

			// Dr Khil, leave with me yo!
            if (monsterName == monsterNames[2]) {
                gameScript.destroySomeoneWithMe(floor, gameObject, currentFloor);
            }
            
			StartCoroutine(destroyMonster());
            
//			gameScript.monsterLeft(floor);
			
        }
		else if (monsterName == monsterNames[3] && patienceScript.currentAmount > 85f){
            if(!hasShakedFloor)
            {
                anim.SetTrigger("Jump");
                gameScript.shakeFloor(floor);
                hasShakedFloor = true;
            }
		}
		else if (patienceScript.currentAmount > 90f){
			anim.SetInteger("State", 1);
		}
        
        if(monsterName == monsterNames[1] && getPatience() < 100f)
        {
            gameScript.patienceCalmer(floor);
        }
	}

	IEnumerator destroyMonster(){
		float time = 1.0f;
		spawnCloudParticles ();
		gameObject.transform.SetParent(gameObject.transform.parent.transform.parent);
		Destroy(patience);
		yield return new WaitForSeconds (time);
		gameScript.monsterLeft(currentFloor);
		gameScript.destroyMe (gameObject);
	}

	public void spawnCloudParticles(){
		GameObject clouds = (GameObject)Resources.Load ("Particles/CloudParticles");
		GameObject cloudParticles = Instantiate(clouds, transform.position, Quaternion.identity);
		Destroy (cloudParticles, 1.5f);
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

    public void monsterInsideElevator(Transform openElevator, Transform pos)
    {
        float x = pos.transform.position.x;
        float y = pos.transform.position.y;
        patience.transform.position = new Vector2(x, y);
        patienceScript.currentAmount = -1;
        insideElevator = true;
    }

    private void movePatienceBubble()
    {
        patience.transform.position = new Vector2(transform.position.x, transform.position.y + 250f);
    }

    public void destroyPatience(Monster monster)
    {
        Destroy(monster.patience);
    }

	public void increasePatience(float increment){
		patienceScript.currentAmount += increment;
	}
}