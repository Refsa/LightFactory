using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PositionHandle : Handle
{
    float offset = -0.25f;
    Vector2 moveTotal;

    protected override Vector3 defaultPosition => transform.parent.position + transform.parent.up * offset;

    void OnEnable()
    {
        mouseDragging += OnDragging;
        mouseLeft += Reset;
    }

    void OnDisable()
    {
        mouseDragging -= OnDragging;
        mouseLeft -= Reset;
    }

    void OnDragging(Vector2 delta)
    {
        moveTotal += delta;
    }

    void Reset()
    {
        moveTotal = Vector2.zero;
    }

    public void SetData(GameObject targetObject)
    {
        offset = 0f;
        transform.localScale = targetObject.transform.localScale * 0.25f;
    }

    public void Handle(GameObject targetObject)
    {
        if (Dragging)
        {
            float snap = GameConstants.GridMajorSnap;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                snap = GameConstants.GridMinorSnap;
            }

            float signX = Mathf.Sign(moveTotal.x);
            float signY = Mathf.Sign(moveTotal.y);

            Vector2 move = moveTotal;
            move.x -= (Mathf.Abs(move.x) % snap) * signX;
            move.y -= (Mathf.Abs(move.y) % snap) * signY;
            moveTotal -= move;

            Vector3 pos = (targetObject.transform.position + move.ToVector3()).Snap(snap);

            targetObject.transform.position = pos;
        }
        else
        {
            moveTotal = Vector2.zero;
        }
    }
}
