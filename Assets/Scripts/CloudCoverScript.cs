using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudCoverScript : MonoBehaviour {

	private const float yOrigin = -3840f;
	private const float yMax = 10f;

	// Sine movevemt, speed
	public float vertSpeed;
	public float maxMove;

	public float animationSpeed;
	private float maxDistance;

	// Use this for initialization
	void Start () {
		vertSpeed = 2.0f;
		maxMove = 1f;
		transform.position = new Vector3 (0.0f, yOrigin, 0f);
		animationSpeed = 0.4f;
		maxDistance = Mathf.Abs(yMax - yOrigin);

		StartCoroutine (stopOverdueClouds ());
	}

	void Update(){
//		float f = Mathf.Sin(Time.time * vertSpeed) * maxMove;
//		transform.localPosition = new Vector3(0.0f, (transform.localPosition.y + f), 0.0f);

	}

	// This function should take as input a percentage between 0 and 1
	public void move(float coverage){
		float yPos = yOrigin + (maxDistance * coverage);
		if (transform.position.y < yMax) {
			iTween.MoveTo (gameObject, iTween.Hash ("position", (new Vector3 (0f, yPos, 0f)), "easetype", iTween.EaseType.easeInOutSine, "time", animationSpeed));
		}
	}

	public bool isMaxed(){
		if (transform.position.y >= yMax){
			return true;
		}
		return false;
	}

	IEnumerator stopOverdueClouds(){
		while (true) {
			yield return new WaitForSeconds (0.1f);
			if (transform.position.y >= yMax) {
				iTween.Stop (gameObject);
			}
		}
	}
}
