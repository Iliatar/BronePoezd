using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    Transform cameraTransform;
    [SerializeField]
    float scrollSpeed, scrollEdgeSize;
    [SerializeField]
    float maxCamSize, minCamSize;
    float worldWidth, worldHeight;
    Camera cameraComponent;

    public void InitializeCamera(float worldWidth, float worldHeight)
    {
        cameraTransform = GetComponent<Transform>();
        cameraComponent = GetComponent<Camera>();
        this.worldWidth = worldWidth;
        this.worldHeight = worldHeight;
        cameraTransform.position = new Vector3(worldWidth / 2, worldHeight / 2, -10);
    }

    private void Update()
    {
        MoveCamera();

        ResizeCamera();
    }

    private void ResizeCamera()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float sizeDelta = -Input.GetAxis("Mouse ScrollWheel");
            if (sizeDelta < 0 && cameraComponent.orthographicSize > minCamSize)
            {
                cameraComponent.orthographicSize += sizeDelta;
            }

            if (sizeDelta > 0 && cameraComponent.orthographicSize < maxCamSize)
            {
                cameraComponent.orthographicSize += sizeDelta;
            }
        }
    }

    private void MoveCamera()
    {
        Vector3 cameraDeltaScroll = new Vector3(0, 0, 0);

        if (Input.mousePosition.x < scrollEdgeSize)
        {
            if (cameraTransform.position.x >= 0)
            {
                cameraDeltaScroll += new Vector3(-1, 0, 0);

            }
        }

        if (Input.mousePosition.x > cameraComponent.pixelWidth - scrollEdgeSize)
        {
            if (cameraTransform.position.x <= worldWidth)
            {
                cameraDeltaScroll += new Vector3(1, 0, 0);

            }
        }

        if (Input.mousePosition.y < scrollEdgeSize)
        {
            if (cameraTransform.position.y >= 0)
            {
                cameraDeltaScroll += new Vector3(0, -1, 0);
            }
        }

        if (Input.mousePosition.y > cameraComponent.pixelHeight - scrollEdgeSize)
        {
            if (cameraTransform.position.y <= worldHeight)
            {
                cameraDeltaScroll += new Vector3(0, 1, 0);
            }
        }

        if (cameraDeltaScroll.magnitude > 0)
        {
            cameraTransform.position += cameraDeltaScroll.normalized * Time.deltaTime * scrollSpeed;
        }
    }

}
