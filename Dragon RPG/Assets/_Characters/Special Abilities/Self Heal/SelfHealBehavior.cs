using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehavior : Abilitybehavior
    {
       
        PlayerMovement player = null;
       
       

        // Use this for initialization
        void Start()
        {
            player = GetComponent<PlayerMovement>();
            
        }


       
        public override void Use(GameObject target)
        {
            PlayAbilitySound();

            var playerHealth = player.GetComponent<HealthSystem>();
            playerHealth.Heal((config as SelfHealConfig).GetExtraHealth());
           
            //TODO Find a way to move audio to parent class
            PlayParticleEffect();

        }
        


    }
}
