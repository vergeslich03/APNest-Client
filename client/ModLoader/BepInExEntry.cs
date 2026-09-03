#if BEPINEX
using System.Reflection;
using APNestClient.ModLoader;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Logger = APNestClient.ModLoader.Logger;

namespace APNestClient;

[BepInPlugin(Guid, "APNest-Client", "0.2.0")]
public class BepInExEntry : BasePlugin
{
    private const string Guid = "vergeslich03.apnestclient";

    public override void Load()
    {
        Logger.Init(Log);

        // MelonLoader applies [HarmonyPatch] classes automatically as part of registering
        // a MelonMod. BepInEx does not, so without this call every patch class is not registered and does absolutely nothing
        new Harmony(Guid).PatchAll(Assembly.GetExecutingAssembly());

        ModCore core = new ModCore();
        core.Initialize(new ModConfig(Config));

        TickBehaviour.Core = core;
        AddComponent<TickBehaviour>();
    }
}
#endif
