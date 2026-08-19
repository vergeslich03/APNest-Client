using MelonLoader;
using UnityEngine;

namespace APNestClient;

public class ConnectUI
{
    private MelonPreferences_Category _preferenceCategory;
    
    private MelonPreferences_Entry<string> _apHost;
    private MelonPreferences_Entry<int> _apPort;
    private MelonPreferences_Entry<string> _apSlotName;
    private string _apPassword;
    private APSession _apSession;

    private bool _visible;
    private Rect _window;
    
    public ConnectUI()
    {
        // TODO: text-entry is hardcoded for now — GUI.TextField/PasswordField throw
        // (GUIStateObjects.GetStateObject "Method unstripping failed", see
        // reference/iron-nest-api-notes.md). Revisit once a workaround is found, and
        // investigate the main-menu envelope-on-the-books spot for a diegetic trigger
        // instead of the current F8 keybind.
        MelonPreferences_Category category = MelonPreferences.CreateCategory("APNestClient");
        MelonPreferences_Entry<string> apHostEntry = category.CreateEntry("APHost", "localhost");
        MelonPreferences_Entry<int> apPortEntry = category.CreateEntry("APPort", 38281);
        MelonPreferences_Entry<string> apSlotEntry = category.CreateEntry("APSlot", "Player1");

        _preferenceCategory = category;
        
        _apHost = apHostEntry;
        _apPort = apPortEntry;
        _apSlotName = apSlotEntry;
        _apPassword = "";
        _apSession =  new APSession();

        _window = new Rect((Screen.width / 2) - 100f, 10f, 200f, 100f);
    }

    public void Persist()
    {
        _preferenceCategory.SaveToFile();
    }

    public void ToggleVisibility()
    {
        _visible = !_visible;
    }

    public void Draw()
    {
        if (_visible)
        {
            DrawWindowContents();
        }
    }

    private void DrawWindowContents()
    {
        const float rowHeight = 20f;
        const float labelWidth = 90f;
        const float fieldWidth = 100f;

        float x = _window.x;
        float y = _window.y;

        // Read-only for now — see the hardcoded-values TODO in the constructor.
        GUI.Label(new Rect(x, y, labelWidth, rowHeight), "Host:");
        GUI.Label(new Rect(x + labelWidth, y, fieldWidth, rowHeight), GetApHost());
        y += rowHeight;

        GUI.Label(new Rect(x, y, labelWidth, rowHeight), "Port:");
        GUI.Label(new Rect(x + labelWidth, y, fieldWidth, rowHeight), GetApPort().ToString());
        y += rowHeight;

        GUI.Label(new Rect(x, y, labelWidth, rowHeight), "Slot:");
        GUI.Label(new Rect(x + labelWidth, y, fieldWidth, rowHeight), GetApSlotName());
        y += rowHeight;

        GUI.Label(new Rect(x, y, labelWidth, rowHeight), "Password:");
        GUI.Label(new Rect(x + labelWidth, y, fieldWidth, rowHeight), string.IsNullOrEmpty(GetApPassword()) ? "(none)" : "****");
        y += rowHeight;

        if (GUI.Button(new Rect(x, y, labelWidth + fieldWidth, rowHeight), "Connect"))
        {
            MelonLogger.Msg("Connect Button clicked. Connection Info: " + GetApHost() + "/" + GetApPort() + "/" + GetApSlotName());
            Persist();
            _apSession.Connect(GetApHost(), GetApPort(), GetApSlotName(),  GetApPassword());
        }
    }

    public string GetApHost()
    {
        return _apHost.Value;
    }

    public int GetApPort()
    {
        return _apPort.Value;
    }
    
    public string GetApSlotName()
    {
        return _apSlotName.Value;
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
        this._apHost.Value = host;
    }

    public void SetApPort(int port)
    {
        this._apPort.Value = port;
    }
    
    public void SetApSlotName(string slotName)
    {
        this._apSlotName.Value = slotName;
    }

    public void SetApPassword(string password)
    {
        this._apPassword = password;
    }
}