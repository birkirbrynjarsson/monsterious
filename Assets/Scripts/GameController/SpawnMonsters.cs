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

        // Spawn monsters to floor (Hardcoded 3 green monsters)
        for(int i = 0; i < 3; i++)
        {
            System.Random rand = new System.Random();
            int randomIndex = rand.Next(0, floors.Length);
            float randomFloor = floors[randomIndex];
            GameObject floor = GameObject.Find("Floors/Floor" + (randomIndex + 1));
            //float randomFloor = floors[6];
            GameObject green = (GameObject)Resources.Load("greenMonster");
            
            // Check if the floor is full
            // TODO: Change this to a recursive function
            if(floor.transform.childCount == 3)
            {
                if(randomIndex == 0)
                {
                    floor = GameObject.Find("Floors/Floor" + 7);
                    randomFloor = floors[6];
                }
                floor = GameObject.Find("Floors/Floor" + (randomIndex));
                randomFloor = floors[randomIndex-1];
            }

            // Check if there is a monster on the floor... if so then spawn next to the monster.
            float x = 0.44f;
            float y = (randomFloor + 0.42f);

            for (int j = floor.transform.childCount; j > 0; j--)
            {
                Debug.Log("Hey! There is a monster there!");
                x = x + 1f;
            }
            Debug.Log("Alot of space!!");
            Instantiate(green, new Vector2(x, y), Quaternion.identity).transform.parent = floor.transform;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
