using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour {

	public void Restart()
    {
        GameObject.Find("Menu").transform.GetComponent<Canvas>().enabled = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void Resume()
    {
        GameObject.Find("Menu").transform.GetComponent<Canvas>().enabled = false;
        Time.timeScale = 1;
    }
}
