using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] Image energyOrb = null;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField]  float energyRegenPerSecond = 1f;
       

        float currentEnergyPoints; //TODO make private
       // CameraUI.CameraRaycaster cameraRaycaster;
        Player player;
       



        // Use this for initialization
        void Start()
        {
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

        public bool IsEnergyAvailable (float amount)
        {
            return amount <= currentEnergyPoints; 
        }

       public void CosnsumeEnergy (float amount)
        {
            //TODO remove magic numbers
            float newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
            UpdateEnergyBar();
        } 

       

        private void UpdateEnergyBar()
        {
            energyOrb.fillAmount = EnergyAsPercent();
        }
        float EnergyAsPercent()
        {
            return currentEnergyPoints / maxEnergyPoints;
        }
    }
}
