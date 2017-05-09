using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour {

    private GameControllerTest gameScript;

	public void Restart()
    {
        Debug.Log("hello");
        GameObject.Find("Menu").transform.GetComponent<Canvas>().enabled = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void Resume()
    {
        GameObject.Find("Menu").transform.GetComponent<Canvas>().enabled = false;
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        GameObject.Find("GameOver").transform.GetComponent<Canvas>().enabled = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
