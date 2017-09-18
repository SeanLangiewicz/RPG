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
        [SerializeField]  RuntimeAnimatorController animatorController;
        [SerializeField]  AnimatorOverrideController animatorOverrideController;
        [SerializeField]  Avatar characterAvatar;

        [Header("Audio Source")]
        [SerializeField]   float audioSourceSpatialBlend = .5f;
        [Range(0, 1f)]  [SerializeField]     float audioSourceVolume;


        [Header("Capsule Collider ")]
        [SerializeField] Vector3 colliderCenter = new Vector3(0, 1.03f, 0);
        [SerializeField] float colliderRadius = 0.2f;
        [SerializeField] float colliderHeight = 2.3f;

        [Header("Movement")]
        [SerializeField] float stoppingDistance = 1f;
        [SerializeField] float moveSpeedMultiplier = 1.7f;
        [SerializeField] float animationSpeedMultiplier = 1.5f;
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField]  float stationaryTurnSpeed = 180;
        [SerializeField]  float moveThreashHold = 1f;

        [Header("Nav Mesh Agent")]
        [SerializeField]  float navMeshAgentSteeringSpeed = 1.0f;
        [SerializeField]  float navMeshAgentStoppingSpeed = 1.3f;
       



        float turnAmount;
        float ForwardAmount;
        bool isAlive = true;

      
      
        GameObject walkTarget = null;
        NavMeshAgent navMeshAgent;
        Animator animator;
        Rigidbody rigidBody;
        

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

            animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            animator.avatar = characterAvatar;

            rigidBody = gameObject.AddComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = audioSourceSpatialBlend;
            audioSource.volume = audioSourceVolume;

            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.speed = navMeshAgentSteeringSpeed;
            navMeshAgent.stoppingDistance = navMeshAgentStoppingSpeed;
            navMeshAgent.autoBraking = true;
            navMeshAgent.updatePosition = false;
            navMeshAgent.updateRotation = true;
        
        }

      

        void Update()
        {
            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance && isAlive)
            {
                Move(navMeshAgent.desiredVelocity);
            }
            else
            {
                Move(Vector3.zero);
            }
        }

        public void Kill()
        {
            isAlive = false;
        }


        public void SetDestination(Vector3 worldPos)
        {
            navMeshAgent.destination = worldPos;
        }

        void Move(Vector3 movement)
        {
            SetForwardAndTurn(movement);
            ApplyExtraTurnRotation();
            UpdateAnimator();
        }

       

        private void SetForwardAndTurn(Vector3 movement)
        {

            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired direction.

            if (movement.magnitude > moveThreashHold)
            {
                movement.Normalize();
            }

            var localMove = transform.InverseTransformDirection(movement);

            turnAmount = Mathf.Atan2(localMove.x, localMove.z);
            ForwardAmount = localMove.z;

        }

        void UpdateAnimator()
        {
            animator.SetFloat("Forward", ForwardAmount, 0.1f, Time.deltaTime); //TODO Check if this can be removed
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime); //TODO Check if this can be removed
            animator.speed = animationSpeedMultiplier;

        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, ForwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }


        void OnAnimatorMove()
        {
            if(Time.deltaTime >0)
            {
                Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

                velocity.y = rigidBody.velocity.y;
                rigidBody.velocity = velocity;
            }
        }
    }
    
}

