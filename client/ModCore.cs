using APNestClient.ModLoader;

namespace APNestClient
{
    public class ModCore
    {
        private ConnectUI _connectUI;
        private ItemReceiver _itemReceiver;
        private APSession _apSession;
        private MainMenuAPHook _mainMenuAPHook;

        public void Initialize(ModConfig config)
        {
            Logger.Msg("APNest Client loaded.");

            _connectUI = new ConnectUI(config);
            _apSession = _connectUI.GetApSession();
            _itemReceiver = new ItemReceiver();
            _mainMenuAPHook = new MainMenuAPHook(_connectUI);

            MissionCompleteChecks.LocationCompleted += name => _apSession.SendLocationChecks(new []{name});
            MedalAchievedChecks.LocationCompleted += name => _apSession.SendLocationChecks(new []{name});
        }

        public void Update()
        {
            _apSession.ProcessPendingItems();
            _itemReceiver.RegisterMissionChangedEventHook();
            _itemReceiver.ProcessPendingMissionLoad();
            _mainMenuAPHook.RegisterMainMenuHooks();
        }
    }
}
