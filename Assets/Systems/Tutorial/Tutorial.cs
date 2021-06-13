using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] List<TutorialStep> tutorialSteps;

    int currentStep;
    bool inTutorial;

    void Awake()
    {
        foreach (var step in tutorialSteps)
        {
            step.Hide();
        }

        StartCoroutine(TutorialRoutine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.F1) && !inTutorial)
        {
            StartCoroutine(TutorialRoutine());
        }
    }

    IEnumerator TutorialRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        GlobalEventBus.Bus.Pub(new CameraLock(true));
        currentStep = 0;
        inTutorial = true;
        while (currentStep < tutorialSteps.Count)
        {
            tutorialSteps[currentStep].Show();

            yield return new WaitForSeconds(0.5f);

            yield return new WaitUntil(() => Input.anyKeyDown);
            tutorialSteps[currentStep].Hide();
            currentStep++;
        }
        inTutorial = false;
        GlobalEventBus.Bus.Pub(new CameraLock(false));
    }
}

public struct ShowTutorial : IMessage
{
    public TutorialStep Tutorial;
}

public abstract class TutorialStep : MonoBehaviour
{
    public virtual void Show()
    {
        gameObject.SetActive(true);
        InternalShow();
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    protected abstract void InternalShow();
}
