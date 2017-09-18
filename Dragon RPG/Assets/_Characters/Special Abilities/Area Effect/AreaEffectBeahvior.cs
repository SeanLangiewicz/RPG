using UnityEngine;
using RPG.Characters;
using RPG.Core;
using System;

public class AreaEffectBeahvior : Abilitybehavior
{
	
	public override void Use(GameObject target)
    {
        PlayAbilitySound();
        DealRadialDamage();
        PlayParticleEffect();

       
    }

    

    private void DealRadialDamage()
    {
       
        //Static spehere cast for targets
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, (config as AreaEffectConfig).GetRadius(), Vector3.up, (config as AreaEffectConfig).GetRadius());
        //for each hit

        foreach (RaycastHit hit in hits)
        {
            var damageable = hit.collider.gameObject.GetComponent<HealthSystem>();
            bool hitPlayer = hit.collider.gameObject.GetComponent <PlayerMovement>();
            if (damageable != null && !hitPlayer)
            {
                float damageToDeal = (config as AreaEffectConfig).GetDamageToEachTarget();

                damageable.TakeDamage(damageToDeal);
            }
        }
    }
}
