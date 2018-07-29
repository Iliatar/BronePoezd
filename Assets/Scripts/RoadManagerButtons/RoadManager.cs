using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BronePoezd;
using UnityEngine;
using UnityEngine.UI;
using BronePoezd.Terrain;
using BronePoezd.Train;

namespace BronePoezd.Interface
{
    public class RoadManager : MonoBehaviour
    {
        const byte exitNumLimit = 8;
        [SerializeField]
        int buttonSize, edgeIntend;
        [SerializeField]
        GameObject segmentButtonPrefab, switcherModeButton;
        GameObject activeButton;
        [SerializeField]
        TrainController train;
        SelectedSegmentInfo activeSegmentInfo;
        [SerializeField]
        Canvas buttonsParent, depoPanel;
        public enum ConstructionMode : byte { empty, builder, switcher}
        ConstructionMode constructionMode;

        private void Start()
        {
            MouseController.GetInstance().ClickLMBevent += ClickLMBHandler;
            MouseController.GetInstance().ClickRMBevent += ClickRMBHandler;

            constructionMode = ConstructionMode.empty;

            CreateButtonsPanel();

            train = FindObjectOfType<TrainController>();
        }

        private void CreateButtonsPanel()
        {
            SegmentPrefabScript[] segmentPrefabsList = GetComponentsInChildren<SegmentPrefabScript>();
            RectTransform parentRect = buttonsParent.GetComponent<RectTransform>();

            SetCanvas(segmentPrefabsList.Length, parentRect);
            SetFunctionalPanel(segmentPrefabsList);

            int maxRotationCount = 0;
            int prefabIndex = 0;
            foreach (SegmentPrefabScript prefabScript in segmentPrefabsList)
            {
                if (prefabScript.RotationCount > maxRotationCount)
                {
                    maxRotationCount = prefabScript.RotationCount;
                    int newVerticalSize = buttonSize * maxRotationCount + edgeIntend * 2;
                    parentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newVerticalSize);
                }

                for (int rotationIndex = 0; rotationIndex < prefabScript.RotationCount; rotationIndex++)
                {
                    InstantiateButton(prefabIndex, rotationIndex, prefabScript);
                }
                prefabIndex++;
            }

        }

        private void SetFunctionalPanel(SegmentPrefabScript[] segmentPrefabsList)
        {
            SetFunctionalButton(switcherModeButton, segmentPrefabsList.Count(), 0);
        }

