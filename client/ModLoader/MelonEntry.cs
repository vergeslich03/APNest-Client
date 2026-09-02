#if !BEPINEX
using APNestClient.ModLoader;
using MelonLoader;

[assembly:MelonInfo(typeof(APNestClient.MelonEntry), "APNest-Client", "0.2.0", "vergeslich03")]

namespace APNestClient;

public class MelonEntry : MelonMod
{
    private ModCore _core;

    public override void OnInitializeMelon()
    {
        _core = new ModCore();
        _core.Initialize(new ModConfig());
    }

    public override void OnUpdate() => _core.Update();
}
#endif