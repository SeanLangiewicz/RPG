//using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;

namespace RPG.Characters 

{

    [RequireComponent(typeof(NavMeshAgent))]

    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] float stoppingDistance = 1f;
        [SerializeField] float moveSpeedMultiplier = .7f;
        //TODO consider animationspeedmultipler

        ThirdPersonCharacter character;   // A reference to the ThirdPersonCharacter on the object

        Vector3 clickPoint;
        GameObject walkTarget = null;
        NavMeshAgent agent;
        Animator animator;
        Rigidbody myRigidbody;

        void Start()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            character = GetComponent<ThirdPersonCharacter>();
            walkTarget = new GameObject("walkTarget");

            animator = GetComponent<Animator>();
            myRigidbody = GetComponent<Rigidbody>();




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
                character.Move(agent.desiredVelocity);
            }
            else
            {
                character.Move(Vector3.zero);
            }
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

                velocity.y = myRigidbody.velocity.y;
                myRigidbody.velocity = velocity;
            }
        }
    }
    

}

