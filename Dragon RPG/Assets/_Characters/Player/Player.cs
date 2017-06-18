using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//TODO condier re-wire
using RPG.CameraUI; 
using RPG.Core; 
using RPG.Weapons; 

namespace RPG.Characters
{

    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float damagePerHit = 10f;
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        [SerializeField] Weapon weaponinUse;

        // Temporarily serialized for debugging
        [SerializeField] SpecialAbilityConfig ability1;


        Animator animator;
        float currentHealthPoints;
        CameraRaycaster cameraRayCaster;

        float lastHitTime = 0f;

        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / (float)maxHealthPoints;
            }
        }

        public void Start()
        {
            RegisterForMouseClick();
            SetCurrentMaxHealth();
            PutWeaponInHand();
            SetupRunTimeAnimator();
            ability1.AddComponent(gameObject);

        }

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            if (currentHealthPoints <= 0)
            {

                //        Destroy(gameObject);
            }
        }

        private void SetCurrentMaxHealth()
        {
            currentHealthPoints = maxHealthPoints;
        }

        private void SetupRunTimeAnimator()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController["DEFAULT ATTACK"] = weaponinUse.GetAnimClip(); // remove constant
        }

        private void PutWeaponInHand()
        {

            var weaponPrefab = weaponinUse.GetWeaponPreFab();
            GameObject weaponSocket = RequestDominantHand();
            var weapon = Instantiate(weaponPrefab, weaponSocket.transform);
            weapon.transform.localPosition = weaponinUse.gripTransform.localPosition;
            weapon.transform.localRotation = weaponinUse.gripTransform.localRotation;



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
        
        void onMouseOverEnemy (Enemy enemy)
        {
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                AttackTarget(enemy);
            }

            else if (Input.GetMouseButtonDown(1))
            {
                AttemptSpecialAbility1(enemy);
            }
        }

        private void AttemptSpecialAbility1(Enemy enemy)
        {
            var energyComponent = GetComponent<Energy>();
            if(energyComponent.IsEnergyAvailable(10f))//TODO read from Scriptable object
            {
                energyComponent.CosnsumeEnergy(10f);
                //TODO Use the ability
            }

            
        }

        private void AttackTarget(Enemy enemy)
        {

            if (Time.time - lastHitTime > weaponinUse.GetMinTimeBetweenHits())
            {
                animator.SetTrigger("Attack"); // TODO Make const
                enemy.TakeDamage(damagePerHit);
                lastHitTime = Time.time;
            }
        }

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= weaponinUse.GetMaxAttackRange();
            
        }

        
    }
}