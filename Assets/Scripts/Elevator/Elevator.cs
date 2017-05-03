using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void MoveUp () {
        GameObject elevator = GameObject.Find("ElevatorRight");
        Vector3 pos = elevator.transform.position;
        pos.y += 1.3f;
        iTween.MoveTo(elevator, pos, 1);
    }

    public static void MoveDown() {
        GameObject elevator = GameObject.Find("ElevatorRight");
        Vector3 pos = elevator.transform.position;
        pos.y += -1.3f;
        iTween.MoveTo(elevator, pos, 1);
    }
}
