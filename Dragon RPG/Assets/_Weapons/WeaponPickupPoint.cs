﻿using UnityEngine;
using RPG.Characters;

namespace RPG.Weapons
{
    [ExecuteInEditMode]
    public class WeaponPickupPoint : MonoBehaviour
    {
        [SerializeField]  Weapon weaponConfig = null;
        [SerializeField]  AudioClip pickUpSFX = null;

        AudioSource audioSource;
        // Use this for initialization
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!Application.isPlaying)
            {
                DestroyChildren();
                InstantiateWeapon();
                print("destroy");
            }
        }

        void DestroyChildren()
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        void InstantiateWeapon()
        {
            var weapon = weaponConfig.GetWeaponPreFab();
            weapon.transform.position = Vector3.zero;
           Instantiate(weapon, gameObject.transform);
        }
        private void OnTriggerEnter()
        {
            FindObjectOfType<Player>().PutWeaponInHand(weaponConfig);
            audioSource.PlayOneShot(pickUpSFX);
        }
    }
}
