using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehavior : MonoBehaviour, ISpecialAbility
    {
        SelfHealConfig config = null;
        Player player = null;
        AudioSource audioSource = null;

        public void SetConfig(SelfHealConfig configToSet)
        {
            this.config = configToSet;
        }

        // Use this for initialization
        void Start()
        {
            player = GetComponent<Player>();
            audioSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {

        }

       
        public void Use(AbilityUseParams useParams)
        {


            player.Heal(config.GetExtraHealth());
            audioSource.clip = config.GetAudioClip();
            audioSource.Play();
            //TODO Find a way to move audio to parent class
            PlayParticleEffect();

        }
        private void PlayParticleEffect()
        {
            var prefab = Instantiate(config.GetParticlePreFab(), transform.position, Quaternion.identity);
            prefab.transform.parent = transform;
            ParticleSystem myParticleSystem = prefab.GetComponent<ParticleSystem>();
            myParticleSystem.Play();
            Destroy(prefab, myParticleSystem.main.duration);
        }


    }
}