        void SetFunctionalButton(GameObject button, int xPos, int yPos)
        {
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            float x = buttonSize / 2 + buttonSize * xPos + edgeIntend;
            float y = -(buttonSize / 2 + buttonSize * yPos + edgeIntend);
            buttonRect.localPosition = new Vector2(x, y);
            buttonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize);
            buttonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize);
        }

        private void InstantiateButton(int prefabIndex, int rotationIndex, SegmentPrefabScript prefabScript)
        {
            GameObject addedButton = Instantiate(segmentButtonPrefab, buttonsParent.transform);

            byte exit1, exit2;
            if (prefabScript.Exit1 < 9)
            {
                exit1 = (byte)((prefabScript.Exit1 + (2 * rotationIndex)) % exitNumLimit);
            }
            else
            {
                exit1 = prefabScript.Exit1;
            }
            if (prefabScript.Exit2 < 9)
            {
                exit2 = (byte)((prefabScript.Exit2 + (2 * rotationIndex)) % exitNumLimit);

            }
            else
            {
                exit2 = prefabScript.Exit2;
            }

            addedButton.GetComponent<SegmentButtonScript>().SetExits(exit1, exit2);

            Image addedButtonImage = addedButton.GetComponentsInChildren<Image>()[1];
            addedButtonImage.sprite = prefabScript.GetComponent<SpriteRenderer>().sprite;
            addedButtonImage.color = new Color(0, 0, 0, 1);
            addedButtonImage.type = Image.Type.Sliced;

            RectTransform addedButtonRect = addedButton.GetComponent<RectTransform>();
            addedButtonRect.SetParent(buttonsParent.transform);

            float posX = buttonSize / 2 + prefabIndex * buttonSize + edgeIntend;
            float posY = -(buttonSize / 2 + rotationIndex * buttonSize + edgeIntend);
            addedButtonRect.localPosition = new Vector3(posX, posY, 0);

            float zRotationAngle = -90 * rotationIndex;
            addedButtonRect.localRotation = Quaternion.Euler(0, 0, zRotationAngle);

            addedButtonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize);
            addedButtonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize);

            float imageScaler = 0.75f;
            RectTransform addedButtonImageRect = addedButton.GetComponentsInChildren<RectTransform>()[1];
            addedButtonImageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize * imageScaler);
            addedButtonImageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize * imageScaler);
        }

        private void SetCanvas(int listLength, RectTransform parentRect)
        {
            int horizontalSize = buttonSize * (listLength + 1) + edgeIntend * 2;
            parentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, horizontalSize);
            int verticalSize = buttonSize + edgeIntend * 2;
            parentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, verticalSize);

        }

        private void ClickLMBHandler(GameObject clickedObject)
        {
            if (clickedObject != null && clickedObject.tag == "Map Tiles")
            {
                TerrainTile clickedTile = clickedObject.GetComponent<TerrainTile>();
                switch (constructionMode)
                {
                    case ConstructionMode.builder:
                        {
                            if (CheckDepotBuild(activeSegmentInfo, clickedTile))
                            {
                                clickedTile.TryAddSegment(activeSegmentInfo);
                            }
                            break;
                        }
                    case ConstructionMode.switcher:
                        {
                            clickedTile.GetComponent<TerrainTile>().IterateActiveSegment();
                            break;
                        }
                    default:
                        {
                            Debug.Log("Construction Mode value is not in work range");
                            break;
                        }
                }
            }
        }

        private void ClickRMBHandler(GameObject clickedObject)
        {
            if (clickedObject != null && clickedObject.tag == "Map Tiles")
            {
                TerrainTile clickedTile = clickedObject.GetComponent<TerrainTile>();
                switch (constructionMode)
                {
                    case ConstructionMode.builder:
                        {
                            TryRemoveSegment(activeSegmentInfo, clickedTile);
                            break;
                        }
                    case ConstructionMode.switcher:
                        {
                            break;
                        }
                    default:
                        {
                            Debug.Log("Construction Mode value is not in work range");
                            break;
                        }
                }
            }
        }

        public void SetActiveButton(GameObject clickedButton, ConstructionMode mode)
        {
            if (activeButton != null)
            {
                activeButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
            }

            activeButton = clickedButton;
            activeButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.65f);

            constructionMode = mode;

            if (constructionMode == ConstructionMode.builder)
            {
                Debug.Log("Builder mode activated");
                SetActiveSegmentInfo();
            }
            else if (constructionMode == ConstructionMode.switcher)
            {
                Debug.Log("Switcher mode activated");
            }
        }

        void SetActiveSegmentInfo()
        {
            var prefabScript = activeButton.GetComponent<SegmentButtonScript>();
            byte exit1 = prefabScript.Exit1;
            byte exit2 = prefabScript.Exit2;
            Sprite sprite = activeButton.GetComponentsInChildren<Image>()[1].sprite;
            float rotation = activeButton.GetComponent<RectTransform>().rotation.eulerAngles.z;
            activeSegmentInfo = new SelectedSegmentInfo(exit1, exit2, sprite, rotation);
        }

        private bool CheckDepotBuild(SelectedSegmentInfo activeSegment, TerrainTile addedTile)
        {
            bool result = false;
            List<TerrainTile.RoadSegment> tileSegments = addedTile.TileSegments;

            Debug.LogFormat("activeSegment.Exit1 = {0}, activeSegment.Exit2 = {1}", activeSegment.Exit1, activeSegment.Exit2);
            bool addedSegmentIsDepot = activeSegment.Exit1 == 9 && activeSegment.Exit2 == 3;
            if (addedSegmentIsDepot)
            {
                Debug.LogFormat("Try to add Depot. depotExist = {0}", DepotMediator.DepotExist());
                if (!DepotMediator.DepotExist() && tileSegments.Count == 0)
                {
                    result = true;
                    DepotMediator.PlaceDepot(addedTile.Position);
                }
            }
            else
            {
                if (tileSegments.Count > 0)
                {
                    bool tileHasDepot = tileSegments[0].Exit1 == 9 && tileSegments[0].Exit2 == 3;
                    if (!tileHasDepot)
                    {
                        result = true;
                    }
                }
                result = true;
            }

            return result;
        }

        private void TryRemoveSegment(SelectedSegmentInfo segmentInfo, TerrainTile tile)
        {
            TerrainTile.RoadSegment segment = new TerrainTile.RoadSegment(segmentInfo.Exit1, segmentInfo.Exit2, SegmentStatus.active, null);
            if (tile.TileSegments.Contains(segment))
            {
                bool segmentIsFree = true;

                if (!DepotMediator.TrainIsInDepot)
                {
                    foreach (var platform in train.PlatformList)
                    {
                        if (platform.CurrentTile == tile)
                        {
                            segmentIsFree = false;
                        }
                    }
                }

                if (segmentIsFree)
                {
                    int removedSegmentIndex = tile.TileSegments.IndexOf(segment);
                    tile.RemoveSegment(removedSegmentIndex);
                    if (segment.Exit1 == 9 && segment.Exit2 == 3)
                    {
                        DepotMediator.RemoveDepot();
                        depoPanel.GetComponent<DepoManagerController>().TogglePanelOff();
                    }
                }
            }
        }

        public struct SelectedSegmentInfo
        {
            public byte Exit1 { get; private set; }
            public byte Exit2 { get; private set; }
            public Sprite Sprite { get; private set; }
            public float Rotation { get; private set; }

            public SelectedSegmentInfo(byte exit1, byte exit2, Sprite sprite, float rotation)
            {
                this.Exit1 = exit1;
                this.Exit2 = exit2;
                this.Sprite = sprite;
                this.Rotation = rotation;
            }
        }
    }
}
