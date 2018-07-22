using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BronePoezd.Train
{
    public class TrainTrustController : MonoBehaviour
    {
        const int thrustSteps = 5;
        const int reverseSteps = 2;
        Slider slider;
        [SerializeField]
        TrainController trainController;
        [SerializeField]
        Image sliderBackGroundImage;

        private void OnEnable()
        {
            slider = GetComponent<Slider>();
            slider.maxValue = thrustSteps + reverseSteps;
            slider.value = reverseSteps;
            UpdateSliderColor(0);
            Debug.Log("TrainTrustController.OnEnable() executed");
        }

        void UpdateSliderColor(float thrustCoefficient)
        {
            Color newColor;
            if (thrustCoefficient >= 0)
            {
                newColor = Color.Lerp(Color.white, Color.red, thrustCoefficient);
            }
            else
            {
                newColor = Color.Lerp(Color.white, Color.yellow, -thrustCoefficient * thrustSteps / reverseSteps);
            }
            sliderBackGroundImage.color = newColor;
        }

        public void UpdateThrust()
        {
            float thrustCoefficient = 0;
            thrustCoefficient = (slider.value - reverseSteps) / thrustSteps;
            if (thrustCoefficient < 0.2 && thrustCoefficient > -0.2)
            {
                thrustCoefficient = 0;
                slider.value = reverseSteps;
            }
            trainController.SetThrust(thrustCoefficient);
            UpdateSliderColor(thrustCoefficient);
            
        }
    }
}
