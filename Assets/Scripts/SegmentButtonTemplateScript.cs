using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BronePoezd.Interface
{
    public class SegmentButtonTemplateScript : MonoBehaviour
    {
        [SerializeField]
        byte rotationCount, exit1, exit2;
        [SerializeField]
        Sprite buttonSprite, segmentSprite, buildingSprite;

        public byte RotationCount
        {
            get
            {
                return rotationCount;
            }
        }
        public byte Exit1
        {
            get
            {
                return exit1;
            }
        }
        public byte Exit2
        {
            get
            {
                return exit2;
            }
        }

        public Sprite GetButtonSprite()
        {
            return buttonSprite;
        }

        public Sprite GetSegmentSprite()
        {
            return segmentSprite;
        }

        public Sprite GetBuildingSprite()
        {
            return buildingSprite;
        }
    }
}
