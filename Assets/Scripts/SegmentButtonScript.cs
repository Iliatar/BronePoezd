using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BronePoezd.Interface
{

    public class SegmentButtonScript : MonoBehaviour
    {
        public byte Exit1 { get; private set; }
        public byte Exit2 {get; private set;}
        SegmentsConstructor segmentConstructor;

        private void Awake()
        {
            segmentConstructor = GameObject.Find("SegmentsConstructor").GetComponent<SegmentsConstructor>();
        }

        public void SetExits (byte exit1, byte exit2)
        {
            this.Exit1 = exit1;
            this.Exit2 = exit2;
            name = "Segment " + exit1 + " to " + exit2;
        }

        public void ButtonClickHandler()
        {
            segmentConstructor.SetSelectedButton(gameObject);
        }
    }
}
