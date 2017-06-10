using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO Consider re-wire

using RPG.Core;
namespace RPG.Weapons
{

    public class Projectile : MonoBehaviour
    {


        [SerializeField] float projectileSpeed; // Note other classes set
        [SerializeField] GameObject shooter; // So can inspect when paused.

        const float DESTORY_DELAY = 0.01f;
        private float damageCaused;



        public void SetShooter(GameObject shooter)
        {
            this.shooter = shooter;
        }
        public void SetDamage(float damage)
        {
            damageCaused = damage;
        }

        public float GetFaultLaunchSpeed()
        {
            return projectileSpeed;
        }

        void OnCollisionEnter(Collision collison)
        {
            var layerCollidedWith = collison.gameObject.layer;
            if (shooter && layerCollidedWith != shooter.layer)
            {
                DamageDamageable(collison);

            }


        }

        private void DamageDamageable(Collision collison)
        {
            Component damageableComponent = collison.gameObject.GetComponent(typeof(IDamageable));

            if (damageableComponent)
            {
                (damageableComponent as IDamageable).TakeDamage(damageCaused);
            }
            Destroy(gameObject, DESTORY_DELAY);
        }
    }
}
