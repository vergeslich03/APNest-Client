using System.Collections.Generic;
using APNestClient.ModLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logger = APNestClient.ModLoader.Logger;

namespace APNestClient;

public class ConnectUI
{
    private readonly ModConfig _modConfig;
    private string _apPassword;
    private APSession _apSession;

    private GameObject _apClipboard;

    private InputSystemSwitcher _inputSystemSwitcher;

    private TextMeshProUGUI _connectButtonLabel;
    private TMP_InputField _hostField;
    private TMP_InputField _portField;
    private TMP_InputField _slotField;
    private TMP_InputField _passwordField;
    private TMP_InputField _statusField;

    public ConnectUI(ModConfig modConfig)
    {
        _modConfig = modConfig;
        _apPassword = "";
        _apSession =  new APSession();
    }

    public void BuildClipboard()
    {
        GameObject clipboardParent = GameObject.Find("MainMenu Interactable objects");
        GameObject clipboardRef =  clipboardParent.transform.Find("Clipboard Menu").gameObject;

        GameObject clipboardClone = Object.Instantiate(clipboardRef, clipboardParent.transform);
        clipboardClone.name = "AP Connection Menu";

        clipboardClone.GetComponentInChildren<Interactable>(true).enabled = false;
        foreach (BoxCollider box in clipboardClone.GetComponentsInChildren<BoxCollider>(true))
        {
            if (box.gameObject.name == "Settings Button")
            {
                box.enabled = false;
            }
        }

        Transform layoutParent = clipboardClone.transform.Find("Canvas/Settings menu/Settings/ContentCtn/Content (Game)/Scroll View/Viewport/Content/Layout");
        GameObject rowTemplate = layoutParent.Find("TextfieldConsoleUGUI (DiscordKey)").gameObject;

        List<GameObject> originalRows = new();
        for (int i = 0; i < layoutParent.childCount; i++)
        {
            originalRows.Add(layoutParent.GetChild(i).gameObject);
        }
        
        GameObject apHostInput = Object.Instantiate(rowTemplate, layoutParent);
        GameObject apPortInput = Object.Instantiate(rowTemplate, layoutParent);
        GameObject apSlotInput = Object.Instantiate(rowTemplate, layoutParent);
        GameObject apPasswordInput = Object.Instantiate(rowTemplate, layoutParent);
        GameObject apStatusRow = Object.Instantiate(rowTemplate, layoutParent);

        foreach (GameObject row in originalRows)
        {
            Object.Destroy(row);
        }
        
        Object.Destroy(clipboardClone.transform.Find("Canvas/Settings menu/Settings/TabsCtn").gameObject);
        Object.Destroy(clipboardClone.transform.Find("Canvas/Settings menu/Settings/ContentCtn/Content (Graphics)").gameObject);
        Object.Destroy(clipboardClone.transform.Find("Canvas/Settings menu/Settings/ContentCtn/Content (Audio)").gameObject);
        Object.Destroy(clipboardClone.transform.Find("Canvas/Settings menu/Settings/ContentCtn/Content (Controls)").gameObject);
        Object.Destroy(clipboardClone.transform.Find("Canvas/Settings menu/Settings/ContentCtn/Content (Controls)Gamepad").gameObject);
        Object.Destroy(clipboardClone.transform.Find("Canvas/Settings menu/Settings/ContentCtn/ButtonSecondaryUGUI (reset all)").gameObject);
        
        GameObject titleObj = clipboardClone.transform.Find("Canvas/Settings menu/Settings/Title Settings").gameObject;
        StaticLocalisedText titleLocalised = titleObj.GetComponent<StaticLocalisedText>();
        if (titleLocalised != null)
        {
            Object.Destroy(titleLocalised);
        }
        titleObj.GetComponent<TextMeshProUGUI>().text = "Archipelago Settings";

        Transform scrollView = clipboardClone.transform.Find("Canvas/Settings menu/Settings/ContentCtn/Content (Game)/Scroll View");
        scrollView.GetComponent<UnityEngine.UI.ScrollRect>().vertical = false;
        Object.Destroy(scrollView.Find("Scrollbar Vertical").gameObject);

        apHostInput.name = "AP Host";
        apPortInput.name = "AP Port";
        apSlotInput.name = "AP Slot";
        apPasswordInput.name = "AP Password";
        apStatusRow.name = "AP Status";

        _hostField = SetupRow(apHostInput, "Host", "archipelago.gg", GetApHost(), TMP_InputField.ContentType.Standard, SetApHost);
        _portField = SetupRow(apPortInput, "Port", "38281", GetApPort().ToString(), TMP_InputField.ContentType.IntegerNumber, value =>
        {
            if (int.TryParse(value, out int port))
            {
                SetApPort(port);
            }
        });
        _slotField = SetupRow(apSlotInput, "Slot", "IronNest", GetApSlotName(), TMP_InputField.ContentType.Standard, SetApSlotName);
        _passwordField = SetupRow(apPasswordInput, "Password", "(none)", GetApPassword(), TMP_InputField.ContentType.Password, SetApPassword);
        _statusField = SetupStatusRow(apStatusRow);

        // Repurpose the settings "Apply" button as our Connect/Disconnect toggle.
        GameObject connectButtonObj = clipboardClone.transform.Find("Canvas/Settings menu/Settings/ContentCtn/SGButtonPrimaryUGUI (apply)").gameObject;
        Button connectButton = connectButtonObj.GetComponent<Button>();
        for (int i = 0; i < connectButton.onClick.GetPersistentEventCount(); i++)
        {
            connectButton.onClick.SetPersistentListenerState(i, UnityEventCallState.Off);
        }

        _connectButtonLabel = connectButtonObj.GetComponentInChildren<TextMeshProUGUI>(true);
        StaticLocalisedText connectLabelLocalized = _connectButtonLabel.GetComponent<StaticLocalisedText>();
        if (connectLabelLocalized != null)
        {
            Object.Destroy(connectLabelLocalized);
        }
        connectButton.onClick.AddListener((UnityAction)OnConnectClicked);
        
        GameObject closeButtonObj = clipboardClone.transform.Find("Canvas/Settings menu/Settings/ContentCtn/ButtonSecondaryUGUI (close)").gameObject;
        Button closeButton = closeButtonObj.GetComponent<Button>();
        for (int i = 0; i < closeButton.onClick.GetPersistentEventCount(); i++)
        {
            closeButton.onClick.SetPersistentListenerState(i, UnityEventCallState.Off);
        }
        closeButton.onClick.AddListener((UnityAction)Hide);

        _apClipboard = clipboardClone;

        _inputSystemSwitcher = Object.FindObjectOfType<InputSystemSwitcher>();
        if (_inputSystemSwitcher == null)
        {
            InputSystemSwitcher[] allSwitchers = Resources.FindObjectsOfTypeAll<InputSystemSwitcher>();
            if (allSwitchers.Length > 0)
            {
                _inputSystemSwitcher = allSwitchers[0];
            }
        }
        if (_inputSystemSwitcher == null)
        {
            Logger.Warning("[ConnectUI] InputSystemSwitcher not found — AP clipboard text input will not work.");
        }

        RefreshAPConnectionUI();
    }

