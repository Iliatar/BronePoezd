using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BronePoezd.Terrain;

namespace BronePoezd.Train
{

    //TODO: заполнить case в SegmentsPathLibrary для больших кривых
    //TODO: сделать метод для "установки" поезда в нужное положение в начале игры и при её загрузке
    public class TrainController : MonoBehaviour
    {
        float mass;
        float maxThrust;
        float thrusterPower;
        float currentSpeed;
        float maxSpeed;
        float maxBreakingForce;
        float dragAtMaxSpeed;
        bool isBreaking;
        TerrainTile currentTile;
        byte exitFrom, exitTo;
        float currentL;
        SegmentsPathLibrary.SegmentPathData pathData;
        TerrainManager terrainManager;
        Transform trainTransform;
        Vector3 currentTilePos;
        [SerializeField]
        Canvas trainControlCanvas;

        private void Awake()
        {
            trainTransform = GetComponent<Transform>();
            mass = 500000;
            maxThrust = 20000;
            maxSpeed = 0.3f;
            maxBreakingForce = 10000;
            dragAtMaxSpeed = 7000;
            currentL = 0;
            InitializeWithZeroSpeed();
            terrainManager = FindObjectOfType<TerrainManager>();
        }

        private void Update()
        {
            Debug.Log("Update() execution started");
            UpdatePosition();
            Debug.Log("Update() executed");
        }

        public void SetThrust(float newThrusterPower)
        {
            thrusterPower = newThrusterPower;
            if (thrusterPower > 1)
            {
                thrusterPower = 1;
            }
            else if (thrusterPower < -1)
            {
                thrusterPower = -1;
            }
            ParticleSystem.EmissionModule emisModule = GetComponentInChildren<ParticleSystem>().emission;
            emisModule.rateOverTime = 12 * Math.Abs(thrusterPower);
            Debug.LogFormat("TrainController.SetThrust() executed. thrusterPower = {0}", thrusterPower);
        }

        private void UpdatePosition()
        {
            UpdateCurrentSpeed();
            float newL = currentL + currentSpeed / terrainManager.TileSize * Time.deltaTime;
            Debug.LogFormat("UpdatePosition() executing. currentL = {0}, newL = {1}", currentL, newL);
            currentL = newL;
            if (newL > pathData.LMax)
            {
                ChangeCurrentMapTile(true);
            }
            else if (newL < 0)
            {
                ChangeCurrentMapTile(false);
            }
            const float posDelta = 0.01f;
            Vector3 positionPrev = new Vector3(pathData.XFunction(currentL - posDelta), pathData.YFunction(currentL - posDelta), -0.05f);
            Vector3 positionNext = new Vector3(pathData.XFunction(currentL + posDelta), pathData.YFunction(currentL + posDelta), -0.05f);
            Vector3 newLocalPos = (positionPrev + positionNext) / 2;
            Vector3 tileOffset = new Vector3(-terrainManager.TileSize / 2, -terrainManager.TileSize / 2, 0);
            Debug.LogFormat("currentTilePos = {0}", currentTilePos);
            Debug.LogFormat("tileOffset = {0}", tileOffset);
            Debug.LogFormat("newLocalPos = {0}", newLocalPos);
            Debug.LogFormat("tileSize = {0}", terrainManager.TileSize);
            Vector3 newPosition = currentTilePos + tileOffset + newLocalPos * terrainManager.TileSize;
            trainTransform.position = newPosition;
            Quaternion newRotarion = Quaternion.LookRotation(transform.forward, positionNext - positionPrev);
            trainTransform.rotation = newRotarion;
            Debug.Log("UpdatePosition() execution finished");
        }

        private void UpdateCurrentSpeed()
        {
            float breakingForce = 0;
            if (isBreaking)
            {
                if (currentSpeed > 0)
                {
                    breakingForce = maxBreakingForce;
                }
                else if (currentSpeed < 0)
                {
                    breakingForce = -maxBreakingForce;
                }
            }
            float thrust = maxThrust * thrusterPower;
            float drag = currentSpeed / maxSpeed * dragAtMaxSpeed;
            float totalForce = thrust - drag - breakingForce;
            float acceleration = totalForce / mass * Time.deltaTime;
            currentSpeed += acceleration;
            if (currentSpeed > maxSpeed)
            {
                currentSpeed = maxSpeed;
            }
            else if (currentSpeed < -maxSpeed)
            {
                currentSpeed = -maxSpeed;
            }
            else if (Math.Abs(currentSpeed) < 0.000001)
            {
                currentSpeed = 0;
            }
            Debug.LogFormat("UpdateCurrentSpeed() executed. thrust = {0}, drag = {1}, breakinForce = {2}, acceleration = {3}, currentSpeed = {4}", thrust, drag, breakingForce, acceleration, currentSpeed);
        }

