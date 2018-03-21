using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;
//TODO consider getting rid of this dependency

namespace RPG.Characters
{
	[RequireComponent(typeof (ThirdPersonCharacter))]
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(AICharacterControl))]

	public class PlayerMovement : MonoBehaviour
	{
		ThirdPersonCharacter thirdPersonCharacter = null;   // A reference to the ThirdPersonCharacter on the object
		CameraRaycaster cameraRaycaster = null;
	    Vector3 clickPoint;
		GameObject walkTarget = null;
		AICharacterControl aiCharacterControl = null;
	     
	    void Start()
	    {
	        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
	        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
			walkTarget = new GameObject ("walkTarget");
			aiCharacterControl = GetComponent<AICharacterControl> ();

			cameraRaycaster.notifyMouseOverPotentiallyWalkableObservers += ProcessPotentialWalkable;
			cameraRaycaster.notifyMouseOverEnemyObservers += OnMouseOverEnemy;
	    }

		void ProcessPotentialWalkable(Vector3 destination)
		{
			if(Input.GetMouseButton(0))
			{
				walkTarget.transform.position = destination;
				aiCharacterControl.SetTarget (walkTarget.transform);
			}	
		}
			
		void OnMouseOverEnemy(Enemy enemy)
		{
			if(Input.GetMouseButton(0) || Input.GetMouseButtonDown(1))
			{
				aiCharacterControl.SetTarget(enemy.transform);
			}
		}

		//TODO make this get called again
		void ProcessGamepadMovement()
		{
			//get input values
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");

			// calculate camera relative direction to move:
			Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
			Vector3 movement = v * cameraForward + h * Camera.main.transform.right;

			thirdPersonCharacter.Move (movement, false, false);
		}
	}
}
