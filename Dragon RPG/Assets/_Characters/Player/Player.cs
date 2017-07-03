using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

//TODO condier re-wire
using RPG.CameraUI; 
using RPG.Core; 
using RPG.Weapons; 

namespace RPG.Characters
{

    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float baseDamage = 10f;
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        [SerializeField] Weapon weaponinUse = null;
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField]  AudioClip[] deathSounds;

        AudioSource audioSource;

        // Temporarily serialized for debugging
        [SerializeField] SpecialAbility[] abilities = null;
        

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
            abilities[0].AttachComponentTo(gameObject);
            audioSource = GetComponent<AudioSource>();

        }

        public void TakeDamage(float damage)
        {
            ReduceHealth(damage);
            audioSource.clip = damageSounds[UnityEngine.Random.Range(0,damageSounds.Length)];
            audioSource.Play();
            bool playerDies = (currentHealthPoints - damage <= 0);
            if (playerDies)
            {
                StartCoroutine(KillPlayer());
            }

        }

        IEnumerator KillPlayer()
        {

            Debug.Log("Death sound");
            audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
            audioSource.Play();
            Debug.Log("Death animation");
            yield return new WaitForSecondsRealtime(audioSource.clip.length); 
            SceneManager.LoadScene(0);
                     
        }

        private void ReduceHealth(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            //Play sound
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
                AttemptSpecialAbility(0,enemy);
            }
        }

        private void AttemptSpecialAbility(int abilityIndex, Enemy enemy)
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

        private void AttackTarget(Enemy enemy)
        {

            if (Time.time - lastHitTime > weaponinUse.GetMinTimeBetweenHits())
            {
                animator.SetTrigger("Attack"); // TODO Make const
                enemy.TakeDamage(baseDamage);
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