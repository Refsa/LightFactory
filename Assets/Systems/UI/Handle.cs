using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Handle : MonoBehaviour
{
    public enum State
    {
        None = 0,
        MouseDrag,
        MouseHover,
    }

    State state;
    Vector2 lastMousePosition;
    protected Vector2 dragDelta;

    bool wasDown;
    bool wasPressedOnThis;

    protected abstract Vector3 defaultPosition { get; }
    public abstract Vector2 Value { get; }
    public bool Active => state != State.None;

    public event System.Action mouseEntered;
    public event System.Action mouseLeft;
    public event System.Action mouseHovering;
    public event System.Action<Vector2> mouseDragging;

    void Awake()
    {
        state = State.None;
    }

    void Update()
    {
        if (state != State.MouseDrag)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                defaultPosition,
                Mathf.SmoothStep(0f, 1f, Mathf.Pow(Time.deltaTime, 0.25f))
            );
        }

        if (state == State.MouseDrag)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                DragHandle();
                mouseDragging?.Invoke(dragDelta);
            }
            else
            {
                mouseLeft?.Invoke();
                ResetState();
            }
        }
    }

    void OnMouseEnter()
    {
        mouseEntered?.Invoke();
    }
    void OnMouseExit()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            return;
        }

        mouseLeft?.Invoke();

        ResetState();
    }
    void OnMouseOver()
    {
        mouseHovering?.Invoke();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            wasPressedOnThis = true;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lastMousePosition = mousePos;
        }

        bool isDown = Input.GetKey(KeyCode.Mouse0);
        if (isDown && wasPressedOnThis)
        {
            if (wasDown)
            {
                DragHandle();

                mouseDragging?.Invoke(dragDelta);
            }

            wasDown = true;
        }
        else
        {
            ResetState();
            state = State.MouseHover;
        }
    }

    void ResetState()
    {
        state = State.None;
        wasDown = false;
        wasPressedOnThis = false;
    }

    void DragHandle()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragDelta = mousePos - lastMousePosition;
        state = State.MouseDrag;
        lastMousePosition = mousePos;
    }
}
