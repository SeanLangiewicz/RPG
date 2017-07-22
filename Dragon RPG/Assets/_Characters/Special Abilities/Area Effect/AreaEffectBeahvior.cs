using UnityEngine;
using RPG.Characters;
using RPG.Core;
using System;

public class AreaEffectBeahvior : Abilitybehavior
{
	
	public override void Use(AbilityUseParams useParams)
    {
        PlayAbilitySound();
        DealRadialDamage(useParams);
        PlayParticleEffect();

       
    }

    

    private void DealRadialDamage(AbilityUseParams useParams)
    {
       
        //Static spehere cast for targets
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, (config as AreaEffectConfig).GetRadius(), Vector3.up, (config as AreaEffectConfig).GetRadius());
        //for each hit

        foreach (RaycastHit hit in hits)
        {
            var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
            bool hitPlayer = hit.collider.gameObject.GetComponent <Player>();
            if (damageable != null && !hitPlayer)
            {
                float damageToDeal = useParams.baseDamage + (config as AreaEffectConfig).GetDamageToEachTarget();//TODO is this right ?

                damageable.TakeDamage(damageToDeal);
            }
        }
    }
}
