using System;
using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] Button quitButton;

    void Start()
    {
        continueButton.onClick.AddListener(OnContinueButton);
        quitButton.onClick.AddListener(OnQuitButton);

        GlobalEventBus.Bus.Sub<ThePrismCollected>(OnThePrismCollected);
        gameObject.SetActive(false);
    }

    private void OnQuitButton()
    {
        Application.Quit();
    }

    private void OnContinueButton()
    {
        gameObject.SetActive(false);
    }

    private void OnThePrismCollected(ThePrismCollected obj)
    {
        gameObject.SetActive(true);
    }
}
