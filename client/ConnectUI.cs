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

        this._apHost = apHostEntry;
        this._apPort = apPortEntry;
        this._apSlotName = apSlotEntry;
        this._apPassword = "";
        this._preferenceCategory = category;

        this._window = new Rect((Screen.width / 2) - 100f, 10f, 200f, 100f);
    }

    public void Persist()
    {
        this._preferenceCategory.SaveToFile();
    }

    public void ToggleVisibility()
    {
        this._visible = !this._visible;
    }

    public void Draw()
    {
        if (this._visible)
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
        }
    }

    public string GetApHost()
    {
        return this._apHost.Value;
    }

    public int GetApPort()
    {
        return this._apPort.Value;
    }
    
    public string GetApSlotName()
    {
        return this._apSlotName.Value;
    }

    public string GetApPassword()
    {
        return this._apPassword;
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