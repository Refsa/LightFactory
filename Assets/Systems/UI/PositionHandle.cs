using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PositionHandle : Handle
{
    [SerializeField] float snap = 1f;
    [SerializeField] float precisionScale = 0.25f;

    float offset = -0.25f;

    Vector2 moveTotal;

    protected override Vector3 defaultPosition => transform.parent.position + transform.parent.up * offset;
    public override Vector2 Value
    {
        get
        {
            return dragDelta;
        }
    }

    void Awake()
    {
        mouseDragging += val => moveTotal += val;
        mouseLeft += () => moveTotal = Vector2.zero;
    }

    public void SetData(GameObject targetObject)
    {
        // offset = targetObject.transform.localScale.y * -0.6f;
        offset = 0f;
    }

    public void Handle(GameObject targetObject)
    {
        if (Dragging)
        {
            float snap = this.snap;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                snap *= precisionScale;
            }

            float signX = Mathf.Sign(moveTotal.x);
            float signY = Mathf.Sign(moveTotal.y);

            Vector2 move = moveTotal;
            move.x -= (Mathf.Abs(move.x) % snap) * signX;
            move.y -= (Mathf.Abs(move.y) % snap) * signY;
            moveTotal -= move;

            Vector3 pos = targetObject.transform.position + move.ToVector3();

            pos.x = Mathf.Round(pos.x / snap) * snap;
            pos.y = Mathf.Round(pos.y / snap) * snap;

            targetObject.transform.position = pos;
        }
    }
}
