using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BronePoezd.Terrain;

namespace BronePoezd.Train
{

    public class PlatformController : MonoBehaviour
    {
        TrainPhysParams physParams;
        byte exitFrom, exitTo;
        float currentL;
        SegmentsPathLibrary.SegmentPathData pathData;
        TerrainManager terrainManager;
        Transform trainTransform;
        TrainController trainController;
        Vector3 currentTilePos;
        public TerrainTile CurrentTile { get; private set; }

        public void Initialize(TrainPhysParams physParams)
        {
            trainTransform = GetComponent<Transform>();
            this.physParams = physParams;
            currentL = 0;
            terrainManager = FindObjectOfType<TerrainManager>();
            trainController = GetComponentInParent<TrainController>();
        }

        public void UpdatePosition(float currentSpeed)
        {
            float newL = currentL + currentSpeed / terrainManager.TileSize * Time.deltaTime;
            //Debug.LogFormat("UpdatePosition() executing. currentL = {0}, newL = {1}", currentL, newL);
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
            //Debug.LogFormat("currentTilePos = {0}", currentTilePos);
            //Debug.LogFormat("tileOffset = {0}", tileOffset);
            //Debug.LogFormat("newLocalPos = {0}", newLocalPos);
            //Debug.LogFormat("tileSize = {0}", terrainManager.TileSize);
            Vector3 newPosition = currentTilePos + tileOffset + newLocalPos * terrainManager.TileSize;
            trainTransform.position = newPosition;
            Quaternion newRotarion = Quaternion.LookRotation(transform.forward, positionNext - positionPrev);
            trainTransform.rotation = newRotarion;
            //Debug.Log("UpdatePosition() execution finished");
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
                DestroyPlatform();
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
            int x = CurrentTile.Position.x;
            int y = CurrentTile.Position.y;
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

        public bool TryChangeCurrentTile(TerrainTile newTile, float currentSpeed)
        {
            Debug.Log("TrySetNewTile executed");
            bool result = false;

            if (newTile.GetActiveSegment() != null)
            {
                ChangeCurrentTile(newTile, currentSpeed);
                result = true;
            }

            return result;
        }

        private void ChangeCurrentTile(TerrainTile newTile, float currentSpeed)
        {
            Debug.Log("SetNewTile executed");
            gameObject.SetActive(true);
            SetCurrentTile(newTile);
            exitFrom = newTile.GetActiveSegment().Exit1;
            exitTo = newTile.GetActiveSegment().Exit2;
            pathData = SegmentsPathLibrary.GetInstance().GetSegmentPathData(exitFrom, exitTo);
            currentL = pathData.LMax / 2;
            UpdatePosition(currentSpeed);
        }

        private void SetCurrentTile(TerrainTile newTile)
        {
            CurrentTile = newTile;
            currentTilePos = CurrentTile.transform.position;
        }

        public void DestroyPlatform()
        {
            trainController.MarkPlatformToDestroy(this);
        }

        public void SetCurrentL(float currentL)
        {
            this.currentL = currentL;
        }

        public bool IsPlatformInDepot()
        {
            bool result = false;
            float depoTriggerL = 0.6f;
            Vector2Int curTilePosition = new Vector2Int(CurrentTile.Position.x, CurrentTile.Position.y);
            if (curTilePosition == DepotMediator.GetDepotPosition())
            {
                if (exitFrom == 9 && currentL < pathData.LMax - depoTriggerL)
                {
                    result = true;
                }
                else if (exitFrom == 3 && currentL > depoTriggerL)
                {
                    result = true;
                }
            }
            return result;
        }

        internal TrainPhysParams GetPhysParams()
        {
            return physParams;
        }
    }
}
