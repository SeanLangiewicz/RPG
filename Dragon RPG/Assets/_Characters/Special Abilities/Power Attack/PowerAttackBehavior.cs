using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehavior : MonoBehaviour, ISpecialAbility
    {
        PowerAttackConfig config;

        public void SetConfig(PowerAttackConfig configToSet)
        {
            this.config = configToSet;
        }
        // Use this for initialization
        void Start()
        {
            print("Power attack behavior attached" + gameObject.name);
        }

        // Update is called once per frame
        void Update()
        {

        }
        
        public void Use()
        {
            print("Power attack used");   
        }
    }
}
