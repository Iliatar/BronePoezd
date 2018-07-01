using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BronePoezd
{
    class MouseController : MonoBehaviour
    {
        #region InstanceAcessor
        static MouseController instance;
        public static MouseController GetInstance()
        {
            return instance;
        }
        #endregion

        public event Action<GameObject> ClickLMBevent;
        public event Action<GameObject> ClickRMBevent;


        private void Awake()
        {
            instance = this;
            ClickLMBevent += EmptyMBClickHander;
            ClickRMBevent += EmptyMBClickHander;
        }

        private void Update()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    HandleMBClick(ClickLMBevent);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    HandleMBClick(ClickRMBevent);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        void HandleMBClick(Action<GameObject> handledEvent)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, 1), 200);
            if (hit.collider != null)
            {
                handledEvent(hit.collider.gameObject);
            }
            else
            {
                handledEvent(null);
            }
        }

        void EmptyMBClickHander (GameObject clickedObj) { }
    }
}
