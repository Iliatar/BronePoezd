﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BronePoezd.Interface
{

    public class SegmentButtonScript : RoadManagerButtonScript
    {
        public byte Exit1 { get; private set; }
        public byte Exit2 {get; private set;}
        public Sprite ButtonSprite { get; private set;}
        public Sprite SegmentSprite { get; private set;}
        public Sprite BuildingSprite { get; private set; }

        override protected void Awake()
        {
            base.Awake();
            constructionMode = RoadManager.ConstructionMode.builder;
        }


        public void SetExits (byte exit1, byte exit2)
        {
            this.Exit1 = exit1;
            this.Exit2 = exit2;
            name = "Segment " + exit1 + " to " + exit2;
        }

        public void SetSprites (Sprite buttonSprite, Sprite segmentSprite, Sprite buildingSprite)
        {
            this.ButtonSprite = buttonSprite;
            this.SegmentSprite = segmentSprite;
            this.BuildingSprite = buildingSprite;
        }
    }
}
