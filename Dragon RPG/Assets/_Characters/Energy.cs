using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;
using System;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] RawImage energyBar = null;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float pointsPerHit = 10f;

        float currentEnergyPoints; //TODO make private
        CameraUI.CameraRaycaster cameraRaycaster;
        

        // Use this for initialization
        void Start()
        {
            currentEnergyPoints = maxEnergyPoints;
            cameraRaycaster = Camera.main.GetComponent<CameraUI.CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += onMouseOverEnemy;

        }

        private void onMouseOverEnemy(Enemy enemy)
        {
            {
                if(Input.GetMouseButtonDown(1))
                {
                    UpdateEnergyPoints();
                    UpdateEnergyBar();
                }
            }

        }
       private void UpdateEnergyPoints ()
        {
            float newEnergyPoints = currentEnergyPoints - pointsPerHit;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
        } 

       

        private void UpdateEnergyBar()
        {
            float xValue = -(EnergyAsPercent() / 2f) - 0.5f;
            energyBar.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
        }
        float EnergyAsPercent()
        {
            return currentEnergyPoints / maxEnergyPoints;
        }
    }
}
