using UnityEngine;

namespace Viva_Libre
{
    public partial class ModderPlayer
    {
        private void ModsSetup()
        {
            Page movementMods = new("Movement Mods", this, [new Function("No Clip", NoClip)]);
            Page playerMods = new("Player Mods", this, [movementMods, new Function("Respawn", Respawn), new Function("Smite Player", SmitePlayer), new Function("Teleport All Character", TeleportAllCharacters)]);
            Page weatherMods = new("Weather Mods", this, []);
            Page serverMods = new("Server Mods", this, [new Function("Low Gravity", LowGravity), new Function("Ragdoll All Players", RagdollAllPlayers), new Function("Respawn All Players", RespawnAllPlayers), weatherMods]);
            Page saveFileMods = new("Save File Mods", this, []);
            defaultPage = new("Main", this, [playerMods, serverMods, saveFileMods, new Function("null", null)]);
            currentPage = defaultPage;
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
            modMenuEnabled = false;
        }
        private void SmitePlayer()
        {
            var data = WeatherSystem.Instance.GetCurrentWeatherData();
            var index = WeatherSystem.Instance.GetAllWeatherData().ToList().IndexOf(data);

            WeatherSystem.Instance.ServerSetWeatherByIndex(4);
            WeatherSystem.Instance.ServerLightingStrike(controller.GetPlayerCharacter().GetPlayerPosition());
            WeatherSystem.Instance.ServerSetWeatherByIndex(index);
            modMenuEnabled = false;
        }

        private void Respawn()
        {
            controller.ClientRequestRespawn();
            modMenuEnabled = false;
        }

        private bool lowGravity = false;
        private void LowGravity()
        {
            lowGravity = !lowGravity;
            Physics.gravity = lowGravity?Vector3.up * -1f:Vector3.up * -10;
            modMenuEnabled = false;
        }

    }
}
