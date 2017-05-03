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
                    Debug.Log("SUCCESS!! You clicked: " + monster.name);
                    monster.transform.parent = elevator.transform;
                    monster.transform.position = elevator.transform.position + 3;
                }
            }

        }
    }
}
