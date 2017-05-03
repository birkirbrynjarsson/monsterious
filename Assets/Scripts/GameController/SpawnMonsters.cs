using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SpawnMonsters : MonoBehaviour {

	// Use this for initialization
	void Start () {
        float floor1 = GameObject.Find("Floors/Floor1").transform.position.y;
        float floor2 = GameObject.Find("Floors/Floor2").transform.position.y;
        float floor3 = GameObject.Find("Floors/Floor3").transform.position.y;
        float floor4 = GameObject.Find("Floors/Floor4").transform.position.y;
        float floor5 = GameObject.Find("Floors/Floor5").transform.position.y;
        float floor6 = GameObject.Find("Floors/Floor6").transform.position.y;
        float floor7 = GameObject.Find("Floors/Floor7").transform.position.y;

        float[] floors = new float[7] {floor1, floor2, floor3, floor4, floor5, floor6, floor7};

        // TODO: Check if there is a monster on the floor... if so then spawn next to the monster.
        for(int i = 0; i<2; i++)
        {
            System.Random rand = new System.Random();
            int randomIndex = rand.Next(0, floors.Length);
            float randomFloor = floors[randomIndex];

            GameObject green = GameObject.Find("greenMonster");

            Instantiate(green, new Vector2(2, (randomFloor+0.42f)), Quaternion.identity);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
