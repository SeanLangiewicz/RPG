using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Area Effect"))]

    public class AreaEffectConfig : AbilityConfig
    {
        [Header("Area Effect Specific")]
        [SerializeField] float radius = 5f;
        [SerializeField] float damageToEachTarget = 15;
        
       


        public override void AttachComponentTo(GameObject gameObjectToattachTo)
        {
            var behaviorComponent = gameObjectToattachTo.AddComponent<AreaEffectBeahvior>();
            behaviorComponent.SetConfig(this);
            behavior=  behaviorComponent;
            
        }
       public float GetDamageToEachTarget()
        {
            return damageToEachTarget;
        }
        public float GetRadius()
        {
            return radius;
        }
    }
    

}
