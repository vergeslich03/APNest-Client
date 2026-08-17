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
        MelonPreferences_Category category = MelonPreferences.CreateCategory("APNestClient");
        MelonPreferences_Entry<string> apHostEntry = category.CreateEntry("APHost", "archipelago.gg");
        MelonPreferences_Entry<int> apPortEntry = category.CreateEntry("APPort", 38281);
        MelonPreferences_Entry<string> apSlotEntry = category.CreateEntry("APSlot", "IRON NEST");
        
        this._apHost = apHostEntry;
        this._apPort = apPortEntry;
        this._apSlotName = apSlotEntry;
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
            _window = GUILayout.Window(
                1,
                _window,
                new System.Action<int>(DrawWindowContents),
                "Archipelago Connection"
            );
        }
    }

    private void DrawWindowContents(int windowID)
    {
        GUILayout.Label("Archipelago Host:");
        SetApHost(GUILayout.TextField(GetApHost()));
        GUILayout.Label("Archipelago Port:");
        if (int.TryParse(GUILayout.TextField(GetApPort().ToString()), out int port))
        {
            SetApPort(port);
        }
        GUILayout.Label("Archipelago Slot:");
        SetApSlotName(GUILayout.TextField(GetApSlotName()));
        GUILayout.Label("Archipelago Password:");
        SetApPassword(GUILayout.PasswordField(GetApPassword(), '*'));
        if (GUILayout.Button("Connect"))
        {
            MelonLogger.Msg("Connect Button clicked. Connection Info: " + GetApHost() + "/" + GetApPort() + "/" + GetApSlotName());
            Persist();
        }
        
        GUI.DragWindow(new Rect(0f, 0f, 200f, 20f));
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