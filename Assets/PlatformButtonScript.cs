using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BronePoezd.Train;
using UnityEngine.UI;

namespace BronePoezd.Interface
{
    public class PlatformButtonScript : MonoBehaviour
    {
        TrainPhysParams physParams;
        DepoManagerController depoManager;

        public void Initialize (TrainPhysParams physParams, DepoManagerController depoManager)
        {
            this.physParams = physParams;
            this.depoManager = depoManager;
        }

        public void ButtonPressedHandler()
        {
            Sprite sprite = GetComponentsInChildren<Image>()[1].sprite;
            depoManager.AddVagon(physParams, sprite);
        }
    }
}
