using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehavior : MonoBehaviour, ISpecialAbility
    {
        PowerAttackConfig config;
        AudioSource audioSource = null;

        public void SetConfig(PowerAttackConfig configToSet)
        {
            this.config = configToSet;
        }
        // Use this for initialization
        void Start()
        {
           
            
        }

        // Update is called once per frame
        void Update()
        {

        }
        
        public void Use(AbilityUseParams useParams)
        {
            DealDamage(useParams);
            PlayParticleEffect();
            audioSource.clip = config.GetAudioClip();
            audioSource.Play();
        }

        private void PlayParticleEffect()
        {
            var prefab = Instantiate(config.GetParticlePreFab(), transform.position, Quaternion.identity);
            // TODO decide if particle system attaches to player 
            ParticleSystem myParticleSystem = prefab.GetComponent<ParticleSystem>();
            myParticleSystem.Play();
            Destroy(prefab, myParticleSystem.main.duration);
        }

        private void DealDamage(AbilityUseParams useParams)
        {
            
            float damageToDeal = useParams.baseDamage + config.GetExtraDamage();
            useParams.target.TakeDamage(damageToDeal);
        }
    }
}
