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
    
    public abstract class SpecialAbility : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;

        protected ISpecialAbility behavior;

       

        abstract public void AttachComponentTo (GameObject gameObjectToattachTo);

        public void Use(AbilityUseParams useParams)
        {
            behavior.Use(useParams);
        }
        public float GetEnergyCost()
        {
            return energyCost;
        }

    }
    public interface ISpecialAbility
    {
        void Use(AbilityUseParams useParams);
    }
}
