using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Patience : MonoBehaviour {

    public Transform LoadingBar;
    public Transform TextIndicator;
    //public Transform TextLoading;
    [SerializeField] private float currentAmount;
    [SerializeField] private float speed;

    // Use this for initialization
    void Start () {
        //TODO: Find random number from 1-7 and put to floor
        System.Random rand = new System.Random();
        int desiredFloor = rand.Next(1, 7);
        Debug.Log(desiredFloor);
        TextIndicator.GetComponent<Text>().text = desiredFloor.ToString();
    }

    // Update is called once per frame
    void Update () {
		
        if(currentAmount < 100)
        {
            currentAmount += speed * Time.deltaTime;
            //TextIndicator.GetComponent<Text>().text = ((int)currentAmount).ToString() + "%";
            //TextLoading.gameObject.SetActive(true);
        }
        else // 100%
        {
            //TextLoading.gameObject.SetActive(true);
            //TextIndicator.GetComponent<Text>().text = "DONE!";
            
            //TODO: Let the monster know that it should leave.. and the Patience meter will drop 
        }
        LoadingBar.GetComponent<Image>().fillAmount = currentAmount / 100;
    }
}
