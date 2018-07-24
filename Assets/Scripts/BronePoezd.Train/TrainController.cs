using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BronePoezd.Terrain;

namespace BronePoezd.Train
{
    public class TrainController : MonoBehaviour
    {
        TrainPhysParams physParams;
        float thrustPower;
        float currentSpeed;
        List<PlatformController> platformList, removedPlatformList;
        bool platformListIsLocked;
        bool isBreaking;
        bool isInDepot;
        [SerializeField]
        GameObject platformPrefab;
        [SerializeField]
        Canvas trainControlCanvas;
        const int platformCountLimit = 7;
        public event Action TrainIsDestroyedEvent;

        public float CurrentSpeed
        {
            get
            {
                return currentSpeed;
            }
        }
        public List<PlatformController> PlatformList
        {
            get
            {
                return platformList;
            }
        }

        private void Awake()
        {
            InitializeInDepot();
        }

        private void Update()
        {
            if (!isInDepot)
            {
                UpdateCurrentSpeed();
                platformListIsLocked = true;
                foreach (PlatformController platform in platformList)
                {
                    platform.UpdatePosition(currentSpeed);
                }
                platformListIsLocked = false;
            }

            DestroyMarkedPlatforms();

        }

        private void DestroyMarkedPlatforms()
        {
            if (!platformListIsLocked && removedPlatformList.Count > 0)
            {
                foreach (PlatformController platform in removedPlatformList)
                {
                    platformList.Remove(platform);
                    Destroy(platform.gameObject);
                }
                physParams = TrainPhysParams.CalculateTotalParams(platformList);
                removedPlatformList.Clear();
                if (platformList.Count == 0)
                {
                    TrainIsDestroyedEvent();
                    InitializeInDepot();
                }
            }
        }

        private void UpdateCurrentSpeed()
        {
            float breakingForce = 0;
            if (isBreaking)
            {
                if (currentSpeed > 0)
                {
                    breakingForce = physParams.GetMaxBreak();
                }
                else if (currentSpeed < 0)
                {
                    breakingForce = -physParams.GetMaxBreak();
                }
            }
            float thrust = physParams.GetMaxThrust() * thrustPower;
            float drag = currentSpeed / physParams.GetMaxSpeed() * physParams.GetMaxDrag();
            float totalForce = thrust - drag - breakingForce;
            float acceleration = totalForce / physParams.GetMass() * Time.deltaTime;
            currentSpeed += acceleration;
            if (currentSpeed > physParams.GetMaxSpeed())
            {
                currentSpeed = physParams.GetMaxSpeed();
            }
            else if (currentSpeed < -physParams.GetMaxSpeed())
            {
                currentSpeed = -physParams.GetMaxSpeed();
            }
            else if (Math.Abs(currentSpeed) < 0.000001)
            {
                currentSpeed = 0;
            }
            Debug.LogFormat("UpdateCurrentSpeed() executed. thrust = {0}, drag = {1}, breakinForce = {2}, acceleration = {3}, currentSpeed = {4}", thrust, drag, breakingForce, acceleration, currentSpeed);
        }

        public void SetThrust(float newThrusterPower)
        {
            thrustPower = newThrusterPower;
            if (thrustPower > 1)
            {
                thrustPower = 1;
            }
            else if (thrustPower < -1)
            {
                thrustPower = -1;
            }
            ParticleSystem.EmissionModule emisModule = GetComponentInChildren<ParticleSystem>().emission;
            //TODO дым должен идти из локомотивов
            emisModule.rateOverTime = 12 * Math.Abs(thrustPower);
            Debug.LogFormat("TrainController.SetThrust() executed. thrusterPower = {0}", thrustPower);
        }

        public void ChangeBreaking(bool newBreakingStatus)
        {
            isBreaking = newBreakingStatus;
        }

        private void InitializeInDepot()
        {
            trainControlCanvas.enabled = false;
            trainControlCanvas.GetComponentInChildren<TrainTrustController>().enabled = false;
            currentSpeed = 0;
            isInDepot = true;
            platformList = new List<PlatformController>();
            removedPlatformList = new List<PlatformController>();
            platformListIsLocked = false;
        }

        public bool AddPlatform(TrainPhysParams platformPhysParams, Sprite sprite, Vector2Int depotPosition)
        {
            bool result = false;
            if (isInDepot)
            {
                if (platformList.Count < platformCountLimit)
                {
                    Vector3 newPosition = new Vector3(0, 0, 0);
                    GameObject newVagon = Instantiate(platformPrefab, newPosition, new Quaternion(), gameObject.transform);
                    newVagon.GetComponent<SpriteRenderer>().sprite = sprite;
                    PlatformController platformController = newVagon.GetComponent<PlatformController>();
                    platformController.Initialize(platformPhysParams);
                    platformList.Add(platformController);
                    result = true;
                    physParams = TrainPhysParams.CalculateTotalParams(platformList);
                    TerrainTile tile = FindObjectOfType<TerrainManager>().GetTileMatrix()[depotPosition.x, depotPosition.y];
                    platformController.TryChangeCurrentTile(tile, 0);
                }
            }
            return result;
        }

        public void MarkPlatformToDestroy(PlatformController removedPlatform)
        {
            removedPlatformList.Add(removedPlatform);
            DestroyMarkedPlatforms();
        }

        public void MarkPlatformToDestroy(int removeIndex)
        {
            MarkPlatformToDestroy(platformList[removeIndex]);
        }

        public void LaunchTrain()
        {
            trainControlCanvas.enabled = true;
            trainControlCanvas.GetComponentInChildren<TrainTrustController>().enabled = true;
            for (int platformIndex = 0; platformIndex < platformList.Count; platformIndex++)
            {
                platformList[platformIndex].SetCurrentL(14.5f - 0.2f * platformIndex);
            }
            isInDepot = false;
        }
    }
}
