using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour {

	private const float MAX_X = 1080.0f;
	private const float MIN_X = -1080.0f;

	public float maxMove;
	public float vertSpeed;
	public float horSpeed;
	private float nextX;

	// Use this for initialization
	void Start () {		
		maxMove = 50.0f;
		vertSpeed = 2.0f;
		horSpeed = 40.0f;
		nextX = -1080f;
		transform.position = new Vector3 (-1080.0f, 0.0f, 0f);
	}

	// Update is called once per frame
	void Update () {
		nextX += Time.deltaTime * horSpeed;
		if (nextX >= MAX_X) {
			nextX = MIN_X;
		}
		float f = Mathf.Sin(Time.time * vertSpeed) * maxMove;
		transform.localPosition = new Vector3(nextX, f, 0.0f);
	}
}
