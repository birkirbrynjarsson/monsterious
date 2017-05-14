using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour {

    string sceneOne = "Welcome to Monster Terminal \nI am princess monsterverse and I will guide you through the game";
    string sceneTwo = "You can move to a different floor by pressing the buttons";
    string sceneThree = "You can move a monster to a open elevator by pressing on it, you can only fit two monsters at once";

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
        if(counter == 1)
        {
            GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>().text = sceneTwo;
            GameObject.FindGameObjectWithTag("TutorialLeft").GetComponent<Image>().enabled = true;
            GameObject.FindGameObjectWithTag("Tutorial1").GetComponent<Animator>().enabled = true;
        }
        else if(counter == 2)
        {
            GameObject.FindGameObjectWithTag("TutorialText").GetComponent<Text>().text = sceneThree;
            GameObject.FindGameObjectWithTag("Tutorial2").GetComponent<Animator>().enabled = true;
        }
    }

    public void Left()
    {

    }
}
