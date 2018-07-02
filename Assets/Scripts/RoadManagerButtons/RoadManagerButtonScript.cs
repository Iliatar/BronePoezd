using BronePoezd.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BronePoezd.Interface
{
    abstract public class RoadManagerButtonScript : MonoBehaviour
    {
        RoadManager roadManager;
        protected RoadManager.ConstructionMode constructionMode;

        virtual protected void Awake()
        {
            roadManager = FindObjectOfType<RoadManager>().GetComponent<RoadManager>();
        }

        public void ButtonClickHandler()
        {
            roadManager.SetActiveButton(gameObject, constructionMode);
        }
    }
}
