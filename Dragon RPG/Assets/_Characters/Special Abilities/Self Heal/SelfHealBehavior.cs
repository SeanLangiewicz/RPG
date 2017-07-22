using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehavior : Abilitybehavior
    {
       
        Player player = null;
       
       

        // Use this for initialization
        void Start()
        {
            player = GetComponent<Player>();
            
        }


       
        public override void Use(AbilityUseParams useParams)
        {
            PlayAbilitySound();

            player.Heal((config as SelfHealConfig).GetExtraHealth());
           
            //TODO Find a way to move audio to parent class
            PlayParticleEffect();

        }
        


    }
}
