using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons
{

    [CreateAssetMenu(menuName = ("RPG/Weapon"))]
    public class Weapon : ScriptableObject
    {

        [SerializeField] float minTimeBetweenHits = .5f;
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] AnimationClip attackAnimation = null;


        public Transform gripTransform;

        public float GetMinTimeBetweenHits()
        {
            //TODO Consider whether we take animation time into account
            return minTimeBetweenHits;
        }

        public float GetMaxAttackRange()
        {
            return maxAttackRange;
        }


        public GameObject GetWeaponPreFab()
        {
            return weaponPrefab;
        }
        public AnimationClip GetAnimClip()
        {
            RemoveAnimationEvents();
            return attackAnimation;
        }

        // So that asset packs cannot cause crashes.
        private void RemoveAnimationEvents()
        {
            attackAnimation.events = new AnimationEvent[0];
        }
    }
}
