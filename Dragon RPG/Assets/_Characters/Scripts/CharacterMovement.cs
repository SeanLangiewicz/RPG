using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;

namespace RPG.Characters 

{

    [RequireComponent(typeof(NavMeshAgent))]

   
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] float stoppingDistance = 1f;
        [SerializeField] float moveSpeedMultiplier = 1.7f;
        [SerializeField] float animationSpeedMultiplier = 1.5f;
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField]  float stationaryTurnSpeed = 180;
        [SerializeField]  float moveThreashHold = 1f;

        //TODO consider animationspeedmultipler



        float turnAmount;
        float ForwardAmount;

      
        Vector3 clickPoint;
        GameObject walkTarget = null;
        NavMeshAgent agent;
        Animator animator;
        Rigidbody rigidBody;

        void Start()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            
            walkTarget = new GameObject("walkTarget");

            animator = GetComponent<Animator>();
    
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;




            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.stoppingDistance = stoppingDistance;

            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMOuseOverPotentiallyWalkable;
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;

        }


        void Update()
        {
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                Move(agent.desiredVelocity);
            }
            else
            {
                Move(Vector3.zero);
            }
        }

        public void Move(Vector3 movement)
        {
            SetForwardAndTurn(movement);
            ApplyExtraTurnRotation();
            UpdateAnimator();
        }

        public void Kill ()
        {
            // to allow death signaling
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


        private void OnMOuseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                agent.SetDestination(destination);
            }
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                agent.SetDestination(enemy.transform.position);

            }
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

