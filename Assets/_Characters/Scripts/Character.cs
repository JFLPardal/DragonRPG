using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;

namespace RPG.Characters
{
    [SelectionBase]
	public class Character : MonoBehaviour
	{
        [Header("Animator")]
        [SerializeField] RuntimeAnimatorController animatorController;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] Avatar avatar;

        [Header("Capsule Collider")]
        [SerializeField] Vector3 colliderCenter;
        [SerializeField] float colliderRadius;
        [SerializeField] float colliderHeight;

        [Header("Nav Mesh Agent")]
        [Header("Steering")]
        [SerializeField] float speed = 1f;
        [SerializeField] float angularSpeed;
        [SerializeField] float acceleration;
        [Header("Obstacle Avoidance")]
        [SerializeField] float radius;
        [SerializeField] float height;
        [SerializeField] float stoppingDistance = 1.3f;
        
        [Header("Movement")]
        [SerializeField] float moveSpeedMultiplier = .7f;
        [SerializeField] float movingTurnSpeed = 360f;
        [SerializeField] float animationSpeedMultiplier = 1.5f;
        [SerializeField] float stationaryTurnSpeed = 180f;
        [SerializeField] float moveThreshold = 1f;

        [Header("Audio")]
        [SerializeField] float spatialBlend = 0.5f;
        
        NavMeshAgent navMeshAgent;
        Animator animator;
        Rigidbody myRigidbody;
        bool isAlive = true;
        
        float turnAmount;
        float forwardAmount;

        void Awake()
        {
            AddRequiredComponents();
        }

        private void AddRequiredComponents()
        {
            var capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = colliderCenter;
            capsuleCollider.radius = colliderRadius;
            capsuleCollider.height = colliderHeight;

            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.speed = speed;
            navMeshAgent.angularSpeed = angularSpeed;
            navMeshAgent.acceleration = acceleration;
            navMeshAgent.stoppingDistance = stoppingDistance;
            navMeshAgent.radius = radius;
            navMeshAgent.height = height;
            navMeshAgent.updatePosition = true;
            navMeshAgent.updateRotation = false;
            navMeshAgent.autoBraking = false;

            animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            animator.avatar = avatar;
            animator.applyRootMotion = true;

            myRigidbody = gameObject.AddComponent<Rigidbody>();
            myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = spatialBlend;

        }
        
        void Update()
        {
            if(navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance && isAlive)
            {
                Move(navMeshAgent.desiredVelocity);
            }
            else
            {
                Move(Vector3.zero);
            }
        }

        public void SetDestination(Vector3 worldPos)
        {
            navMeshAgent.destination = worldPos;
        }

        public AnimatorOverrideController GetOverrideController()
        {
            return animatorOverrideController;
        }

        void Move(Vector3 move)
        {
            SetForwardAndTurn(move);
            ApplyExtraTurnRotation();
            UpdateAnimator();
        }

        public void Kill()
        {
            isAlive = false;
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
            if (movement.magnitude >= moveThreshold)
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
