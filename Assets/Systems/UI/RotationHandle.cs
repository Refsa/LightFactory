using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationHandle : Handle
{
    protected override Vector3 defaultPosition => transform.parent.position + direction * 0.5f;
    public override Vector2 Value => transform.localPosition.normalized;

    Vector3 direction = Vector2.up;

    void Awake()
    {
        mouseDragging += val =>
        {
            transform.position += val.ToVector3();
        };
    }

    public void GetData(GameObject targetObject)
    {
        direction = targetObject.transform.right;
    }
}
