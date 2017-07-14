using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehavior : MonoBehaviour, ISpecialAbility
    {
        SelfHealConfig config;
        Player player = null;

        public void SetConfig(SelfHealConfig configToSet)
        {
            this.config = configToSet;
        }

        // Use this for initialization
        void Start()
        {
            player = GetComponent<Player>();
        }

        // Update is called once per frame
        void Update()
        {

        }

       
        public void Use(AbilityUseParams useParams)
        {
            print("Self heal used by : " + gameObject.name);

            player.AdjustHealth(-config.GetExtraHealth());


        }


    }
}
