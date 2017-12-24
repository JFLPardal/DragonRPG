using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] float walkMoveStopRadius = 0.2f;

    ThirdPersonCharacter m_Character;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster;
    Vector3 currentClickTarget;
     
	bool isInControllerMode = false;	//TODO consider make this variable static

    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        m_Character = GetComponent<ThirdPersonCharacter>();
        currentClickTarget = transform.position;
    }

    private void FixedUpdate()
    {
		if(Input.GetKeyDown(KeyCode.G)) //G for gamepad TODO allow the player to choose this button
			isInControllerMode = !isInControllerMode;

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
		Vector3 m_CamForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 m_Move = v*m_CamForward + h * Camera.main.transform.right;

		m_Character.Move (m_Move, false, false);
	}

	private void ProcessMouseMovement ()
	{
		if (Input.GetMouseButton (0)) 
		{
			switch (cameraRaycaster.layerHit) {
			case Layer.walkable:
				currentClickTarget = cameraRaycaster.hit.point;
				var playerToClickPoint = currentClickTarget - transform.position;
				if (playerToClickPoint.magnitude > walkMoveStopRadius)
					m_Character.Move (playerToClickPoint, false, false);
				else
					m_Character.Move (Vector3.zero, false, false);
				break;
			case Layer.enemy:
				print ("not walking to enemy");
				break;
			default:
				print ("unexpected layer");
				return;
			}
		}
		else
			m_Character.Move (Vector3.zero, false, false);
	}
}

