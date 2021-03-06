﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.touchCount > 0)
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit2D hit;
            Touch myTouch = Input.touches[0];

            if (myTouch.phase == TouchPhase.Began)
            {
                hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
                string parent = hit.transform.parent.name;
                if (hit.transform.gameObject.name == "ArrowDown")
                {
                    Elevator.MoveDown(parent);
                }
                else if (hit.transform.gameObject.name == "ArrowUp")
                {
                    Elevator.MoveUp(parent);                
                }
            }

        }
    }
}
