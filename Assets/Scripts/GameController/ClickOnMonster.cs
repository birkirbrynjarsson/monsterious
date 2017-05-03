using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickOnMonster : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit2D hit;

            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
                if (hit.collider != null && hit.transform.gameObject.tag == "Monster")
                {
                    GameObject monster = hit.transform.gameObject;
                    GameObject elevator = GameObject.Find("Elevator1");
                    GameObject bubble = GameObject.Find(monster.name + "/bubble");

                    Debug.Log("SUCCESS!! You clicked: " + monster.name);
                    
                    // TODO: CHECK IF THERE IS ANY ELEVATOR ON THAT FLOOR 
                    //     : USE SOMETHING ELSE TO CHECK IF THE ELEVATOR IS FULL OR NOT (THIS IS JUST TEMPO)
                    if(elevator.transform.childCount == 1)
                    {
                        monster.transform.parent = elevator.transform;
                        monster.transform.position = new Vector2(elevator.transform.position.x, (elevator.transform.position.y + 0.3f));
                        Destroy(bubble);
                        //elevator.transform.position;
                    }
                    else if (elevator.transform.childCount == 2)
                    {
                        monster.transform.parent = elevator.transform;
                        monster.transform.position = new Vector2(elevator.transform.position.x, (elevator.transform.position.y - 0.3f));
                        Destroy(bubble);
                        //elevator.transform.position;
                    }
                    else
                    {
                        Debug.Log("The elevator is full!! You clicked: " + monster.name);
                    }

                }
            }
        }
    }
}
