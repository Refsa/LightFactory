using Refsa.EventBus;
using UnityEngine;

public class TransformHandleTutorial : TutorialStep
{
    protected override void InternalShow()
    {

    }

    public override void Show()
    {
        base.Show();

        GlobalEventBus.Bus.Pub(new TransformHandleToggle(true));
    }

    public override void Hide()
    {
        base.Hide();

        GlobalEventBus.Bus.Pub(new TransformHandleToggle(false));
    }
}