using UnityEngine.Assertions;
using UnityEngine;
using UnityEngine.Assertions;

using RPG.CameraUI; //for mouse events


//TODO extract weapon system

namespace RPG.Characters
{
    
    public class PlayerMovement : MonoBehaviour
    {
        
       
        [SerializeField] float baseDamage = 10f;

        [Header("Critical Hit System")]
        [Range(.1f, 1.0f)][SerializeField] float criticalHitChance = 0.1f;
        [SerializeField] float criticatHitMultiplier = 1.25f;
        [SerializeField] ParticleSystem criticalHitParticle = null;

        [Header("Weapon In Use")]
        [SerializeField] Weapon currentWeaponConfig = null;

        [Header("Animation Controller")]
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        

       
        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT_ATTACK";


        Character character;
        Enemy enemy = null;
        AudioSource audioSource = null;
        Animator animator = null;
        SpecialAbilities abilities;
       
        CameraRaycaster cameraRayCaster = null;
        float lastHitTime = 0;

        GameObject weaponObject;

        

        public void Start()
        {
            character = GetComponent<Character>();

            abilities = GetComponent<SpecialAbilities>();    

            RegisterForMouseClick();
            PutWeaponInHand(currentWeaponConfig); //TODO Move to weapon system
            SetAttackAnimation(); //TODO Move to weapon system




        }

        public void Update()
        {
            
                ScanForAbilityKeyDown();
         
        }

        private void ScanForAbilityKeyDown()
        {
            for (int keyIndex = 1; keyIndex < abilities.GetNumberOfAbilities(); keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    abilities.AttemptSpecialAbility(keyIndex);
                }
            }
        }

        private void RegisterForMouseClick()
        {

            cameraRayCaster = FindObjectOfType<CameraRaycaster>();
            cameraRayCaster.onMouseOverEnemy += onMouseOverEnemy;
            cameraRayCaster.onMouseOverPotentiallyWalkable += onMouseOverPotentiallyWalkable;

        }

        void onMouseOverPotentiallyWalkable( Vector3 destination)
        {
            if(Input.GetMouseButton(0))
            {
                character.SetDestination(destination);
            }
        }

        void onMouseOverEnemy(Enemy enemyToSet)
        {
            this.enemy = enemyToSet;
            if (Input.GetMouseButton(0) && IsTargetInRange(enemyToSet.gameObject))
            {
                AttackTarget();
            }

            else if (Input.GetMouseButtonDown(1))

            {
                abilities.AttemptSpecialAbility(0);
            }
        }


        //Move to weapon system
        public void PutWeaponInHand (Weapon weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPreFab();
            GameObject weaponSocket = RequestDominantHand();
            //Empty hand
            Destroy(weaponObject);
            weaponObject = Instantiate(weaponPrefab, weaponSocket.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;

           
        }

   

        


      
        private void SetAttackAnimation()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAnimClip(); // remove constant
        }

        //TODO Move to weapon system
        private GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            Assert.AreNotEqual(numberOfDominantHands, 0, "No DominantHand found on player, please add one");
            Assert.IsFalse(numberOfDominantHands > 1, "Multipule dominant hand scrips on player, please remove one");
            return dominantHands[0].gameObject;

        }

      
        

        //TODO use co-routines for move and attack
        private void AttackTarget()
        {

            if (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits())
            {
                SetAttackAnimation();
                animator.SetTrigger(ATTACK_TRIGGER);
                enemy.TakeDamage(CalculateDamage());
               
                lastHitTime = Time.time;
            }
        }

        //TODO Move to weapon system
        private float CalculateDamage()
        {
            bool isCriticalHit = UnityEngine.Random.Range(0f, 1f) <= criticalHitChance;
            float damageBeforeCritical = baseDamage + currentWeaponConfig.GetAdditionalDamage();
            if(isCriticalHit)
            {
                criticalHitParticle.Play();
                return damageBeforeCritical * criticatHitMultiplier;
            }
            else
            {
               
                return damageBeforeCritical;
            }
        }

        bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= currentWeaponConfig.GetMaxAttackRange();
            
        }

        
    }
}