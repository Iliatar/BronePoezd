using BronePoezd.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BronePoezd.Interface
{
    public class ModeButtonScript : RoadManagerButtonScript
    {
        [SerializeField]
        RoadManager.ConstructionMode buttonMode;

        override protected void Awake()
        {
            base.Awake();
            constructionMode = buttonMode;
        }
    }
}
