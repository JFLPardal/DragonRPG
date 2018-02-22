using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
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
	[SerializeField] const int layerWalkable = 8;
	[SerializeField] const int layerEnemy = 9;

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

		cameraRaycaster.notifyMouseClickObservers += ProcessMouseClick;
		cameraRaycaster.notifyMouseOverPotentiallyWalkableObservers += ProcessPotentialWalkable;
    }

	void ProcessMouseClick(RaycastHit raycastHit, int layerHit)
	{
		switch(layerHit)
		{
			case(layerEnemy): 
				GameObject enemy = raycastHit.collider.gameObject;
				aiCharacterControl.SetTarget(enemy.transform);
				break;
			default:
				Debug.LogWarning ("Don't know how to handle mouse click for player movement");
				return;
		}
	}

		void ProcessPotentialWalkable(Vector3 destination)
		{
			if(Input.GetMouseButton(0))
			{
				walkTarget.transform.position = destination;
				aiCharacterControl.SetTarget (walkTarget.transform);
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
