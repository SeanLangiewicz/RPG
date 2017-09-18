﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Characters
{
    public class HealthSystem : MonoBehaviour
    {


        [SerializeField]     float maxHealthPoints = 100f;
        [SerializeField]     Image healthBar;
        [SerializeField]     float deathVanishSeconds = 2.0f;
        [SerializeField]     AudioClip[] damageSounds = null;
        [SerializeField]     AudioClip[] deathSounds = null;
    
    

        const string TRIGGER_DEATH = "Death";

       float currentHealthPoints;
        Animator animator;
        AudioSource audioSource;
        Character characterMovement;


        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / (float)maxHealthPoints;
            }
        }

        void Start()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            characterMovement = GetComponent<Character>();

            currentHealthPoints = maxHealthPoints;

        }

        void Update()
        {
            UpdateHealthBar();
        }

        void UpdateHealthBar()
        {
          if(healthBar) // Enemies may not have health bars to update
            {
                healthBar.fillAmount = healthAsPercentage;
            }
        }

        public void TakeDamage(float damage)
        {
            bool characterDies = (currentHealthPoints - damage <= 0);
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            var clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
            audioSource.PlayOneShot(clip);

            if (characterDies)
            {
                StartCoroutine(KillCharacter());
            }

        }

        public void Heal(float points)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + points, 0f, maxHealthPoints);
        }


        IEnumerator KillCharacter()
        {
            StopAllCoroutines();
            characterMovement.Kill();
            animator.SetTrigger("Death");

            var playerComponent = GetComponent<Player>();
            if(playerComponent && playerComponent.isActiveAndEnabled)// relying on lazy evaluation
            {
                audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
                audioSource.Play(); // overriding any existing sounds
                yield return new WaitForSecondsRealtime(audioSource.clip.length + 3);
            }
            else // assume is enemy for now, reconsider on NPCs
            {
                DestroyObject(gameObject, deathVanishSeconds);
            }
           

            SceneManager.LoadScene(0);

        }

    }
}
