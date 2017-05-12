using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {

    void Start()
    {
        Time.timeScale = 1;
    }

    public void startGame()
    {
        Debug.Log("clicked");
        Debug.Log(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(1);
    }
}
