using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BronePoezd.Interface
{
    public class SegmentPrefabScript : MonoBehaviour
    {
        [SerializeField]
        byte rotationCount, exit1, exit2;

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
    }
}
