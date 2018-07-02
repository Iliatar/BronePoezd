using BronePoezd.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BronePoezd.Interface

{
    public class SwitcherButtonScript : RoadManagerButtonScript
    {
        override protected void Awake()
        {
            base.Awake();
            constructionMode = RoadManager.ConstructionMode.switcher;
        }
    }
}
