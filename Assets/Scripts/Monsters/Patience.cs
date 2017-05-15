using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Patience : MonoBehaviour {

    public Transform LoadingBar;
    public Transform TextIndicator;
    private Monster monsterScript;
    public int desiredFloor;
    public int currentFloor;
    public bool patienceStopper;

    //public Transform TextLoading;
    [SerializeField] public float currentAmount;
    [SerializeField] private float speed;


    // Use this for initialization
    void Start () {
		speed = 3.0f;
        patienceStopper = false;
		currentAmount = 0f;
    }

    // Update is called once per frame
    void Update () {
        if(!patienceStopper)
        {
            if (currentAmount == -1)
            {

            }
            else if (currentAmount < 100)
            {
                currentAmount += speed * Time.deltaTime;
            }

            LoadingBar.GetComponent<Image>().fillAmount = currentAmount / 100;
        }

    }

    internal void setDesiredFloor(int des)
    {
        desiredFloor = des;
        TextIndicator.GetComponent<Text>().text = desiredFloor.ToString();
    }
}
