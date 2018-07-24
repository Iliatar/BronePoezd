using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BronePoezd.Train;

namespace BronePoezd
{
    static class DepotData
    {
        static bool depotExist = false;
        static Vector2Int depotPosition;

        static public void PlaceDepot(Vector2Int placedPosition)
        {
            depotExist = true;
            depotPosition = placedPosition;
            GameObject.FindObjectOfType<DepoManagerController>().TogglePanelOn();
        }

        static public bool DepotExist()
        {
            return depotExist;
        }

        static public Vector2Int DepotPosition()
        {
            return depotPosition;
        }

        static public void  RemoveDepot()
        {
            depotExist = false;
        }
    }
}
