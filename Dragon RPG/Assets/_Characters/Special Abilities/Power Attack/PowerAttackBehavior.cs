using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehavior : MonoBehaviour, ISpecialAbility
    {
        PowerAttackConfig config;

        public void SetConfig(PowerAttackConfig configToSet)
        {
            this.config = configToSet;
        }
        // Use this for initialization
        void Start()
        {
            print("Power attack behavior attached to " + gameObject.name);
            
        }

        // Update is called once per frame
        void Update()
        {

        }
        
        public void Use(AbilityUseParams useParams)
        {
            DealDamage(useParams);
            PlayParticleEffect();
        }

        private void PlayParticleEffect()
        {
            // TODO decide if particle system attaches to player 
            var prefab = Instantiate(config.GetParticlePreFab(), transform.position, Quaternion.identity);
            ParticleSystem myParticleSystem = prefab.GetComponent<ParticleSystem>();
            myParticleSystem.Play();
            Destroy(prefab, myParticleSystem.main.duration);
        }

        private void DealDamage(AbilityUseParams useParams)
        {
            print("Power Attack used by:" + gameObject.name);
            float damageToDeal = useParams.baseDamage + config.GetExtraDamage();
            useParams.target.AdjustHealth(damageToDeal);
        }
    }
}
