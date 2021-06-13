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

    protected State state;
    State previousState;

    Vector2 lastMousePosition;
    protected Vector2 dragDelta;

    bool wasDown;
    bool wasPressedOnThis;

    protected abstract Vector3 defaultPosition { get; }
    public bool Active => state != State.None;
    public bool Dragging => state == State.MouseDrag;

    public event System.Action mouseEntered;
    public event System.Action mouseLeft;
    public event System.Action mouseHovering;
    public event System.Action<Vector2> mouseDragging;
    public event System.Action mouse0Clicked;

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
        else if (state == State.MouseDrag)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                DragHandle();
                mouseDragging?.Invoke(dragDelta);
            }
            else
            {
                ResetState();
            }
        }

        if (previousState != State.None && state == State.None)
        {
            mouseLeft?.Invoke();
        }

        previousState = state;
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

        ResetState();
    }
    void OnMouseOver()
    {
        mouseHovering?.Invoke();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            wasPressedOnThis = true;
            lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            mouse0Clicked?.Invoke();
        }

        bool isDown = Input.GetKey(KeyCode.Mouse0);
        if (isDown && wasPressedOnThis)
        {
            if (wasDown)
            {
                state = State.MouseDrag;
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
