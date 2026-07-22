using MelonLoader;
using Newtonsoft.Json;
using Rewired;
using UnityEngine;

[assembly: MelonInfo(typeof(Viva_Libre.Core), "Viva Libre", "1.0.0", "cheesy", null)]
[assembly: MelonGame("RubberBandGames", "Wobbly Life")]

namespace Viva_Libre
{
    public class Core : MelonMod
    {
        public static Dictionary<PlayerController, ModderPlayer> players = new();
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
            GameInstance.onAssignedPlayerController += GameInstance_onAssignedPlayerController;
            GameInstance.onUnassignedPlayerController += GameInstance_onUnassignedPlayerController;
        }

        private void GameInstance_onUnassignedPlayerController(PlayerController playerController)
        {
            if (players.ContainsKey(playerController))
            {
                players[playerController].Stop();
                players.Remove(playerController);
            }
        }

        private void GameInstance_onAssignedPlayerController(PlayerController playerController)
        {
            if (!players.ContainsKey(playerController))
            {
                ModderPlayer player = new()
                {
                    controller = playerController,
                };
                player.Start();
                players.Add(playerController, player);
            }
        }

        public override void OnUpdate()
        {
            foreach (var item in players)
            {
                item.Value.Update();
            }
        }

        public override void OnGUI()
        {
            foreach (var item in players)
            {
                item.Value.OnGUI();
            }
        }
    }
}