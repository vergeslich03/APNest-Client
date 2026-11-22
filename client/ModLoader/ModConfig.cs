#if BEPINEX
using BepInEx.Configuration;
#else
using MelonLoader;
#endif

namespace APNestClient.ModLoader;

public class ModConfig
{
#if BEPINEX
    private readonly ConfigFile _cfgFile;
    private readonly ConfigEntry<string> _apHost;
    private readonly ConfigEntry<int> _apPort;
    private readonly ConfigEntry<string> _apSlotName;
    private readonly ConfigEntry<bool> _debugMode;
    
    public ModConfig(ConfigFile cfgFile)
    {
        _cfgFile = cfgFile;
        _apHost = cfgFile.Bind("APNestClient", "APHost", "archipelago.gg");
        _apPort = cfgFile.Bind("APNestClient", "APPort", 38281);
        _apSlotName = cfgFile.Bind("APNestClient", "APSlotName", "IronNest");
        _debugMode = cfgFile.Bind("APNestClient", "DebugMode", false, "Enables debug hotkeys (F10: finish current mission with all Gold medals)");
    }
    
    public void Save() => _cfgFile.Save();
#else
    private readonly MelonPreferences_Category _category;
    private readonly MelonPreferences_Entry<string> _apHost;
    private readonly MelonPreferences_Entry<int> _apPort;
    private readonly MelonPreferences_Entry<string> _apSlotName;
    private readonly MelonPreferences_Entry<bool> _debugMode;

    public ModConfig()
    {
        _category = MelonPreferences.CreateCategory("APNestClient");
        _apHost = _category.CreateEntry("APHost", "archipelago.gg");
        _apPort = _category.CreateEntry("APPort", 38281);
        _apSlotName = _category.CreateEntry("APSlotName", "IronNest");
        _debugMode = _category.CreateEntry("DebugMode", false, description: "Enables debug hotkeys (F10: finish current mission with all Gold medals)");
    }

    public void Save() => _category.SaveToFile();
#endif
    
    public string Host { get => _apHost.Value; set => _apHost.Value = value; }
    public int Port { get => _apPort.Value; set => _apPort.Value = value; }
    public string SlotName { get => _apSlotName.Value; set => _apSlotName.Value = value; }
    public bool DebugMode => _debugMode.Value;
}