    private void OnConnectClicked()
    {
        if (_apSession.GetConnectionState() == APConnectionState.Connected)
        {
            _apSession.Disconnect();
            RefreshAPConnectionUI();
            return;
        }

        SetApHost(_hostField.text.Trim());
        if (int.TryParse(_portField.text, out int port))
        {
            SetApPort(port);
        }
        SetApSlotName(_slotField.text.Trim());
        SetApPassword(_passwordField.text);
        Persist();

        _apSession.Connect(GetApHost(), GetApPort(), GetApSlotName(), GetApPassword());
        RefreshAPConnectionUI();
    }

    private void RefreshAPConnectionUI()
    {
        string buttonText;
        string statusText;

        switch (_apSession.GetConnectionState())
        {
            case APConnectionState.Connected:
                buttonText = "Disconnect";
                statusText = "Connected";
                break;
            case APConnectionState.Connecting:
                buttonText = "Connecting...";
                statusText = "Connecting...";
                break;
            case APConnectionState.Failed:
                buttonText = "Connect";
                string reason = _apSession.GetLoginFailureReason();
                statusText = "Failed: " + (string.IsNullOrEmpty(reason) ? "unknown error" : reason);
                break;
            default:
                buttonText = "Connect";
                statusText = "Not connected";
                break;
        }

        if (_connectButtonLabel != null)
        {
            _connectButtonLabel.text = buttonText;
        }
        if (_statusField != null)
        {
            _statusField.text = statusText;
        }
    }

