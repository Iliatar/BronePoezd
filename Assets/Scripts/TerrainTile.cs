using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BronePoezd.Interface;

namespace BronePoezd.Terrain
{
    public class TerrainTile : MonoBehaviour
    {
        [SerializeField]
        GameObject segmentSpritePrefab;

        static TerrainManager terrainManager;

        const byte SegmentsLimit = 3;
        public Vector2Int Position { get; private set; }
        public TerrainTypes TerrainType { get; private set; }
        public List<RoadSegment> TileSegments { get; private set; }

        private void Awake()
        {
            TileSegments = new List<RoadSegment>();
            if (terrainManager == null)
            {
                terrainManager = GetComponentInParent<TerrainManager>();
            }
        }

        public void TryAddSegment(RoadManager.SelectedSegmentInfo segmentInfo)
        {
            RoadSegment addedSegment = new RoadSegment(segmentInfo.Exit1, segmentInfo.Exit2, SegmentStatus.active, null);
            if (TileSegments.Count < SegmentsLimit)
            {
                if (!TileSegments.Contains(addedSegment))
                {
                    if (ContainsCommonExit(addedSegment))
                    {
                        AddSegment(segmentInfo);
                    }
                }
            }
        }

        void AddSegment(RoadManager.SelectedSegmentInfo segmentInfo)
        {
            GameObject newSpriteObject = Instantiate(segmentSpritePrefab, gameObject.transform.position, Quaternion.Euler(0, 0, segmentInfo.Rotation), gameObject.transform);
            SpriteRenderer newSegmentSprite = newSpriteObject.GetComponent<SpriteRenderer>();
            newSegmentSprite.sprite = segmentInfo.Sprite;
            newSegmentSprite.size = new Vector2(terrainManager.TileSize * 1.05f, terrainManager.TileSize * 1.05f);

            SegmentStatus newSegmentStatus = SegmentStatus.inactive;
            if (TileSegments.Count == 0)
            {
                newSegmentStatus = SegmentStatus.active;
            }

            RoadSegment newSegment = new RoadSegment(segmentInfo.Exit1, segmentInfo.Exit2, newSegmentStatus, newSegmentSprite);
            TileSegments.Add(newSegment);

            TileSegments[TileSegments.Count - 1].SetStatus(newSegmentStatus);
        }

        public void TryRemoveSegment(RoadManager.SelectedSegmentInfo segmentInfo)
        {
            RoadSegment removedSegment = new RoadSegment(segmentInfo.Exit1, segmentInfo.Exit2, SegmentStatus.active, null);

            if (TileSegments.Contains(removedSegment))
            {
                int removedSegmentIndex = TileSegments.IndexOf(removedSegment);
                RemoveSegment(removedSegmentIndex);
            }

        }

        void RemoveSegment(int removedSegmentIndex)
        {
            if (TileSegments[removedSegmentIndex].Status == SegmentStatus.active && TileSegments.Count > 1)
            {
                IterateActiveSegment();
            }
            TileSegments[removedSegmentIndex].DeleteSegment();
            TileSegments.RemoveAt(removedSegmentIndex);
        }

        public void IterateActiveSegment()
        {
            if (TileSegments.Count >= 2)
            {
                int segmentIndex = 0;
                foreach (RoadSegment segment in TileSegments)
                {
                    if (segment.Status == SegmentStatus.active)
                    {
                        segment.SetStatus(SegmentStatus.inactive);
                        int activeSegmentIndex = segmentIndex < TileSegments.Count - 1 ? segmentIndex + 1 : 0;
                        TileSegments[activeSegmentIndex].SetStatus(SegmentStatus.active);
                        break;
                    }
                    segmentIndex++;
                }
            }
        }

        /// <summary>
        /// Метод проверяет, есть ли у chekedSegment выход,
        /// являющийся общим для всех Segment, входящих в TileSegments.
        /// </summary>
        /// <param name="chekedSegment"></param>
        /// <returns></returns>
        bool ContainsCommonExit(RoadSegment chekedSegment)
        {
            bool Exit1IsCommon = true;
            bool Exit2IsCommon = true;
            foreach (RoadSegment segment in TileSegments)
            {
                if (Exit1IsCommon)
                {
                    if (chekedSegment.Exit1 != segment.Exit1 && chekedSegment.Exit1 != segment.Exit2)
                    {
                        Exit1IsCommon = false;
                    }
                }
                if (Exit2IsCommon)
                {
                    if (chekedSegment.Exit2 != segment.Exit1 && chekedSegment.Exit2 != segment.Exit2)
                    {
                        Exit2IsCommon = false;
                    }
                }
                if (!(Exit1IsCommon || Exit2IsCommon))
                {
                    break;
                }
            }
            return (Exit1IsCommon || Exit2IsCommon);
        }

        public class RoadSegment
        {
            public byte Exit1 { get; private set; }
            public byte Exit2 { get; private set; }
            public SegmentStatus Status { get; private set; }
            SpriteRenderer segmentSprite;

            public RoadSegment(byte exit1, byte exit2, SegmentStatus status, SpriteRenderer segmentSprite)
            {
                this.Exit1 = exit1;
                this.Exit2 = exit2;
                this.Status = status;
                this.segmentSprite = segmentSprite;
            }

            public void SetStatus(SegmentStatus newStatus)
            {
                if (newStatus == SegmentStatus.active)
                {
                    Status = SegmentStatus.active;
                    segmentSprite.color = new Color(0, 0, 0, 1);
                    segmentSprite.sortingOrder = 1;
                }
                else if (newStatus == SegmentStatus.inactive)
                {
                    Status = SegmentStatus.inactive;
                    segmentSprite.color = new Color(0f, 0f, 0f, 0.4f);
                    segmentSprite.sortingOrder = 0;
                }
            }

            public void DeleteSegment()
            {
                Destroy(segmentSprite.gameObject);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is RoadSegment))
                {
                    throw new ArgumentException("Argument type is not RailRoadSegment");
                }

                RoadSegment comparedSegment = (RoadSegment)obj;

                bool result = false;

                if (Exit1 == comparedSegment.Exit1 && Exit2 == comparedSegment.Exit2)
                {
                    result = true;
                }
                else if (Exit1 == comparedSegment.Exit2 && Exit2 == comparedSegment.Exit1)
                {
                    result = true;
                }

                return result;
            }

            public override int GetHashCode()
            {
                return Exit1 > Exit2 ? Exit1 * 1000 + Exit2 : Exit2 * 1000 + Exit1;
            }
        }
    }

    public enum TerrainTypes : byte { grass };
    public enum SegmentStatus : byte { active, inactive };
}