using MelonLoader;
using UnityEngine.InputSystem;

[assembly: MelonInfo(typeof(APNestClient.ModMain), "AP Nest Client", "0.0.1", "vergeslichlp")]

namespace APNestClient
{
    public class ModMain : MelonMod
    {
        private ConnectUI _connectUI;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("APNest Client loaded.");

            _connectUI = new ConnectUI();
            APSession apSession = _connectUI.GetApSession();

            MissionCompleteChecks.LocationCompleted += name => apSession.SendLocationChecks(new []{name});
        }

        public override void OnGUI()
        {
            _connectUI.Draw();
        }

        public override void OnUpdate()
        {
            if (Keyboard.current != null && Keyboard.current.f8Key.wasPressedThisFrame)
            {
                _connectUI.ToggleVisibility();
            }
        }
    }
}