        private void ChangeCurrentMapTile(bool isMovingForvard)
        {
            Debug.LogFormat("ChangeCurrentMapTile() starting Execution. isMovingForvard = {0}", isMovingForvard);

            byte findNewTileArgument = isMovingForvard ? exitTo : exitFrom;
            TerrainTile newTile = FindNewTile(findNewTileArgument);

            bool noCouplingSegment = true;

            if (newTile != null)
            {
                TerrainTile.RoadSegment newSegment = newTile.GetActiveSegment();

                if (newSegment != null)
                {
                    bool checkResult;
                    if (isMovingForvard)
                    {
                        checkResult = CheckCommonExit(ref exitFrom, ref exitTo, newSegment);
                    }
                    else
                    {
                        checkResult = CheckCommonExit(ref exitTo, ref exitFrom, newSegment);
                    }

                    if (checkResult)
                    {
                        if (isMovingForvard)
                        {
                            currentL -= pathData.LMax;
                            pathData = SegmentsPathLibrary.GetInstance().GetSegmentPathData(exitFrom, exitTo);
                        }
                        else
                        {
                            pathData = SegmentsPathLibrary.GetInstance().GetSegmentPathData(exitFrom, exitTo);
                            currentL = pathData.LMax + currentL;
                        }

                        SetCurrentTile(newTile);
                        noCouplingSegment = false;
                        Debug.LogFormat("ChangeCurrentMapTile() executed. CurrentL after reset = {0}", currentL);
                    }
                }
            }

            if (noCouplingSegment)
            {
                Debug.Log("Train crashed!!! There is no active coupling segment in new tile!!!");
                DestroyTrain();
            }
        }

        private bool CheckCommonExit(ref byte checkedExit, ref byte changedExit, TerrainTile.RoadSegment newSegment)
        {
            bool checkResult = true;

            checkedExit = (byte)((changedExit + 4) % 8);
            Debug.LogFormat("Setting exitFrom = {0}", exitFrom);

            if (newSegment.Exit1 == checkedExit)
            {
                changedExit = newSegment.Exit2;
                Debug.LogFormat("Setting exitTo = {0}", exitTo);
            }
            else if (newSegment.Exit2 == checkedExit)
            {
                changedExit = newSegment.Exit1;
                Debug.LogFormat("Setting exitTo = {0}", exitTo);
            }
            else
            {
                checkResult = false;
            }

            return checkResult;
        }

        private TerrainTile FindNewTile(byte exitTo)
        {
            TerrainTile newTile = null;
            int x = currentTile.Position.x;
            int y = currentTile.Position.y;
            switch (exitTo)
            {
                case 0:
                    {
                        x--;
                        y--;
                        break;
                    }
                case 1:
                    {
                        x--;
                        break;
                    }
                case 2:
                    {
                        x--;
                        y++;
                        break;
                    }
                case 3:
                    {
                        y++;
                        break;
                    }
                case 4:
                    {
                        x++;
                        y++;
                        break;
                    }
                case 5:
                    {
                        x++;
                        break;
                    }
                case 6:
                    {
                        x++;
                        y--;
                        break;
                    }
                case 7:
                    {
                        y--;
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("exitTo argument have be in  range [0-7]");
                        break;
                    }
            }
            if (x >= 0 || y >= 0 || x < terrainManager.GetFieldSize().x || y < terrainManager.GetFieldSize().y)
            {
                newTile = terrainManager.GetTileMatrix()[x, y];
                Debug.LogFormat("newTile is tile[{0}, {1}]", x, y);
            }
            return newTile;
        }

        public bool TryChangeCurrentTile(TerrainTile newTile)
        {
            Debug.Log("TrySetNewTile executed");
            bool result = false;

            if (newTile.GetActiveSegment() != null)
            {
                ChangeCurrentTile(newTile);
                result = true;
            }

            return result;
        }

        private void ChangeCurrentTile(TerrainTile newTile)
        {
            Debug.Log("SetNewTile executed");
            gameObject.SetActive(true);
            SetCurrentTile(newTile);
            exitFrom = newTile.GetActiveSegment().Exit1;
            exitTo = newTile.GetActiveSegment().Exit2;
            pathData = SegmentsPathLibrary.GetInstance().GetSegmentPathData(exitFrom, exitTo);
            currentL = pathData.LMax / 2;
            InitializeWithZeroSpeed();
            UpdatePosition();
        }

        private void SetCurrentTile(TerrainTile newTile)
        {
            currentTile = newTile;
            currentTilePos = currentTile.transform.position;
        }

        private void InitializeWithZeroSpeed()
        {
            SetThrust(0);
            currentSpeed = 0;
            isBreaking = false;
            trainControlCanvas.enabled = true;
            FindObjectOfType<TrainTrustController>().enabled = true;
        }

        public void DestroyTrain()
        {
            gameObject.SetActive(false);
            trainControlCanvas.enabled = false;
            FindObjectOfType<TrainTrustController>().enabled = false;
        }

        public void ChangeBreaking(bool newBreakingStatus)
        {
            isBreaking = newBreakingStatus;
        }
    }
}
