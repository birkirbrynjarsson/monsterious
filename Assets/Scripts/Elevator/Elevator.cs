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

    public static void MoveUp (string parent) {
        GameObject elevator = GameObject.Find(parent);
        Vector3 pos = elevator.transform.position;
        pos.y += 1.3f;
        iTween.MoveTo(elevator, pos, 1);
    }

    public static void MoveDown(string parent) {
        GameObject elevator = GameObject.Find(parent);
        Vector3 pos = elevator.transform.position;
        pos.y += -1.3f;
        iTween.MoveTo(elevator, pos, 1);
    }
}
