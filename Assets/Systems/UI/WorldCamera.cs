using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class WorldCamera : MonoBehaviour
{
    [SerializeField] float panSpeed = 0.025f;
    [SerializeField, Range(0f, 1f)] float panSmooth = 0.5f;

    [SerializeField] float zoomSpeed = 0.5f;
    [SerializeField, Range(0f, 1f)] float zoomSmooth = 0.5f;
    [SerializeField] Vector2 minMaxZoom = new Vector2(0.2f, 300f);

    [AutoBind] new Camera camera;

    Vector3 targetPos;

    float targetZoom;
    float lastScrollY;

    void Awake()
    {
        targetZoom = camera.orthographicSize;
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            Vector3 worldPos = camera.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = transform.position.z;
            targetPos = worldPos;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            targetPos = transform.position;
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            Vector2 mouseDelta = new Vector2(
                Input.GetAxis("Mouse X"),
                Input.GetAxis("Mouse Y")
            );

            targetPos -= (mouseDelta * (camera.orthographicSize * panSpeed)).ToVector3();
        }

        targetPos.z = -10f;

        transform.position = Vector3.Lerp(transform.position, targetPos, Mathf.Pow(Time.deltaTime, panSmooth));

        // Zoom
        {
            Vector2 scrollDelta = Input.mouseScrollDelta;

            if (lastScrollY == 0f)
            {
                if (scrollDelta.y != 0f)
                {
                    if (scrollDelta.y > 0f)
                    {
                        targetZoom += zoomSpeed * camera.orthographicSize;
                    }
                    else
                    {
                        targetZoom -= zoomSpeed * camera.orthographicSize;
                    }
                    
                    targetZoom = Mathf.Clamp(targetZoom, minMaxZoom.x, minMaxZoom.y);
                }
            }

            lastScrollY = scrollDelta.y;

            camera.orthographicSize =
                Mathf.Lerp(camera.orthographicSize, targetZoom, Mathf.Pow(Time.deltaTime, zoomSmooth));
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minMaxZoom.x, minMaxZoom.y);
        }
    }
}