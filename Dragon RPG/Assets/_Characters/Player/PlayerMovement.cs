//using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI; // TODO Consider re-wiring

namespace RPG.Characters 

{

    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AICharacterControl))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class PlayerMovement : MonoBehaviour
    {

        ThirdPersonCharacter thirdPersonCharacter;   // A reference to the ThirdPersonCharacter on the object
        CameraRaycaster cameraRaycaster;
        Vector3 clickPoint;
        AICharacterControl aiCharacterControl = null;
        GameObject walkTarget = null;

        void Start()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
            aiCharacterControl = GetComponent<AICharacterControl>();
            walkTarget = new GameObject("walkTarget");
            
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMOuseOverPotentiallyWalkable;
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
                
          }



        private void OnMOuseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                walkTarget.transform.position = destination;
                aiCharacterControl.SetTarget(walkTarget.transform);
            }
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if(Input.GetMouseButton(0)|| Input.GetMouseButton(1)) 
            {
                aiCharacterControl.SetTarget(enemy.transform);
               
            }
        }

        

       

        // TODO make this get called again
        void ProcessDirectMovemment()
        {


            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // calculate camera relative direction to move:
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 movement = v * cameraForward + h * Camera.main.transform.right;

            thirdPersonCharacter.Move(movement, false, false);

        }
    }

}

