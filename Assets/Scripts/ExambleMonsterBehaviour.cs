using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExambleMonsterBehaviour : MonoBehaviour {
    public int floor = 1;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //if (Input.touchCount == 1)
        //{
        //    Touch touch = Input.GetTouch(0);

        //    float x = -7.5f + 15 * touch.position.x / Screen.width;
        //    float y = -4.5f + 9 * touch.position.y / Screen.height;

        //    transform.position = new Vector3(x, y, 0);
        //}

        //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit) && hit.transform.gameObject.name == "greenMonster")
        //    {
        //        //hit.GetComponent<TouchObjectScript>().ApplyForce();
        //        Touch touch = Input.GetTouch(0);

        //        float x = -7.5f + 15 * touch.position.x / Screen.width;
        //        float y = -4.5f + 9 * touch.position.y / Screen.height;

        //        transform.position = new Vector3(x, y, 0);
        //    }
        //}

      

            //if (Physics.Raycast(ray, out hit, 10000) && hit.transform.gameObject.tag == "Monster")
            //{
            //    ////hit.GetComponent<TouchObjectScript>().ApplyForce();
            //    //Touch touch = Input.GetTouch(0);

            //    //float x = -7.5f + 15 * touch.position.x / Screen.width;
            //    //float y = -4.5f + 9 * touch.position.y / Screen.height;

            //    //transform.position = new Vector3(x, y, 0);

            //    Touch touch = Input.GetTouch(0);

            //    float x = -7.5f + 15 * touch.position.x / Screen.width;
            //    float y = -4.5f + 9 * touch.position.y / Screen.height;
      
            //    transform.position = new Vector3(x, y, 0);
            //}
        }
    }
