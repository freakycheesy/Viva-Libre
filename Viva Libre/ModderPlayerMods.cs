using ModWobblyLife;
using UnityEngine;

namespace Viva_Libre
{
    public partial class ModderPlayer
    {
        private void ModsSetup()
        {
            // Player Mods
            Page movementMods = new("Movement Mods", this, [new Function("No Clip", NoClip)]);
            Page playerMods = new("Player Mods", this, [movementMods, new Function("Respawn", Respawn), new Function("Smite Player", SmitePlayer), new Function("Teleport All Character", TeleportAllCharacters)]);
            // Server Mods
            Page timeMods = new("Time Mods", this, [new Function("Morning", SetMorning), new Function("Midday", SetMidday), new Function("Evening", SetEvening), new Function("Midnight", SetMidnight)]);
            Page weatherMods = new("Weather Mods", this, [timeMods]);
            Page serverMods = new("Server Mods", this, [new Function("Low Gravity", LowGravity), new Function("Ragdoll All Players", RagdollAllPlayers), new Function("Respawn All Players", RespawnAllPlayers), weatherMods]);
            // Gameplay Mods
            Page gameplayMods = new("Gameplay Mods", this, [playerMods, serverMods, new Function("null", null), new Function("null", null)]);
           // Client Mods
            Page saveFileMods = new("Save File Mods", this, []);
            Page clientMods = new("Client Mods", this, [saveFileMods]);
            // Prop Spawner
            Page propSpawner = new("Prop Spawner", this, null);
            // Default Page
            defaultPage = new("Main", this, [gameplayMods, clientMods, propSpawner, new Function("Switch Player", NextPlayer)]);
            currentPage = defaultPage;
        }

        public int selectedPlayer = 0;
        private void NextPlayer()
        {
            PlayerController[] controllers = GameInstance.Instance.GetPlayerControllers().ToArray();
            selectedPlayer++;
            if (selectedPlayer > controllers.Length) selectedPlayer = 0;
            controller = controllers[selectedPlayer];
        }
        private void SetEvening()
        {
            DayNightCycle.Instance.SetEvening();
        }
        private void SetMidnight()
        {
            DayNightCycle.Instance.SetMidnight();
        }
        private void SetMidday()
        {
            DayNightCycle.Instance.SetMidday();
        }

        private void SetMorning()
        {
            DayNightCycle.Instance.SetMorning();
        }

        private void RespawnAllPlayers()
        {
            GameInstance.Instance.GetPlayerCharacters().ForEach(x => x.Kill(0));
        }

        private void RagdollAllPlayers()
        {
            GameInstance.Instance.GetPlayerCharacters().ForEach(x => x.GetRagdollController().Ragdoll());
        }

        private void TeleportAllCharacters()
        {
            GameInstance.Instance.GetPlayerCharacters().ForEach(x=>x.SetPlayerPosition(character.GetPlayerPosition()));
        }

        private void NoClip()
        {
            bool noclip = character.GetPlayerCharacterMovement().IsNoClipEnabled();
            character.GetPlayerCharacterMovement().SetNoClipEnabled(!noclip);
        }
        private void SmitePlayer()
        {
            var data = WeatherSystem.Instance.GetCurrentWeatherData();
            var index = WeatherSystem.Instance.GetAllWeatherData().ToList().IndexOf(data);

            WeatherSystem.Instance.ServerSetWeatherByIndex(4);
            WeatherSystem.Instance.ServerLightingStrike(character.GetPlayerPosition());
            WeatherSystem.Instance.ServerSetWeatherByIndex(index);
        }

        private void Respawn()
        {
            controller.ClientRequestRespawn();
        }

        private bool lowGravity = false;
        private void LowGravity()
        {
            lowGravity = !lowGravity;
            Physics.gravity = lowGravity?Vector3.up * -1f:Vector3.up * -10;
        }

    }
}
