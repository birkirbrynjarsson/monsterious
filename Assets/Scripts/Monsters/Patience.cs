using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Patience : MonoBehaviour {

    public Transform LoadingBar;
    public Transform TextIndicator;
    public int desiredFloor;
    //public Transform TextLoading;
    [SerializeField] public float currentAmount;
    [SerializeField] private float speed;

    private static System.Random rand;

    // Use this for initialization
    void Start () {
        //TODO: Find random number from 1-7 and put to floor
        rand = new System.Random((int)Time.time);
        desiredFloor = rand.Next(1, 8);
    
        TextIndicator.GetComponent<Text>().text = desiredFloor.ToString();
    }

    // Update is called once per frame
    void Update () {
		
        if(currentAmount < 100)
        {
            currentAmount += speed * Time.deltaTime;
        }
        else // 100%
        {   
            //TODO: Let the monster know that it should leave.. and the Patience meter will drop 
        }
        LoadingBar.GetComponent<Image>().fillAmount = currentAmount / 100;
    }
}
