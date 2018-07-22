using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BronePoezd.Train;
using UnityEngine.UI;

namespace BronePoezd.Interface
{
    public class PlatformIconScript : MonoBehaviour
    {
        DepoManagerController depoManager;

        public void Initialize(DepoManagerController depoManager)
        {
           this.depoManager = depoManager;
        }

        public void ButtonPressedHandler()
        {
            depoManager.RemoveVagon(gameObject);
        }
    }
}
