using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class SpecialAbilities : MonoBehaviour
    {
        [SerializeField] AbilityConfig[] abilities = null;
        [SerializeField] Image energyBar = null;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField]  float energyRegenPerSecond = 1f;
        //TODO add outOfEnergy;
       
       


        float currentEnergyPoints; //TODO make private
        AudioSource audioSource;


        float GetEnergyasPercent
        {
                  get 
                {
                return currentEnergyPoints / maxEnergyPoints;
                }
        }

       
        void Start()
        {
           audioSource = GetComponent<AudioSource>();

            AttachInitialAbilities();
            currentEnergyPoints = maxEnergyPoints;
            UpdateEnergyBar();



        }

         void Update()
        {
            if (currentEnergyPoints < maxEnergyPoints)
            {
                AddEnergyPoint();
                UpdateEnergyBar();
            }
        }

        private void AddEnergyPoint()
        {
            float pointsToAdd = energyRegenPerSecond * Time.deltaTime;
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + pointsToAdd, 0, maxEnergyPoints);
        }


       public void ConsumeEnergy (float amount)
        {
            //TODO remove magic numbers
            float newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
            UpdateEnergyBar();
        } 

       

        private void UpdateEnergyBar()
        {
            energyBar.fillAmount = GetEnergyasPercent;
        }

         void AttachInitialAbilities()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo(gameObject);
            }

        }

        public void AttemptSpecialAbility(int abilityIndex)
        {
            var energyComponent = GetComponent<SpecialAbilities>();
            var energyCost = abilities[abilityIndex].GetEnergyCost();
            if (energyCost <= currentEnergyPoints)
            {
                ConsumeEnergy(energyCost);
                print("Using Special ability" + abilityIndex); //TODO make work
            }
            else

            {
                //TODO play out of energy sound
            }
        }

        
        public int GetNumberOfAbilities()
        {
            return abilities.Length;
        }

    }
}
