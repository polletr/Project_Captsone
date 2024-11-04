using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private float _tutorialTime;
    [SerializeField] private TextMeshProUGUI _tutorialtext;
    [SerializeField] private TextMeshProUGUI _rechargetext;

    public GameEvent Event;
    public TutorialEvents TutorialEvent;

    public GlobalEventSO stunTutorial;
    public GlobalEventSO disappearTutorial;
    public GlobalEventSO revealTutorial;
    public GlobalEventSO swapAbilityTutorial;
    public GlobalEventSO flashlightOnTutorial;
    public GlobalEventSO flashlightOffTutorial;

    private CountdownTimer _countdownTimer;

    private Action currentFunction;
    private UnityAction currentTutorialEvent;

    private void Awake()
    {
        _tutorialtext.text = "";
        _rechargetext.text = "";
        _countdownTimer = new CountdownTimer(_tutorialTime);
    }

    private void OnEnable()
    {
        stunTutorial.OnTriggerGlobalEvent += StunText;
        disappearTutorial.OnTriggerGlobalEvent += DisappearText;
        flashlightOnTutorial.OnTriggerGlobalEvent += FlashlightOnText;
        flashlightOffTutorial.OnTriggerGlobalEvent += FlashlightOffText;
        revealTutorial.OnTriggerGlobalEvent += RevealText;
        swapAbilityTutorial.OnTriggerGlobalEvent += SwapAbilityText;


        Event.SetTutorialText += SetRechargeText;
        Event.SetTutorialTextTimer += SetTextTimer;
    }

    private void OnDisable()
    {
        stunTutorial.OnTriggerGlobalEvent -= StunText;
        disappearTutorial.OnTriggerGlobalEvent -= DisappearText;
        flashlightOnTutorial.OnTriggerGlobalEvent -= FlashlightOnText;
        flashlightOffTutorial.OnTriggerGlobalEvent -= FlashlightOffText;
        revealTutorial.OnTriggerGlobalEvent -= RevealText;
        swapAbilityTutorial.OnTriggerGlobalEvent -= SwapAbilityText;


        Event.SetTutorialText -= SetRechargeText;
        Event.SetTutorialTextTimer -= SetTextTimer;
    }

    private void Update()
    {
        _countdownTimer.Tick(Time.deltaTime);
        if (_countdownTimer.IsFinished)
        {
            _tutorialtext.text = "";
            _countdownTimer.Stop();
            _countdownTimer.Reset();
        }
    }

    public void SetTextTimer(string text)
    {
        _countdownTimer.Reset();
        _countdownTimer.Start();
        _rechargetext.text = text;
    }

    private void SetText(string text)
    {
        _tutorialtext.text = text;
    } 
    private void SetRechargeText(string text)
    {
        _rechargetext.text = text;
    }

    private void StunText()
    {
        SetText("Hold down left mouse to stun");
        TutorialEvent.OnStun += RemoveStunText;
    }

    private void DisappearText()
    {
        SetText("Hold down left mouse button to make highlighted objects disappear");
        TutorialEvent.OnDisappear += RemoveDisappearText;

    }

    private void FlashlightOnText()
    {
        SetText("Press F to turn ON flashlight");
        TutorialEvent.OnTurnOnFlashlight += RemoveFlashlightOnText;

    }

    private void FlashlightOffText()
    {
        SetText("Press F to turn OFF flashlight");
        TutorialEvent.OnTurnOffFlashlight += RemoveFlashlightOffText;
    }

    private void RevealText()
    {
        SetText("Hold down left mouse button to reveal hidden objects");
        TutorialEvent.OnReveal += RemoveRevealText;
    }

    private void SwapAbilityText()
    {
        SetText("Press 1-2-3 or roll mouse wheel to swap abilities");
        TutorialEvent.OnSwapAbility += RemoveSwapText;
    }

    #region Remove Text

    private void RemoveSwapText()
    {
        TutorialEvent.OnSwapAbility -= RemoveSwapText;
        _tutorialtext.text = "";
    }

    private void RemoveRevealText()
    {
        TutorialEvent.OnReveal -= RemoveRevealText;
        _tutorialtext.text = "";
    }
    
    private void RemoveFlashlightOffText()
    {
        TutorialEvent.OnTurnOffFlashlight -= RemoveFlashlightOffText;
        _tutorialtext.text = "";
    }
    
    private void RemoveFlashlightOnText()
    {
        TutorialEvent.OnTurnOnFlashlight -= RemoveFlashlightOnText;
        _tutorialtext.text = "";
    }
    
    private void RemoveStunText()
    {
        TutorialEvent.OnStun -= RemoveStunText;
        _tutorialtext.text = "";
    }
    
    private void RemoveDisappearText()
    {
        TutorialEvent.OnDisappear -= RemoveDisappearText;
        _tutorialtext.text = "";
    }

    #endregion
}