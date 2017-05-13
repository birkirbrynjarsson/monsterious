using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    private GameControllerTest gameScript;
    private Score scoreScript;
    
	public void Restart()
    {
        gameScript = GameObject.Find("GameController").GetComponent<GameControllerTest>();
        scoreScript = GameObject.Find("Coins/CoinText").GetComponent<Score>();
        scoreScript.AddCoins(gameScript.score);
        GameObject.Find("Menu").transform.GetComponent<Canvas>().enabled = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void Resume()
    {
        Debug.Log("Hey thereee...");
        GameObject.Find("Menu").transform.GetComponent<Canvas>().enabled = false;
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        //gameScript = GameObject.Find("GameController").GetComponent<GameControllerTest>();
        //gameScript.AddCoins(gameScript.score);
        GameObject.Find("GameOver").transform.GetComponent<Canvas>().enabled = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenSettings()
    {
        GameObject.Find("SettingsMenu").transform.GetComponent<Canvas>().enabled = true;
    }

    public void ExitSettings()
    {
        GameObject.Find("SettingsMenu").transform.GetComponent<Canvas>().enabled = false;
    }

    public void OpenMonsters()
    {
        GameObject.Find("MonsterMenu").transform.GetComponent<Canvas>().enabled = true;
    }

    public void CloseMonsters()
    {
        GameObject.Find("MonsterMenu").transform.GetComponent<Canvas>().enabled = false;
    }

    public void OpenStore()
    {
        GameObject.Find("StoreMenu").transform.GetComponent<Canvas>().enabled = true;
    }

    public void CloseStore()
    {
        GameObject.Find("StoreMenu").transform.GetComponent<Canvas>().enabled = false;
    }
}
