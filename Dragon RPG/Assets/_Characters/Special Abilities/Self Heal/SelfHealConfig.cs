﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Self Heal"))]

    public class SelfHealConfig : SpecialAbility
    {
        [Header("Self Heal Specific")]
        [SerializeField] float extraHealth = 50f;


        public override void AttachComponentTo(GameObject gameObjectToattachTo)
        {
            var behaviorComponent = gameObjectToattachTo.AddComponent<SelfHealBehavior>();
            behaviorComponent.SetConfig(this);
            behavior = behaviorComponent;
        }
        public float GetExtraHealth()
        {
            return extraHealth;
        }
    }

}
