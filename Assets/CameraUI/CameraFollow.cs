using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	Transform target;


	void Start () {
		target = GameObject.FindGameObjectWithTag("Player").transform;
	}

	void LateUpdate () {
		transform.position =  target.position;
	}
}
