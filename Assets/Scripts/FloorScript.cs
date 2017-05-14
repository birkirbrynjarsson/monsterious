using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorScript : MonoBehaviour {

	Vector3 originalPosition;
	bool floorShaking;
	float shakeDuration = 1.0f;
	float decreaseFactor = 1.0f;
	float shakeAmount = 100f;


	// Use this for initialization
	void Start () {
		floorShaking = false;
		originalPosition = transform.position;
		shakeDuration = 1.0f;
		decreaseFactor = 1.0f;
		shakeAmount = 100f;
	}
	
	// Update is called once per frame
	void Update () {
		if (floorShaking) {
			if (shakeDuration > 0) {
				transform.localPosition = originalPosition + Random.insideUnitSphere * shakeAmount;
				shakeDuration -= Time.deltaTime * decreaseFactor;
			} else {
				shakeDuration = 1.0f;
				floorShaking = false;
				transform.localPosition = originalPosition;
			}
		}
	}

	public IEnumerator shakeFloor(){
		float animationLength = 0.6f;
		yield return new WaitForSeconds (animationLength);
		floorShaking = true;
	}
}
