using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class RotationHandle : Handle
{
    [SerializeField] float snap = 22.5f;
    [SerializeField] float precisionScale = 0.25f;

    Vector3 direction = Vector2.up;

    protected override Vector3 defaultPosition => transform.parent.position + direction * 0.5f;
    public override Vector2 Value
    {
        get
        {
            Vector3 dir = transform.localPosition.normalized;
            float angle = Vector2.SignedAngle(dir, Vector2.up);
            return Quaternion.Euler(0f, 0f, angle) * Vector2.up;
        }
    }

    void Awake()
    {
        mouseDragging += val =>
        {
            transform.position += val.ToVector3();
        };
    }

    public void SetData(GameObject targetObject)
    {
        direction = targetObject.transform.right;
    }

    public void Handle(GameObject targetObject)
    {
        float angle = 0f;

        if (Dragging)
        {
            float snap = this.snap;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                snap *= precisionScale;
            }

            Vector3 dir = transform.localPosition.normalized;
            angle = Vector2.SignedAngle(dir, Vector2.up);
            angle = Mathf.Round(angle / snap) * snap;
            angle *= -1f;
            targetObject.transform.rotation = Quaternion.Euler(0f, 0f, 90f + angle);
        }
    }
}
