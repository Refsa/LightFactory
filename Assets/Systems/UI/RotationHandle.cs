using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class RotationHandle : Handle
{
    Vector3 direction = Vector2.up;

    protected override Vector3 defaultPosition => transform.parent.position + direction * 0.5f;

    void OnEnable()
    {
        mouseDragging += OnDragging;
    }

    void OnDisable()
    {
        mouseDragging -= OnDragging;
    }

    void OnDragging(Vector2 delta)
    {
        transform.position += delta.ToVector3();
    }

    public void SetData(GameObject targetObject)
    {
        direction = targetObject.transform.right;
    }

    public void Handle(GameObject targetObject)
    {
        if (Dragging)
        {
            float snap = GameConstants.RotationMajorSnap;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                snap = GameConstants.RotationMinorSnap;
            }

            Vector3 dir = transform.localPosition.normalized;
            float angle = Vector2.SignedAngle(dir, Vector2.up);
            angle = Mathf.Round(angle / snap) * snap;
            angle *= -1f;
            targetObject.transform.rotation = Quaternion.Euler(0f, 0f, 90f + angle);
        }
    }
}
