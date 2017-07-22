using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class Abilitybehavior : MonoBehaviour
    {
        protected AbilityConfig config;

        float PARTICLE_CLEAN_UP_DELAY = 5;

        public abstract void Use(AbilityUseParams useParams);


        public void SetConfig(AbilityConfig configToSet)
        {
            config = configToSet;
        }

        protected void PlayParticleEffect()
        {
            var particlePrefab = config.GetParticlePreFab();
            var particleObject = Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);

            particleObject.transform.parent = transform; // set world space in prefab is required
            particleObject.GetComponent<ParticleSystem>().Play();
            StartCoroutine(DestoryParticleWhenFinished(particleObject));
            
           
        }
        IEnumerator DestoryParticleWhenFinished(GameObject particlePrefab)
        {
            while(particlePrefab.GetComponent<ParticleSystem>().isPlaying)
            {
                yield return new WaitForSeconds(PARTICLE_CLEAN_UP_DELAY);
            }
            Destroy(particlePrefab);
            yield return new WaitForEndOfFrame();
        }

        protected void PlayAbilitySound()
        {
            var abilitySound = config.GetRandomAblitySound(); 
            var audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(abilitySound);
        }
    }
}
