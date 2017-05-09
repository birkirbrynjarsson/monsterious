using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ClickOnFloor : MonoBehaviour
{

    ElevatorTest elevatorScript;

    const float FLOOR_NUDGE = 0.925f;
    float speed = 1.0f;

    string parent;
    public int destFloor = 0;
    float arriveTime = .0f;
    public float destSpeed = .0f;

    GameControllerTest gameController;
    GameObject ele;
    ElevatorTest elScript;

    public void clickOnFloor()
    {
        parent = EventSystem.current.currentSelectedGameObject.transform.parent.name;
        destFloor = Int32.Parse(EventSystem.current.currentSelectedGameObject.name);
        gameController = GameObject.Find("GameController").GetComponent<GameControllerTest>();

        MoveElevator();
    }

    void MoveElevator()
    {
        if (parent == "Right")
        {
            Mover("Elevator3");
        }
        else if (parent == "Middle")
        {
            Mover("Elevator2");
        }
        else if (parent == "Left")
        {
            Mover("Elevator1");
        }
    }

    void Mover(string elevator)
    {
        ele = GameObject.Find(elevator);
        elScript = ele.gameObject.GetComponent<ElevatorTest>();
        iTween.EaseType easing = iTween.EaseType.easeInOutSine;

        if (this.arriveTime > Time.time)
        {
            this.arriveTime += speed;
            easing = iTween.EaseType.easeOutSine;
        }
        else
        {
            gameController.removeBubble(ele);
            elScript.checkFloorIndicator();
            elScript.closeDoor();
            this.arriveTime = speed + Time.time;
            gameController.elevatorDeparting(ele);
        }

        destSpeed = arriveTime - Time.time;
        iTween.MoveTo(ele, iTween.Hash("position", TransformElevator(ele), "easetype", easing, "time", destSpeed, "oncomplete", "arrivedAtFloor"));
        elScript.thisFloor = destFloor;
    }

    Vector3 TransformElevator(GameObject ele)
    {
        return new Vector3(ele.transform.position.x, EventSystem.current.currentSelectedGameObject.transform.position.y + 0.21f, 0);
    }
}

