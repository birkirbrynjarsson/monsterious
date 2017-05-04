using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GUIText scoreText;
    private static int score;

	// Use this for initialization
	void Start () {
        score = 0;
        UpdateScore();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddScore(int newScoreValue) {
        score += newScoreValue;
        UpdateScore();
    }

    static void UpdateScore () {
        scoreText.text = " " + score;
    }
}
