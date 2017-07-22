using UnityEngine;
using RPG.Characters;
using RPG.Core;
using System;

public class AreaEffectBeahvior : Abilitybehavior
{
    AreaEffectConfig config;
    AudioSource audioSource = null;
    
   
    public void SetConfig(AreaEffectConfig configToSet)
    {
        this.config = configToSet;
    }

	// Use this for initialization
	void Start ()
    {

        audioSource = GetComponent<AudioSource>();

    }
	
	public override void Use(AbilityUseParams useParams)
    {
        DealRadialDamage(useParams);
        PlayParticleEffect();
        audioSource.clip = config.GetAudioClip();
        audioSource.Play();
    }

    private void PlayParticleEffect()
    {
        // TODO decide if particle system attaches to player 
        var particlePrefab = config.GetParticlePreFab();
        var prefab = Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);
       ParticleSystem myParticleSystem = prefab.GetComponent<ParticleSystem>();
        myParticleSystem.Play();
        Destroy(prefab, myParticleSystem.main.duration);
    }

    private void DealRadialDamage(AbilityUseParams useParams)
    {
       
        //Static spehere cast for targets
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, config.GetRadius(), Vector3.up, config.GetRadius());
        //for each hit

        foreach (RaycastHit hit in hits)
        {
            var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
            bool hitPlayer = hit.collider.gameObject.GetComponent <Player>();
            if (damageable != null && !hitPlayer)
            {
                float damageToDeal = useParams.baseDamage + config.GetDamageToEachTarget();//TODO is this right ?

                damageable.TakeDamage(damageToDeal);
            }
        }
    }
}
