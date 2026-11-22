using APNestClient.ModLoader;
using UnityEngine.InputSystem;

namespace APNestClient
{
    public class ModCore
    {
        private ConnectUI _connectUI;
        private ItemReceiver _itemReceiver;
        private APSession _apSession;
        private MainMenuAPHook _mainMenuAPHook;

        private bool _initialized;
        private bool _debugMode;

        public void Initialize(ModConfig config)
        {
            Logger.Msg("APNest Client loaded.");

            _connectUI = new ConnectUI(config);
            _apSession = _connectUI.GetApSession();
            _itemReceiver = new ItemReceiver();
            _mainMenuAPHook = new MainMenuAPHook(_connectUI);
            _debugMode = config.DebugMode;
            if (_debugMode)
            {
                Logger.Warning("Debug mode enabled: F10 finishes the current mission with all Gold medals.");
            }

            MissionCompleteChecks.LocationCompleted += name => _apSession.SendLocationChecks(new []{name});
            MedalAchievedChecks.LocationCompleted += name => _apSession.SendLocationChecks(new []{name});
            DeathLinkSendHook.MissionFailed += () => _apSession.SendDeathLink();

            _initialized = true;
        }

        public void Update()
        {
            if (!_initialized)
            {
                return;
            }

            _apSession.ProcessPendingItems();
            _apSession.ProcessPendingDeathLinks();
            MissionReadyGuard.RegisterMissionChangedEventHook();
            _itemReceiver.ProcessPendingMissionLoad();
            _mainMenuAPHook.RegisterMainMenuHooks();

            if (_debugMode)
            {
                DebugFinishMission.PollHotkey();
            }
        }
    }
}
