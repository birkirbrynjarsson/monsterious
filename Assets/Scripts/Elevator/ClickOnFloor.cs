using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ClickOnFloor : MonoBehaviour {

    ElevatorTest elevatorScript;
    GameControllerTest gameController;
    public int floorNr;
    string parent;
    public bool doorOpen = false;
    public float speed = 1.0f;

    private GameObject door;

    // Use this for initialization
    void Start () {
        floorNr = 0;
        parent = "";
    }

    public void clickOnFloor()
    {
        parent = EventSystem.current.currentSelectedGameObject.transform.parent.name;
        floorNr = Int32.Parse(EventSystem.current.currentSelectedGameObject.name);
        door = GameObject.Find("door").gameObject;
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
        GameObject ele = GameObject.Find(elevator);
        iTween.EaseType easing = iTween.EaseType.easeInOutSine;
        gameController.removeBubble(ele);
        closeDoor();
        float arriveTime = speed + Time.time;
        float destSpeed = arriveTime - Time.time;
        iTween.MoveTo(ele, iTween.Hash("position", TransformElevator(ele), "easetype", easing, "time", destSpeed, "oncomplete", "arrivedAtFloor"));
    }

    Vector3 TransformElevator(GameObject ele)
    {
        return new Vector3(ele.transform.position.x, EventSystem.current.currentSelectedGameObject.transform.position.y + 0.21f, 0);
    }

    public void arrivedAtFloor()
    {
        //movingUp = false;
        //movingDown = false;
        //upActive.gameObject.SetActive(false);
        //downActive.gameObject.SetActive(false);
        //currFloor = destFloor;

        // Notify the Game Controller
        if (gameController != null)
        {
            Debug.Log("heeeyyyy");
            gameController.Arrived(gameObject);
        }
        Debug.Log("I just arrived at a floor bitch!");
    }

    public void openDoor()
    {
        doorOpen = true;
        //disableFloorIndicator();
        door.SetActive(false);
    }

    public void closeDoor()
    {
        doorOpen = false;
        door.SetActive(true);
        //checkFloorIndicator();
    }


}
