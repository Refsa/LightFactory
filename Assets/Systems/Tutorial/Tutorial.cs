using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] List<TutorialStep> tutorialSteps;

    int currentStep;

    void Awake()
    {
        foreach (var step in tutorialSteps)
        {
            step.Hide();
        }

        StartCoroutine(TutorialRoutine());
    }

    IEnumerator TutorialRoutine()
    {
        while (currentStep < tutorialSteps.Count)
        {
            tutorialSteps[currentStep].Show();

            yield return new WaitForSeconds(0.5f);

            yield return new WaitUntil(() => Input.anyKeyDown);
            tutorialSteps[currentStep].Hide();
            currentStep++;
        }
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
