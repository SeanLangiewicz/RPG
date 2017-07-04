using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;
using RPG.Core;

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
       
        //print("Area Effect behavior attached to " + gameObject.name);

    }
	
	public void Use(AbilityUseParams useParams)
    {
        print("Area Effect used by  " + gameObject.name);
        //Static spehere cast for targets
        RaycastHit[] hits = Physics.SphereCastAll(transform.position,config.GetRadius(),Vector3.up,config.GetRadius());
        //for each hit

        foreach(RaycastHit hit in hits)
        {
            var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
            if(damageable != null)
            {
                float damageToDeal = useParams.baseDamage + config.GetDamageToEachTarget();//TODO is this right ?
               
                damageable.TakeDamage(damageToDeal);
            }
        }
            //if damage
                //Deal damage to target + player base damage\
            
       
    }
}
