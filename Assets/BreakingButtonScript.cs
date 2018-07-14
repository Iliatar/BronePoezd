using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BronePoezd.Train
{

    public class BreakingButtonScript : MonoBehaviour
    {
        bool isBreaking;
        Image buttonImage;
        [SerializeField]
        TrainController trainController;

        private void OnEnable()
        {
            isBreaking = false;
            buttonImage = GetComponent<Image>();
            buttonImage.color = Color.white;
            trainController.ChangeBreaking(false);
        }

        public void PressButtonHandler()
        {
            isBreaking = !isBreaking;
            if (isBreaking)
            {
                buttonImage.color = Color.red;
                trainController.ChangeBreaking(true);
            }
            else
            {
                buttonImage.color = Color.white;
                trainController.ChangeBreaking(false);
            }
        }

    }
}
