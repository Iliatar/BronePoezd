using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BronePoezd.Interface;

namespace BronePoezd.Train
{
    public class DepoManagerController : MonoBehaviour
    {
        [SerializeField]
        List<Sprite> spriteList;
        [SerializeField]
        GameObject buttonPrefab, pickPrefab;
        List<TrainPhysParams> paramsList;
        List<GameObject> platformIconsList;
        [SerializeField]
        int buttonSize, edgeIntend;
        TrainController train;
        Canvas panelCanvas;

        private void Awake()
        {
            Debug.Log("DepotManagerController Awake() executed");
            train = FindObjectOfType<TrainController>();
            paramsList = new List<TrainPhysParams>
            {
                new TrainPhysParams (120, 20, 0.4f, 10, 7),
                new TrainPhysParams (20, 0, 0.4f, 3, 1)
            };
            platformIconsList = new List<GameObject>();
            panelCanvas = GetComponent<Canvas>();
            panelCanvas.enabled = false;
            CreateTypeButtons();
            train.TrainIsDestroyedEvent += TrainIsDestroyedEventHandler;
            FindObjectOfType<DepotSpriteScript>().Initialize();
            DepotMediator.Initialize();
        }

        private void CreateTypeButtons()
        {
            int typeIndex = 0;
            foreach (var type in paramsList)
            {
                InstantiateButton(typeIndex);

                typeIndex++;
            }
        }

        private void InstantiateButton(int typeIndex)
        {
            GameObject addedButton = Instantiate(buttonPrefab, gameObject.transform);

            RectTransform addedButtonRect = addedButton.GetComponent<RectTransform>();
            addedButtonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize);
            addedButtonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize);
            int x = edgeIntend + buttonSize / 2;
            int y = -(edgeIntend + buttonSize / 2 + typeIndex * buttonSize);
            addedButtonRect.localPosition = new Vector3(x, y, 0);

            Image addedButtonImage = addedButton.GetComponentsInChildren<Image>()[1];
            addedButtonImage.sprite = spriteList[typeIndex];
            addedButtonImage.color = new Color(1, 1, 1, 1);
            addedButtonImage.type = Image.Type.Sliced;
            float imageScaler = 0.75f;
            RectTransform addedButtonImageRect = addedButtonImage.GetComponent<RectTransform>();
            addedButtonImageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize * imageScaler);
            addedButtonImageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize * imageScaler);

            addedButton.GetComponent<PlatformButtonScript>().Initialize(paramsList[typeIndex], this);
        }

        public void AddVagon(TrainPhysParams physParams, Sprite sprite)
        {
            bool addIsSucced = train.AddPlatform(physParams, sprite);

            if (addIsSucced)
            {
                RedrawPlatformIcons();
            }
        }

        private void RedrawPlatformIcons()
        {
            foreach (GameObject icon in platformIconsList)
            {
                Destroy(icon);
            }
            platformIconsList.Clear();

            //добавляем новую кнопку на панель управления дэпо
            int pickIndex = 0;
            foreach (var platform in train.PlatformList)
            {
                GameObject newIcon = Instantiate(pickPrefab, gameObject.transform);

                RectTransform iconRect = newIcon.GetComponent<RectTransform>();
                iconRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize);
                iconRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize);

                int x = edgeIntend + buttonSize / 2 + buttonSize;
                int y = -(edgeIntend + buttonSize / 2 + buttonSize * pickIndex);
                Vector3 pickPosition = new Vector3(x, y, 0);
                iconRect.localPosition = pickPosition;


                //добавляем картинку
                //выставляем размер картинки
                //TODO - выделить это в отдельный метод, общий с созданием кнопки
                Image iconImage = newIcon.GetComponentsInChildren<Image>()[1];
                iconImage.sprite = platform.GetComponent<SpriteRenderer>().sprite;
                float imageScaler = 0.75f;
                RectTransform addedIconImageRect = iconImage.GetComponent<RectTransform>();
                addedIconImageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize * imageScaler);
                addedIconImageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize * imageScaler);

                newIcon.GetComponent<PlatformIconScript>().Initialize(this);
                platformIconsList.Add(newIcon);
                pickIndex++;
            }
        }

        public void RemoveVagon (GameObject pressedIcon)
        {
            int removeIndex = platformIconsList.IndexOf(pressedIcon);
            train.MarkPlatformToDestroy(removeIndex);
            RedrawPlatformIcons();
        }

        public void TogglePanelOn ()
        {
            RedrawPlatformIcons();
            panelCanvas.enabled = true;
        }

        public void TogglePanelOff ()
        {
            panelCanvas.enabled = false;
        }

        private void TrainIsDestroyedEventHandler()
        {
            if (DepotMediator.DepotExist())
            {
                TogglePanelOn();
            }
        }

        public void GoButtonPresssedHandler()
        {
            DepotMediator.SetTrainIsInDepot(false);


            TogglePanelOff();
        }
    }
}
