using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class ThirdPersonCharacter : MonoBehaviour
    {
        [SerializeField]        float movingTurnSpeed = 360;
        [SerializeField]        float stationaryTurnSpeed = 180;
        [SerializeField]       float runCycleLegOffSet = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
        [SerializeField]        float m_AnimSpeedMultiplier = 1f;
       

        Rigidbody myRigidBody;
        Animator myAnimator;
      
   
      
        float m_TurnAmount;
        float m_ForwardAmount;
  

        void Start()
        {
            myAnimator = GetComponent<Animator>();
            myRigidBody = GetComponent<Rigidbody>();
            myRigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    
        }


        public void Move(Vector3 move)
        {

            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (move.magnitude > 1f) move.Normalize();
            move = transform.InverseTransformDirection(move);

            
            myAnimator.applyRootMotion = true;
            
         
            m_TurnAmount = Mathf.Atan2(move.x, move.z);
            m_ForwardAmount = move.z;

            ApplyExtraTurnRotation();

           

            // send input and other state parameters to the animator
            UpdateAnimator(move);
        }


        void UpdateAnimator(Vector3 move)
        {
            // update the animator parameters
            myAnimator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime); //TODO Check if this can be removed
            myAnimator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime); //TODO Check if this can be removed

           

            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle =
                Mathf.Repeat(
                    myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime + runCycleLegOffSet, 1);

           
        }       

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, m_ForwardAmount);
            transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
        }


        
    }
}
