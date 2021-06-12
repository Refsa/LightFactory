using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformHandle : MonoBehaviour
{
    [SerializeField] PositionHandle positionHandle;
    [SerializeField] RotationHandle rotationHandle;

    public bool Active => positionHandle.Active || rotationHandle.Active;

    GameObject rotateable;
    GameObject movable;

    public void SetData(GameObject targetObject)
    {
        if (targetObject.HasTag("Movable"))
        {
            movable = targetObject;
        }
        else if (targetObject.HasTagInParent("Movable"))
        {
            movable = targetObject.transform.parent.gameObject;
        }
        else
        {
            movable = null;
        }

        if (targetObject.HasTag("Rotateable"))
        {
            rotateable = targetObject;
        }
        else if (targetObject.HasTagInParent("Rotateable"))
        {
            rotateable = targetObject.transform.parent.gameObject;
        }
        else
        {
            rotateable = null;
        }

        positionHandle.SetData(movable);
        rotationHandle.SetData(rotateable);

        rotationHandle.gameObject.SetActive(rotateable != null);
        positionHandle.gameObject.SetActive(movable != null);
    }

    public void Tick()
    {
        if (movable != null)
        {
            positionHandle.SetData(movable);
            positionHandle.Handle(movable);
        }

        if (rotateable != null)
        {
            rotationHandle.SetData(rotateable);
            rotationHandle.Handle(rotateable);
        }
    }
}
