using System.Collections;
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
                if (hit.collider != null && hit.transform.gameObject.tag == "Arrow")
                {
                    Debug.Log("SUCCESS" + hit.transform.gameObject.name);
                }
                if (hit.transform.gameObject.name == "ArrowDownRight")
                {
                    GameObject elevator = GameObject.Find("ElevatorRight");
                    Vector3 pos = elevator.transform.position;
                    pos.y += -1.3f;
                    iTween.MoveTo(elevator, pos, 1);
                    /* elevator.transform.position = Vector3.MoveTowards(
                        elevator.transform.position, 
                        pos, 
                        0.1 * Time.deltaTime);*/
                    // elevator.transform.position = pos;
                }
                else if (hit.transform.gameObject.name == "ArrowUpRight")
                {
                    GameObject elevator = GameObject.Find("ElevatorRight");
                    Vector3 pos = elevator.transform.position;
                    pos.y += 1.3f;
                    iTween.MoveTo(elevator, pos, 1);                    
                }
            }

        }
    }
}
