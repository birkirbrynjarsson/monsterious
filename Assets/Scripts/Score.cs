using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Score : MonoBehaviour {

    //public Text coinText;
    int coins;
    private static Score instance = null;


    void Awake () {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        //DontDestroyOnLoad(gameObject.GetComponent<Text>());
        instance.coins = int.Parse(GameObject.Find("Coins/CoinText").GetComponent<Text>().text);
    }

    void Update () {

    }

    public void AddCoins(int newCoinValue)
    {
        instance.coins += newCoinValue;
        GameObject.Find("Coins/CoinText").GetComponent<Text>().text = " " + coins;
    }
}
