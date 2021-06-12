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

        rotationHandle.gameObject.SetActive(rotateable != null);
        positionHandle.gameObject.SetActive(movable != null);

        positionHandle.SetData(movable);
        rotationHandle.GetData(rotateable);
    }

    public void Tick()
    {
        if (movable != null)
        {
            positionHandle.SetData(movable);
            if (positionHandle.Active)
            {
                movable.transform.position += positionHandle.Value.ToVector3();
            }
        }

        if (rotateable != null)
        {
            if (rotationHandle.Active)
            {
                rotateable.transform.rotation = Quaternion.Euler(0f, 0f, 90f) * Quaternion.FromToRotation(Vector3.up, rotationHandle.Value.ToVector3());
            }
            else
            {
                rotationHandle.GetData(rotateable);
            }
        }
    }
}
