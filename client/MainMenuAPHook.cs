using System;
using Il2Cpp;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.Events;
using Logger = APNestClient.ModLoader.Logger;
using Object = UnityEngine.Object;

namespace APNestClient;

public class MainMenuAPHook
{
    private ConnectUI _connectUI;

    private GameObject _apButtonRef;

    private bool _mainMenuHooksRegistered = false;

    public MainMenuAPHook(ConnectUI connectUI)
    {
        _connectUI = connectUI;
    }

    public void RegisterMainMenuHooks()
    {
        if (_mainMenuHooksRegistered || MissionManager.Instance == null)
        {
            return;
        }

        Action<string> loadHandler = sceneName =>
        {
            HandleMainMenuLoaded(sceneName);
        };
        Action<string> unloadHandler = sceneName =>
        {
            HandleMainMenuUnloaded(sceneName);
        };

        MissionManager.Instance.MainMenuLoaded += loadHandler;
        MissionManager.Instance.MainMenuUnloaded += unloadHandler;
        _mainMenuHooksRegistered = true;
        Logger.Msg("[MainMenuAPHook] Subscribed to MainMenuLoaded/Unloaded.");
    }

    private void HandleMainMenuLoaded(string sceneName)
    {
        if (_apButtonRef != null)
        {
            return;
        }

        GameObject interactableParent = GameObject.Find("MainMenu Interactable objects");
        GameObject buttonRef = GameObject.Find("Credits Button");

        if (interactableParent == null || buttonRef == null)
        {
            return;
        }

        GameObject apConnectButton = Object.Instantiate(buttonRef, interactableParent.transform);
        apConnectButton.name = "AP Connect Button";
        
        float offsetForward = 0.5f;
        float offsetDown = -0.4f;
        float offsetZ = 1.6f;
        apConnectButton.transform.localPosition += new Vector3(offsetForward, offsetDown, offsetZ);

        float tiltBackDegrees = 80f;
        float tiltRight = 15f;
        apConnectButton.transform.localRotation *= Quaternion.Euler(tiltBackDegrees, tiltRight, 0f);

        LookAtTarget lookAtTarget = apConnectButton.GetComponent<LookAtTarget>();
        
        for (int i = 0; i < lookAtTarget.onClickDown.GetPersistentEventCount(); i++)
        {
            lookAtTarget.onClickDown.SetPersistentListenerState(i, UnityEventCallState.Off);
        }
        for (int i = 0; i < lookAtTarget.onClickUp.GetPersistentEventCount(); i++)
        {
            lookAtTarget.onClickUp.SetPersistentListenerState(i, UnityEventCallState.Off);
        }
        
        Action clickHandler = () =>
        {
            _connectUI.ToggleVisibility();
        };
        lookAtTarget.RegisterOnClickUp(clickHandler);

        Animator animator = apConnectButton.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        AnimatorBoolToggler[] togglers = apConnectButton.GetComponents<AnimatorBoolToggler>();
        foreach (AnimatorBoolToggler toggler in togglers)
        {
            toggler.enabled = false;
        }

        TextMeshPro[] labels = apConnectButton.GetComponentsInChildren<TextMeshPro>(true);
        foreach (TextMeshPro label in labels)
        {
            label.text = "AP";

            StaticLocalisedText localisedText = label.GetComponent<StaticLocalisedText>();
            if (localisedText != null)
            {
                Object.Destroy(localisedText);
            }
        }

        _apButtonRef = apConnectButton;
        _connectUI.BuildClipboard();
    }

    private void HandleMainMenuUnloaded(string sceneName)
    {
        _apButtonRef = null;
    }
}