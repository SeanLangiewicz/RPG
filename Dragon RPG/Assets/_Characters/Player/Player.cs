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

    public class Player : MonoBehaviour, IDamageable
    {
        [Header("General Stats")]
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float baseDamage = 10f;

        [Header("Critical Hit System")]
        [Range(.1f, 1.0f)][SerializeField] float criticalHitChance = 0.1f;
        [SerializeField] float criticatHitMultiplier = 1.25f;
        [SerializeField] ParticleSystem criticalHitParticle = null;

        [Header("Weapon In Use")]
        [SerializeField] Weapon currentWeaponConfig = null;

        [Header("Animation Controller")]
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        

        [Header("Player Sounds")]
        [SerializeField] AudioClip[] damageSounds = null;
        [SerializeField]  AudioClip[] deathSounds = null;

      


     

        // Temporarily serialized for debugging
        [Header("Player Abilities(Remove Later)")] [SerializeField] AbilityConfig[] abilities = null;

        const string TRIGGER_DEATH = "Death";
        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT_ATTACK";



        Enemy enemy = null;
        AudioSource audioSource = null;
        Animator animator = null;
        float currentHealthPoints = 0;
        CameraRaycaster cameraRayCaster = null;
        float lastHitTime = 0;

        GameObject weaponObject;

        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / (float)maxHealthPoints;
            }
        }

        public void Start()
        {
            audioSource = GetComponent<AudioSource>();
            

            RegisterForMouseClick();
            SetCurrentMaxHealth();
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
             if (healthAsPercentage >Mathf.Epsilon)
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

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            audioSource.clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
            audioSource.Play();
            
            if (currentHealthPoints <= 0)
            {
                StartCoroutine(KillPlayer());
            }

        }
        public void Heal (float points)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + points, 0f, maxHealthPoints);
        }


        IEnumerator KillPlayer()
        {

            animator.SetTrigger("Death");
            GetComponent<AICharacterControl>().enabled = false;
            Debug.Log("movement disabled");
            Debug.Log("Death animation");
            audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
            audioSource.Play();
            yield return new WaitForSecondsRealtime(audioSource.clip.length +3);
           
            SceneManager.LoadScene(0);
                     
        }



        private void SetCurrentMaxHealth()
        {
            currentHealthPoints = maxHealthPoints;
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