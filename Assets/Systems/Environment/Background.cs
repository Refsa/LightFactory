using System;
using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;

public class Background : MonoBehaviour
{
    Vector3[] fullscreenQuadVertices;
    [AutoBind] MeshFilter quadFilter;
    [AutoBind] MeshRenderer quadRenderer;

    void Awake()
    {
        GlobalEventBus.Bus.Sub<CameraZoomChanged>(OnCameraZoomChanged);

        fullscreenQuadVertices = new Vector3[4];
    }

    private void OnCameraZoomChanged(CameraZoomChanged obj)
    {
        fullscreenQuadVertices[0] = obj.Camera.transform.InverseTransformPoint(obj.Camera.ViewportToWorldPoint(new Vector3(0, 0, 0)));
        fullscreenQuadVertices[1] = obj.Camera.transform.InverseTransformPoint(obj.Camera.ViewportToWorldPoint(new Vector3(1.01f, 0, 0)));
        fullscreenQuadVertices[3] = obj.Camera.transform.InverseTransformPoint(obj.Camera.ViewportToWorldPoint(new Vector3(1.01f, 1.01f, 0)));
        fullscreenQuadVertices[2] = obj.Camera.transform.InverseTransformPoint(obj.Camera.ViewportToWorldPoint(new Vector3(0, 1.01f, 0)));

        quadFilter.mesh.SetVertices(fullscreenQuadVertices);
    }

    void LateUpdate()
    {

    }
}
