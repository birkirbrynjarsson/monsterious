using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour {

    string sceneOne = "Welcome to Monster Terminal \nI am princess monsterverse and I will guide you through the game";
    string sceneTwo = "You can move to a different floor by pressing the buttons";
    string sceneThree = "You can move a monster to a open elevator by pressing on it, you can only fit two monsters at once";
    string sceneFour = "This is mr. monster, he and his clones love to ride the elevators";
    string sceneFive = "This is Monster Monroe \n She is so attractive that the timer on the monsters on her floor stops";
    string sceneSix = "This is Hunkiest Hulk \n If you don’t get him in the elevator on time he will shake the ground with his anger, annoying his neighbours";
    string sceneSeven = "This is Dr. Khil \n Be vary, if you don’t get him to the elevator in time he will convince someone to leave with him";

    int counter = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void skipTutorial()
    {
        SceneManager.LoadScene(2);
    }

    public void Right()
    {
        counter++;
        if (counter == 1)
        {
            GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>().text = sceneTwo;
            GameObject.FindGameObjectWithTag("TutorialLeft").GetComponent<Image>().enabled = true;
            GameObject.FindGameObjectWithTag("Tutorial1").GetComponent<Animator>().enabled = true;
        }
        else if (counter == 2)
        {
            GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>().text = sceneThree;
            GameObject tutorial2 = (GameObject)Resources.Load("Tutorial/Tutorial2");
            Vector2 position = new Vector2(0, 0);
            GameObject tutorial = Instantiate(tutorial2, position, Quaternion.identity);
            Destroy(GameObject.FindGameObjectWithTag("Tutorial1"));
        }
        else if (counter == 3)
        {
            GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>().text = sceneFour;
            GameObject mrMonsterRe = (GameObject)Resources.Load("Tutorial/MrMonster2");
            Vector2 position = new Vector2(100, 600);
            GameObject mrMonster = Instantiate(mrMonsterRe, position, Quaternion.identity);
            Destroy(GameObject.FindGameObjectWithTag("Tutorial2"));
        }
        else if (counter == 4)
        {
            GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>().text = sceneFive;
            GameObject monsterMonroeRe = (GameObject)Resources.Load("Tutorial/MonsterMonroe2");
            Vector2 position = new Vector2(400, 3900);
            GameObject monsterMonroe = Instantiate(monsterMonroeRe, position, Quaternion.identity);
            Destroy(GameObject.FindGameObjectWithTag("TutorialMrMonster"));
        }
        else if (counter == 5)
        {
            GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>().text = sceneSix;
            GameObject tutorial2 = (GameObject)Resources.Load("Tutorial/HulkiestHunk2");
            Vector2 position = new Vector2(-350, -1600);
            GameObject tutorial = Instantiate(tutorial2, position, Quaternion.identity);
            Destroy(GameObject.FindGameObjectWithTag("TutorialMonsterMonroe"));
        }
        else if (counter == 6)
        {
            GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>().text = sceneSeven;
            GameObject tutorial2 = (GameObject)Resources.Load("Tutorial/DrKhil2");
            Vector2 position = new Vector2(-1100, 1200);
            GameObject tutorial = Instantiate(tutorial2, position, Quaternion.identity);
            Destroy(GameObject.FindGameObjectWithTag("TutorialHulkiestHunk"));
            GameObject.FindGameObjectWithTag("TutorialRight").GetComponent<Image>().enabled = false;
            GameObject.FindGameObjectWithTag("TutorialSkip").GetComponent<Text>().text = "Let's play!";
        }
    }

    public void Left()
    {
        counter--;
        if (counter == 0)
        {
            GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>().text = sceneOne;
            GameObject tutorial1 = (GameObject)Resources.Load("Tutorial/Tutorial1");
            Vector2 position = new Vector2(0, 0);
            Destroy(GameObject.FindGameObjectWithTag("Tutorial1"));
            GameObject tutorial = Instantiate(tutorial1, position, Quaternion.identity);
            GameObject.FindGameObjectWithTag("TutorialLeft").GetComponent<Image>().enabled = false;
            tutorial.GetComponent<Animator>().enabled = false;
        }
        else if (counter == 1)
        {
            GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>().text = sceneTwo;            
            GameObject tutorial2 = (GameObject)Resources.Load("Tutorial/Tutorial1");
            Vector2 position = new Vector2(0, 0);
            GameObject tutorial = Instantiate(tutorial2, position, Quaternion.identity);
            Destroy(GameObject.FindGameObjectWithTag("Tutorial2"));
        }
        else if (counter == 2)
        {
            GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>().text = sceneThree;
            GameObject tutorial2 = (GameObject)Resources.Load("Tutorial/Tutorial2");
            Vector2 position = new Vector2(0, 0);
            GameObject tutorial = Instantiate(tutorial2, position, Quaternion.identity);
            Destroy(GameObject.FindGameObjectWithTag("TutorialMrMonster"));
        }
        else if (counter == 3)
        {
            GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>().text = sceneFour;
            GameObject mrMonsterRe = (GameObject)Resources.Load("Tutorial/MrMonster2");
            Vector2 position = new Vector2(100, 600);
            GameObject mrMonster = Instantiate(mrMonsterRe, position, Quaternion.identity);
            Destroy(GameObject.FindGameObjectWithTag("TutorialMonsterMonroe"));
        }
        else if (counter == 4)
        {
            GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>().text = sceneFive;
            GameObject monsterMonroeRe = (GameObject)Resources.Load("Tutorial/MonsterMonroe2");
            Vector2 position = new Vector2(400, 3900);
            GameObject monsterMonroe = Instantiate(monsterMonroeRe, position, Quaternion.identity);
            Destroy(GameObject.FindGameObjectWithTag("TutorialHulkiestHunk"));
        }
        else if (counter == 5)
        {
            GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>().text = sceneSix;
            GameObject tutorial2 = (GameObject)Resources.Load("Tutorial/HulkiestHunk2");
            Vector2 position = new Vector2(-350, -1600);
            GameObject tutorial = Instantiate(tutorial2, position, Quaternion.identity);
            Destroy(GameObject.FindGameObjectWithTag("TutorialDrKhil"));

            GameObject.FindGameObjectWithTag("TutorialRight").GetComponent<Image>().enabled = true;
        }
    }
}