    private TMP_InputField SetupRow(GameObject row, string label, string placeholder, string initialValue, TMP_InputField.ContentType contentType, System.Action<string> onEndEdit)
    {
        UILeaderboardOptOutListener optOut = row.GetComponent<UILeaderboardOptOutListener>();
        if (optOut != null)
        {
            Object.Destroy(optOut);
        }

        GameObject labelObj = row.transform.Find("Label").gameObject;
        StaticLocalisedText labelLocalised = labelObj.GetComponent<StaticLocalisedText>();
        if (labelLocalised != null)
        {
            Object.Destroy(labelLocalised);
        }
        labelObj.GetComponent<TextMeshProUGUI>().text = label;

        Transform inputFieldTransform = row.transform.Find("InputField (TMP)");
        TMP_InputField inputField = inputFieldTransform.GetComponent<TMP_InputField>();

        InputFieldHelper inputFieldHelper = inputFieldTransform.GetComponent<InputFieldHelper>();
        if (inputFieldHelper != null)
        {
            Object.Destroy(inputFieldHelper);
        }

        inputField.contentType = contentType;
        inputField.text = initialValue;

        inputField.interactable = true;
        inputField.readOnly = false;
        CanvasGroup canvasGroup = inputFieldTransform.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        inputFieldTransform.Find("Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = placeholder;

        inputField.onEndEdit.AddListener(onEndEdit);

        System.Action<string> onSelect = value =>
        {
            if (_inputSystemSwitcher != null)
            {
                _inputSystemSwitcher.EnableTextInput();
            }
        };
        System.Action<string> onDeselect = value =>
        {
            if (_inputSystemSwitcher != null)
            {
                _inputSystemSwitcher.DisableTextInput();
            }
        };
        
        inputField.onSelect.AddListener(onSelect);
        inputField.onDeselect.AddListener(onDeselect);

        return inputField;
    }

    private TMP_InputField SetupStatusRow(GameObject row)
    {
        UILeaderboardOptOutListener optOut = row.GetComponent<UILeaderboardOptOutListener>();
        if (optOut != null)
        {
            Object.Destroy(optOut);
        }

        GameObject labelObj = row.transform.Find("Label").gameObject;
        StaticLocalisedText labelLocalised = labelObj.GetComponent<StaticLocalisedText>();
        if (labelLocalised != null)
        {
            Object.Destroy(labelLocalised);
        }
        labelObj.GetComponent<TextMeshProUGUI>().text = "Status";

        Transform inputFieldTransform = row.transform.Find("InputField (TMP)");
        TMP_InputField inputField = inputFieldTransform.GetComponent<TMP_InputField>();

        InputFieldHelper inputFieldHelper = inputFieldTransform.GetComponent<InputFieldHelper>();
        if (inputFieldHelper != null)
        {
            Object.Destroy(inputFieldHelper);
        }

        inputField.readOnly = true;
        inputFieldTransform.Find("Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = "";

        return inputField;
    }

    public void Persist()
    {
        _modConfig.Save();
    }

    public void ToggleVisibility()
    {
        PickUpZoomTarget zoomTarget = _apClipboard.GetComponentInChildren<PickUpZoomTarget>();
        if (zoomTarget.IsHeld)
        {
            Hide();
            return;
        }
        zoomTarget.PickUp();
    }

    private void Hide()
    {
        PickUpZoomTarget zoomTarget = _apClipboard.GetComponentInChildren<PickUpZoomTarget>();
        if (!zoomTarget.IsHeld)
        {
            return;
        }
        zoomTarget.Release();

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        if (_inputSystemSwitcher != null)
        {
            _inputSystemSwitcher.DisableTextInput();
        }
    }

    public string GetApHost()
    {
        return _modConfig.Host;
    }

    public int GetApPort()
    {
        return _modConfig.Port;
    }
    
    public string GetApSlotName()
    {
        return _modConfig.SlotName;
    }

    public string GetApPassword()
    {
        return _apPassword;
    }

    public APSession GetApSession()
    {
        return _apSession;
    }

    public void SetApHost(string host)
    {
        _modConfig.Host = host;
    }

    public void SetApPort(int port)
    {
        _modConfig.Port = port;
    }
    
    public void SetApSlotName(string slotName)
    {
        _modConfig.SlotName = slotName;
    }

    public void SetApPassword(string password)
    {
        _apPassword = password;
    }
}