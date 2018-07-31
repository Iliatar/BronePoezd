using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BronePoezd.Train;

namespace BronePoezd
{
    static class DepotMediator
    {
        static bool depotExist = false;
        static Vector2Int depotPosition;
        public static bool TrainIsInDepot { get; private set; }
        static TrainController train;
        static DepoManagerController depoManager;

        static public void Initialize ()
        {
            train = GameObject.FindObjectOfType<TrainController>();
            depoManager = GameObject.FindObjectOfType<DepoManagerController>();
            TrainIsInDepot = false;
            RemoveDepot();
        }

        static public void PlaceDepot(Vector2Int placedPosition)
        {
            depotExist = true;
            depotPosition = placedPosition;
            if (TrainIsInDepot)
            {
                depoManager.TogglePanelOn();
            }
        }

        static public bool DepotExist()
        {
            return depotExist;
        }

        static public Vector2Int GetDepotPosition()
        {
            return depotPosition;
        }

        static public void  RemoveDepot()
        {
            depotExist = false;
            if (TrainIsInDepot)
            {
                train.MarkPlatformToDestroy();
            }
            depotPosition = new Vector2Int(-1, -1);
        }

        static public void SetTrainIsInDepot (bool isInDepot)
        {
            TrainIsInDepot = isInDepot;
            if (depotExist)
            {
                if (isInDepot)
                {
                    depoManager.TogglePanelOn();
                }
                else
                {
                    train.LaunchTrain();
                }
            }
            Debug.LogFormat("SetTrainIsInDepot:{0}", TrainIsInDepot);
        }
    }
}
