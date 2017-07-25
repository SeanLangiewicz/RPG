using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public struct AbilityUseParams
    {
        public IDamageable target;
        public float baseDamage;

        //Constructor
        public AbilityUseParams(IDamageable target, float baseDamage)
        {
            this.target = target;
            this.baseDamage = baseDamage;
        }
    }
    
    public abstract class AbilityConfig : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;
        [SerializeField] GameObject particlePrefab = null;
        [SerializeField] AudioClip[] audioClips = null;

        protected Abilitybehavior behavior;

        public abstract Abilitybehavior GetBehaviorComponent(GameObject objectToAttachTo);



        public void AttachAbilityTo (GameObject objectToAttachTo)
        {
            Abilitybehavior behaviorComponent = GetBehaviorComponent(objectToAttachTo);
            behaviorComponent.SetConfig(this);
            behavior = behaviorComponent;
        }

        public void Use(AbilityUseParams useParams)
        {
            behavior.Use(useParams);
        }
        public float GetEnergyCost()
        {
            return energyCost;
        }
        public GameObject GetParticlePreFab()
        {
            return particlePrefab;
        }

        public AudioClip GetRandomAblitySound()
        {
            return audioClips[Random.Range (0,audioClips.Length)];
        }

    }
   
}
