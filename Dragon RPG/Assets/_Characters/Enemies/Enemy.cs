using System.Collections;
using System.Collections.Generic;
using UnityEngine;



using RPG.Weapons; // consider re-wire
using RPG.Core; // Condier re-wire

namespace RPG.Characters

{

    public class Enemy : MonoBehaviour, IDamageable
    {
        [Header("Base Stats")]
        [SerializeField] float maxHealthPoints = 100f;

        [Header("Damage Tuning")]
        [SerializeField] float damagePerShot = 9f;
        [SerializeField] float firingPeriodInS = 0.5f;
        [SerializeField] float firingPeriodV = 0.1f;

        [Header("Chase / Attack ")]
        [SerializeField] float chaseRaidus = 6f;
        [SerializeField] float attackRadius = 6f;

        [Header("Projectile")]
        [SerializeField] Vector3 aimOffset = new Vector3(0, 1f, 0);
        [SerializeField] GameObject projectileToUse = null;
        [SerializeField] GameObject projectileSocket = null;

        bool isAttacking = false;

        const string TRIGGER_DEATH = "Death";
        Animator animator = null;

        float currentHealthPoints;
        AICharacterControl aiCharacterControl = null;
        Player player = null;

        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / (float)maxHealthPoints;
            }
        }
        void Start()
        {
            player = FindObjectOfType<Player>();
            aiCharacterControl = GetComponent<AICharacterControl>();
            currentHealthPoints = maxHealthPoints;
        }

        void Update()
        {
            if(player.healthAsPercentage <= Mathf.Epsilon)
            {
                StopAllCoroutines();
                // To stop enemy behaviour
                Destroy(this); 
            }
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayer <= attackRadius && !isAttacking)
            {
                //TODO Slow this down
                isAttacking = true;
                float randomizedDelay = Random.Range(firingPeriodInS-firingPeriodV,firingPeriodInS+firingPeriodV);
                InvokeRepeating("FireProjectile", 0f, randomizedDelay);

            }
            if (distanceToPlayer > attackRadius)
            {
                isAttacking = false;
                CancelInvoke();
            }
            if (distanceToPlayer <= chaseRaidus)
            {
                aiCharacterControl.SetTarget(player.transform);

            }
            else
            {
                aiCharacterControl.SetTarget(transform);
            }

        }

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            if (currentHealthPoints <= 0)
            {
               
                Destroy(gameObject);
            }
        }
        // TODO separate out character firing logic
        void FireProjectile()
        {
            GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity);
            Projectile projectileComponent = newProjectile.GetComponent<Projectile>();
            projectileComponent.SetDamage(damagePerShot);
            projectileComponent.SetShooter(gameObject);

            Vector3 unitVectorToPlayer = (player.transform.position + aimOffset - projectileSocket.transform.position).normalized;
            float projectileSpeed = projectileComponent.GetFaultLaunchSpeed();
            newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
        }

        void OnDrawGizmos()
        {
            //Draw Attack Sphere
            Gizmos.color = new Color(255f, 0f, 0, .5f);
            Gizmos.DrawWireSphere(transform.position, attackRadius);

            //Draw Chase spehere
            Gizmos.color = new Color(0f, 0f, 255, .5f);
            Gizmos.DrawWireSphere(transform.position, chaseRaidus);
        }

    }
}