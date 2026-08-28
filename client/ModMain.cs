using MelonLoader;

[assembly: MelonInfo(typeof(APNestClient.ModMain), "AP Nest Client", "0.0.1", "vergeslich03")]

namespace APNestClient
{
    public class ModMain : MelonMod
    {
        private ConnectUI _connectUI;
        private ItemReceiver _itemReceiver;
        private APSession _apSession;
        private MainMenuAPHook _mainMenuAPHook;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("APNest Client loaded.");

            _connectUI = new ConnectUI();
            _apSession = _connectUI.GetApSession();

            _itemReceiver = new ItemReceiver();

            _mainMenuAPHook = new MainMenuAPHook(_connectUI);

            MissionCompleteChecks.LocationCompleted += name => _apSession.SendLocationChecks(new []{name});
            MedalAchievedChecks.LocationCompleted += name => _apSession.SendLocationChecks(new []{name});
        }

        public override void OnUpdate()
        {
            _apSession.ProcessPendingItems();
            _itemReceiver.RegisterMissionChangedEventHook();
            _itemReceiver.ProcessPendingMissionLoad();
            _mainMenuAPHook.RegisterMainMenuHooks();
        }
    }
}
