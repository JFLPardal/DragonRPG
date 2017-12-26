using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] float walkMoveStopRadius = 0.2f;
	[SerializeField] float attackMoveStopRadius = 5f;

	ThirdPersonCharacter thirdPersonCharacter;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster;
    Vector3 currentDestination, clickPoint;
     
	bool isInControllerMode = false;	//TODO consider make this variable static

    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        currentDestination = transform.position;	
    }

    private void FixedUpdate()
	{//TODO allow the player to choose this button
		if (Input.GetKeyDown (KeyCode.G)) //G for gamepad 
		{ 
			isInControllerMode = !isInControllerMode;
			currentDestination = transform.position;	//clear click target
		}
		if (isInControllerMode)
			ProcessGamepadMovement ();
		else
        	ProcessMouseMovement ();
    }

	private void ProcessGamepadMovement()
	{
		//get input values
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		// calculate camera relative direction to move:
		Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 movement = v * cameraForward + h * Camera.main.transform.right;

		thirdPersonCharacter.Move (movement, false, false);
	}

	private void ProcessMouseMovement ()
	{
		if (Input.GetMouseButton (0)) 
		{
			clickPoint = cameraRaycaster.hit.point;
			switch (cameraRaycaster.currentLayerHit) {
			case Layer.walkable:
				currentDestination = ShortDestination(clickPoint, walkMoveStopRadius);
				break;
			case Layer.enemy:
				currentDestination = ShortDestination (clickPoint, attackMoveStopRadius);
				break;
			default:
				print ("unexpected layer");
				return;
			}
		}
		WalkToDestination ();
	}

	private void WalkToDestination ()
	{
		var playerToClickPoint = currentDestination - transform.position;
		if (playerToClickPoint.magnitude > 0)
			thirdPersonCharacter.Move (playerToClickPoint, false, false);
		else
			thirdPersonCharacter.Move (Vector3.zero, false, false);
	}

	private Vector3 ShortDestination(Vector3 destination, float shortening)
	{
		Vector3 reductionVector = (destination - transform.position).normalized * shortening;
		return destination - reductionVector;
	}

	void OnDrawGizmos()
	{
		//draw movement gizmos
		Gizmos.color = Color.black;
		Gizmos.DrawLine (transform.position, currentDestination);
		Gizmos.DrawSphere (currentDestination, 0.1f);
		Gizmos.DrawSphere (clickPoint, 0.1f);

		//draw attack gizmos
		Gizmos.color = new Color(255, 0, 0, 0.5f);
		Gizmos.DrawWireSphere (transform.position, attackMoveStopRadius);
	}
}

