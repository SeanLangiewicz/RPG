﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehavior : Abilitybehavior
    {
       
        

        public void SetConfig(PowerAttackConfig configToSet)
        {
            this.config = configToSet;
        }
      
        
        public override void Use(GameObject target)
        {
            PlayAbilitySound();
            DealDamage(target);
            PlayParticleEffect();
        }

       

        private void DealDamage(GameObject target)
        {
            
            float damageToDeal = (config as PowerAttackConfig).GetExtraDamage();
            target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
        }
    }
}
