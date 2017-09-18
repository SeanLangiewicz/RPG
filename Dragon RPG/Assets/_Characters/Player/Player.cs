using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

//TODO condier re-wire
using RPG.CameraUI; 
using RPG.Core; 


namespace RPG.Characters
{

    public class Player : MonoBehaviour
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
        

       
        

      


     

        // Temporarily serialized for debugging
        [Header("Player Abilities(Remove Later)")] [SerializeField] AbilityConfig[] abilities = null;

       
        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT_ATTACK";



        Enemy enemy = null;
        AudioSource audioSource = null;
        Animator animator = null;
       
        CameraRaycaster cameraRayCaster = null;
        float lastHitTime = 0;

        GameObject weaponObject;

        

        public void Start()
        {
                     

            RegisterForMouseClick();
            PutWeaponInHand(currentWeaponConfig);
            SetAttackAnimation();
            AttachInitialAbilities();
            
           

        }

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

        private void AttachInitialAbilities()
        {
            for(int abilityIndex =0;abilityIndex < abilities.Length;abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo(gameObject);
            }
            
        }

        public void Update()
        {
            var healthPercentage = GetComponent<HealthSystem>().healthAsPercentage;
             if (healthPercentage >Mathf.Epsilon)
            {
                ScanForAbilityKeyDown();
            }
        }

        private void ScanForAbilityKeyDown()
        {
            for(int keyIndex =1; keyIndex <abilities.Length; keyIndex++)
            {
                if(Input.GetKeyDown(keyIndex.ToString()))
                    {
                    AttemptSpecialAbility(keyIndex);
                    }
            }
        }



        private void SetAttackAnimation()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAnimClip(); // remove constant
        }

        private GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            Assert.AreNotEqual(numberOfDominantHands, 0, "No DominantHand found on player, please add one");
            Assert.IsFalse(numberOfDominantHands > 1, "Multipule dominant hand scrips on player, please remove one");
            return dominantHands[0].gameObject;

        }

        private void RegisterForMouseClick()
        {
            
            cameraRayCaster = FindObjectOfType<CameraRaycaster>();
            cameraRayCaster.onMouseOverEnemy += onMouseOverEnemy;
            
        }
        
        void onMouseOverEnemy (Enemy enemyToSet)
        {
            this.enemy = enemyToSet;
            if (Input.GetMouseButton(0) && IsTargetInRange(enemyToSet.gameObject))
            {
                AttackTarget();
            }

            else if (Input.GetMouseButtonDown(1))
                
            {
                AttemptSpecialAbility(0);
            }
        }

        private void AttemptSpecialAbility(int abilityIndex)
        {
            var energyComponent = GetComponent<Energy>();
            var energyCost = abilities[abilityIndex].GetEnergyCost();
            if(energyComponent.IsEnergyAvailable(energyCost))//TODO read from Scriptable object
            {
                energyComponent.CosnsumeEnergy(energyCost);
                var abilityParams = new AbilityUseParams(enemy, baseDamage);
                abilities[abilityIndex].Use(abilityParams);
            }

            
        }

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

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= currentWeaponConfig.GetMaxAttackRange();
            
        }

        
    }
}