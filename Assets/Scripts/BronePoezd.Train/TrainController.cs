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
            if (!DepotMediator.TrainIsInDepot)
            {
                UpdateCurrentSpeed();
                platformListIsLocked = true;
                foreach (PlatformController platform in platformList)
                {
                    platform.UpdatePosition(currentSpeed);
                }
                platformListIsLocked = false;

                if(CheckTrainIsInDepot())
                {
                    SetTrainInDepot();
                }
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
            foreach (var platform in platformList)
            {
                float platformMaxThrust = platform.GetPhysParams().GetMaxThrust();
                if (platformMaxThrust > 0)
                {
                    ParticleSystem.EmissionModule emisModule = platform.GetComponentInChildren<ParticleSystem>().emission;
                    emisModule.rateOverTime = 12 * Math.Abs(thrustPower) * platformMaxThrust / 20;
                }
            }
        }

        public void ChangeBreaking(bool newBreakingStatus)
        {
            isBreaking = newBreakingStatus;
        }

        private void InitializeInDepot()
        {
            platformList = new List<PlatformController>();
            removedPlatformList = new List<PlatformController>();
            SetTrainInDepot();
            platformListIsLocked = false;
        }

        public bool AddPlatform(TrainPhysParams platformPhysParams, Sprite sprite)
        {
            bool result = false;
            if (DepotMediator.TrainIsInDepot)
            {
                if (platformList.Count < platformCountLimit)
                {
                    Vector3 newPosition = new Vector3(0, 0, 0);
                    GameObject newPlatform = Instantiate(platformPrefab, newPosition, new Quaternion(), gameObject.transform);
                    newPlatform.GetComponent<SpriteRenderer>().sprite = sprite;
                    PlatformController platform = newPlatform.GetComponent<PlatformController>();
                    platform.Initialize(platformPhysParams);
                    platformList.Add(platform);
                    physParams = TrainPhysParams.CalculateTotalParams(platformList);
                    SetPlatformInDepot(platform);
                    result = true;
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

        public void MarkPlatformToDestroy()
        {
            platformListIsLocked = true;
            foreach (var platform in platformList)
            {
                MarkPlatformToDestroy(platform);
            }
            platformListIsLocked = false;
        }

        public void LaunchTrain()
        {
            trainControlCanvas.enabled = true;
            trainControlCanvas.GetComponentInChildren<TrainTrustController>().enabled = true;
            //здесь надо развернуть поезд в сторону выезда
            for (int platformIndex = 0; platformIndex < platformList.Count; platformIndex++)
            {
                platformList[platformIndex].SetCurrentL(14.5f - 0.2f * platformIndex);
            }
        }

        private bool CheckTrainIsInDepot()
        {
            bool result = true;
            int platformIndex = 0;

            while (result && platformIndex < platformList.Count)
            {
                result = platformList[platformIndex].IsPlatformInDepot();
                Debug.LogFormat("Platform #{0} is in Depot:{1}", platformIndex, result);
                platformIndex++;
            }

            return result;
        }

        private void SetTrainInDepot()
        {
            trainControlCanvas.enabled = false;
            trainControlCanvas.GetComponentInChildren<TrainTrustController>().enabled = false;
            currentSpeed = 0;
            DepotMediator.SetTrainIsInDepot(true);
            foreach (var platform in platformList)
            {
                SetPlatformInDepot(platform);
            }
        }

        private void SetPlatformInDepot (PlatformController platform)
        {
            TerrainTile depotTile = FindObjectOfType<TerrainManager>().GetTileMatrix()[DepotMediator.GetDepotPosition().x, DepotMediator.GetDepotPosition().y];
            platform.TryChangeCurrentTile(depotTile, 0);
        }
    }
}
