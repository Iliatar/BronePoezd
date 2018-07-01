using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BronePoezd;
using UnityEngine;
using UnityEngine.UI;
using BronePoezd.Terrain;

namespace BronePoezd.Interface
{
    public class SegmentsConstructor : MonoBehaviour
    {
        const byte exitNumLimit = 8;
        [SerializeField]
        int buttonSize;
        [SerializeField]
        GameObject segmentButtonPrefab;
        GameObject selectedButton;
        SelectedSegmentInfo selectedSegmentInfo;
        [SerializeField]
        Canvas buttonsParent;

        private void Start()
        {
            MouseController.GetInstance().ClickLMBevent += ClickLMBHandler;
            MouseController.GetInstance().ClickRMBevent += ClickRMBHandler;

            CreateSegmentButtons();
        }

        private void CreateSegmentButtons()
        {
            SegmentPrefabScript[] segmentPrefabsList = GetComponentsInChildren<SegmentPrefabScript>();
            RectTransform parentRect = buttonsParent.GetComponent<RectTransform>();

            SetCanvas(segmentPrefabsList.Length, parentRect);
            SetPrefab();

            int maxRotationCount = 0;
            int prefabIndex = 0;
            foreach (SegmentPrefabScript prefabScript in segmentPrefabsList)
            {
                if (prefabScript.RotationCount > maxRotationCount)
                {
                    maxRotationCount = prefabScript.RotationCount;
                    int newVerticalSize = buttonSize * maxRotationCount;
                    parentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newVerticalSize);
                }

                for (int rotationIndex = 0; rotationIndex < prefabScript.RotationCount; rotationIndex++)
                {
                    InstantiateButton(prefabIndex, rotationIndex, prefabScript);
                }
                prefabIndex++;
            }
        }

        private void InstantiateButton(int prefabIndex, int rotationIndex, SegmentPrefabScript prefabScript)
        {
            GameObject addedButton = Instantiate(segmentButtonPrefab, buttonsParent.transform);

            byte exit1 = (byte)((prefabScript.Exit1 + (2 * rotationIndex)) % exitNumLimit);
            byte exit2 = (byte)((prefabScript.Exit2 + (2 * rotationIndex)) % exitNumLimit);
            addedButton.GetComponent<SegmentButtonScript>().SetExits(exit1, exit2);

            Image addedButtonImage = addedButton.GetComponentsInChildren<Image>()[1];
            addedButtonImage.sprite = prefabScript.GetComponent<SpriteRenderer>().sprite;
            addedButtonImage.type = Image.Type.Sliced;

            RectTransform addedButtonRect = addedButton.GetComponent<RectTransform>();
            addedButtonRect.SetParent(buttonsParent.transform);

            float posX = buttonSize / 2 + prefabIndex * buttonSize;
            float posY = -(buttonSize / 2 + rotationIndex * buttonSize);
            addedButtonRect.localPosition = new Vector3(posX, posY, 0);

            float zRotationAngle = -90 * rotationIndex;
            addedButtonRect.localRotation = Quaternion.Euler(0, 0, zRotationAngle);
        }

        private void SetCanvas(int listLength, RectTransform parentRect)
        {
            int horizontalSize = buttonSize * listLength;
            parentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, horizontalSize);
            int verticalSize = buttonSize;
            parentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, verticalSize);

        }

        private void SetPrefab()
        {
            RectTransform buttonPrefabRect = segmentButtonPrefab.GetComponent<RectTransform>();
            buttonPrefabRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize);
            buttonPrefabRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize);

            RectTransform buttonPrefabImageRect = segmentButtonPrefab.GetComponentsInChildren<RectTransform>()[1];
            buttonPrefabImageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize * 0.9f);
            buttonPrefabImageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize * 0.9f);
        }

        private void ClickLMBHandler(GameObject clickedObject)
        {
            if (clickedObject != null && clickedObject.tag == "Map Tiles")
            {
                if (selectedButton != null)
                {
                    clickedObject.GetComponent<TerrainTile>().TryAddSegment(selectedSegmentInfo);
                }
            }
        }

        private void ClickRMBHandler(GameObject clickedObject)
        {
            if (clickedObject != null && clickedObject.tag == "Map Tiles")
            {
                clickedObject.GetComponent<TerrainTile>().TryRemoveSegment(selectedSegmentInfo);
            }
        }


        public void SetSelectedButton(GameObject clickedButton)
        {
            if (selectedButton != null)
            {
                selectedButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
            }

            selectedButton = clickedButton;
            selectedButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.65f);

            var prefabScript = selectedButton.GetComponent<SegmentButtonScript>();
            byte exit1 = prefabScript.Exit1;
            byte exit2 = prefabScript.Exit2;
            TerrainTile.RoadSegment segment = new TerrainTile.RoadSegment(exit1, exit2, SegmentStatus.inactive);
            Sprite sprite = selectedButton.GetComponentsInChildren<Image>()[1].sprite;
            float rotation = selectedButton.GetComponent<RectTransform>().rotation.eulerAngles.z;
            Debug.Log("Selected segment Rotation = " + selectedButton.GetComponent<RectTransform>().rotation.eulerAngles.z);
            selectedSegmentInfo = new SelectedSegmentInfo(segment, sprite, rotation);
        }

        public struct SelectedSegmentInfo
        {
            public TerrainTile.RoadSegment Segment { get; private set; }
            public Sprite Sprite { get; private set; }
            public float Rotation { get; private set; }

            public SelectedSegmentInfo(TerrainTile.RoadSegment segment, Sprite sprite, float rotation)
            {
                this.Segment = segment;
                this.Sprite = sprite;
                this.Rotation = rotation;
            }
        }
    }
}
