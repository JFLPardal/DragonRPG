using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;

namespace RPG.Characters
{
	[RequireComponent(typeof(NavMeshAgent))]

	public class CharacterMovement : MonoBehaviour
	{
        [SerializeField] float stoppingDistance = 2f;
        [SerializeField] float moveSpeedMultiplier = .7f;
        [SerializeField] float movingTurnSpeed = 360f;
        [SerializeField] float animationSpeedMultiplier = 1.5f;
        [SerializeField] float stationaryTurnSpeed = 180f;
        [SerializeField] float moveThreshold = 1f;
        
	    Vector3 clickPoint;
        NavMeshAgent agent;
        Animator animator;
        Rigidbody myRigidbody;
        
        float turnAmount;
        float forwardAmount;

        void Start()
	    {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyMouseOverPotentiallyWalkableObservers += OnMouseOverPotentiallyWalkable;
            cameraRaycaster.notifyMouseOverEnemyObservers += OnMouseOverEnemy;

            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = true;
            agent.updateRotation = false;
            agent.stoppingDistance = stoppingDistance;

            animator = GetComponent<Animator>();
            animator.applyRootMotion = true;

            myRigidbody = GetComponent<Rigidbody>();
            myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        void Update()
        {
            if(agent.remainingDistance > agent.stoppingDistance)
            {
                Move(agent.desiredVelocity);
            }
            else
            {
                Move(Vector3.zero);
            }
        }

        public void Move(Vector3 move)
        {
            SetForwardAndTurn(move);
            ApplyExtraTurnRotation();
            UpdateAnimator();
        }

        void OnMouseOverPotentiallyWalkable(Vector3 destination)
		{
			if(Input.GetMouseButton(0))
			{
                agent.SetDestination(destination);
			}	
		}
			
		void OnMouseOverEnemy(Enemy enemy)
		{
			if(Input.GetMouseButton(0) || Input.GetMouseButtonDown(1))
			{
                agent.SetDestination(enemy.transform.position);
			}
		}

        public void OnAnimatorMove()
        {
            if(Time.deltaTime > 0)
            {
                Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

                velocity.y = myRigidbody.velocity.y;
                myRigidbody.velocity = velocity;
            }
        }

        private void SetForwardAndTurn(Vector3 movement)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (movement.magnitude > moveThreshold)
            {
                movement.Normalize();
            }
            var localMove = transform.InverseTransformDirection(movement);
            turnAmount = Mathf.Atan2(localMove.x, localMove.z);
            forwardAmount = localMove.z;
        }

        void UpdateAnimator()
        {
            animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
            animator.speed = animationSpeedMultiplier;
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }
    }
}
