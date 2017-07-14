using UnityEngine;
using RPG.Characters;
using RPG.Core;
using System;

public class AreaEffectBeahvior : MonoBehaviour , ISpecialAbility
{
    AreaEffectConfig config;
    
   
    public void SetConfig(AreaEffectConfig configToSet)
    {
        this.config = configToSet;
    }

	// Use this for initialization
	void Start ()
    {
       
        print("Area Effect behavior attached to " + gameObject.name);

    }
	
	public void Use(AbilityUseParams useParams)
    {
        DealRadialDamage(useParams);
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

    private void DealRadialDamage(AbilityUseParams useParams)
    {
        print("Area Effect used by  " + gameObject.name);
        //Static spehere cast for targets
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, config.GetRadius(), Vector3.up, config.GetRadius());
        //for each hit

        foreach (RaycastHit hit in hits)
        {
            var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float damageToDeal = useParams.baseDamage + config.GetDamageToEachTarget();//TODO is this right ?

                damageable.AdjustHealth(damageToDeal);
            }
        }
    }
}
