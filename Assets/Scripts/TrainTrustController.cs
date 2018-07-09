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
            slider.maxValue = thrustSteps + reverseSteps + 1;
            slider.value = reverseSteps + 1;
            UpdateSliderColor(0);
            Debug.Log("TrainTrustController.OnEnable() executed");
        }

        public void BrakeButtonPressedHandler()
        {
            if (slider.value >0)
            {
                slider.value--;
                UpdateThrust();
            }
        }

        public void ThrottleButtonPressedHandler()
        {
            if (slider.value < slider.maxValue)
            {
                slider.value++;
                UpdateThrust();
            }
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

        void UpdateThrust()
        {
            float thrustCoefficient = 0;
            if (slider.value >= reverseSteps)
            {
                thrustCoefficient = (slider.value - reverseSteps - 1) / thrustSteps;
            }
            else
            {
                thrustCoefficient = (slider.value - reverseSteps) / thrustSteps;
            }
            trainController.SetThrust(thrustCoefficient);
            UpdateSliderColor(thrustCoefficient);
        }
    }
}
