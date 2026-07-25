using MelonLoader;
using UnityEngine;

namespace Viva_Libre
{
    public partial class ModderPlayer
    {
        // Player Mods
        Page speedMods;
        Page movementMods;
        Page playerMods;
        // Server Mods
        Page timeMods;
        Page weatherMods;
        Page serverMods;
        // Gameplay Mods
        Page gameplayMods;
        // Client Mods
        Page rewardsUnlocker; 
        Page rewardsLocker;

        Page unlockableManager;
        Page saveFileMods;
        Page clientMods;
        // Prop Spawner
        Page propSpawner;
        // Default Page
        private void ModsSetup()
        {
            // Player Mods
            speedMods = new("Speed Mods", this, [new Function("Increase Speed", IncreaseSpeed), new Function("Decrease Speed", DecreaseSpeed), new Function("Increase Jump", IncreaseJump), new Function("Decrease Jump", DecreaseJump)]);
            movementMods = new("Movement Mods", this, [new Function("No Clip", NoClip), new Function("Reset", ResetMovement), speedMods]);
            playerMods = new("Player Mods", this, [movementMods, new Function("Respawn", Respawn), new Function("Smite Player", SmitePlayer), new Function("Teleport All Character", TeleportAllCharacters)]);
            // Server Mods
            timeMods = new("Time Mods", this, [new Function("Morning", SetMorning), new Function("Midday", SetMidday), new Function("Evening", SetEvening), new Function("Midnight", SetMidnight)]);
            weatherMods = new("Weather Mods", this, [timeMods]);
            serverMods = new("Server Mods", this, [new Function("Toggle Low Gravity", LowGravity), new Function("Ragdoll All Players", RagdollAllPlayers), new Function("Respawn All Players", RespawnAllPlayers), weatherMods]);
            // Gameplay Mods
            gameplayMods = new("Gameplay Mods", this, [playerMods, serverMods, new Function("null", null), new Function("null", null)]);
            // Client Mods
            rewardsUnlocker = new("Rewards Unlocker", this, [new Function("Unlock All Vehicles", UnlockAllVehicles), new Function("Unlock All Outfits", UnlockAllOutfits), new Function("Unlock All Achievements", UnlockAllAchievements)]);
            rewardsLocker = new("Rewards Locker", this, [new Function("Lock All Vehicles", LockAllVehicles), new Function("Lock All Outfits", LockAllOutfits), new Function("Lock All Achievements", LockAllAchievements)]);
            unlockableManager = new("Unlockable Manager", this, [rewardsUnlocker, rewardsLocker]);
            saveFileMods = new("Save File Mods", this, [unlockableManager]);
            clientMods = new("Client Mods", this, [saveFileMods, new Function("Toggle First Person", ToggleFirstPerson)]);
            // Prop Spawner
            propSpawner = new("Prop Spawner", this, [new Function("Toggle", ToggleSandbox)]);
            // Default Page
            defaultPage = new("Main", this, [gameplayMods, clientMods, propSpawner, new Function("Swap Victim", NextPlayer)]);
            currentPage = defaultPage;
        }

        FirstPerson firstPerson;
        private void ToggleFirstPerson()
        {
            firstPerson.firstPersonEnabled = !firstPerson.firstPersonEnabled;
        }

        private void ToggleSandbox()
        {
            myController.GetModPlayerController().ServerSetSandboxUIEnabled(true);
            ModdablePlayerController.FindObjectOfType<ModdablePlayerController>().ServerSetSandboxUIEnabled(true);
        }

        private void DecreaseJump()
        {
            var move = character.GetPlayerCharacterMovement();
            move.SetJumpMultiplier(move.GetJumpMultiplier() - 1);
        }

        private void IncreaseJump()
        {
            var move = character.GetPlayerCharacterMovement();
            move.SetJumpMultiplier(move.GetJumpMultiplier() + 1);
        }

        private void DecreaseSpeed()
        {
            var move = character.GetPlayerCharacterMovement();
            move.SetSpeedMultiplier(move.GetSpeedMultiplier() - 1);
        }

        private void IncreaseSpeed()
        {
            var move = character.GetPlayerCharacterMovement();
            move.SetSpeedMultiplier(move.GetSpeedMultiplier()+1);
        }

        private void ResetMovement()
        {
            character.GetPlayerCharacterMovement().SetNoClipEnabled(false);
            character.GetPlayerCharacterMovement().SetSpeedMultiplier(1);
            character.GetPlayerCharacterMovement().SetJumpMultiplier(1);
        }

        private void UnlockAllAchievements()
        {
            var achievements = (WobblyAchievement[])Enum.GetValues(typeof(WobblyAchievement));

            foreach (var item in achievements)
            {
                AchievementManager.Instance.UnlockAchievement(item, myController);
            }
        }
        private void LockAllAchievements()
        {
            var achievements = (WobblyAchievement[])Enum.GetValues(typeof(WobblyAchievement));

            foreach (var item in achievements)
            {
                AchievementManager.Instance.LockAchievement(item, myController);
            }
        }
        private void LockAllVehicles()
        {
            myController.GetPlayerPersistentData().VehiclesData.Vehicles.Clear();
            //VehicleManager.Instance.GetVehicles().ForEach(x => myController.GetPlayerPersistentData().VehiclesData.LockVehicle(new() { guidStr = x.GetAssetIdRaw(), vehicleGuid = x.GetAssetId() }));
        }

        private void UnlockAllVehicles()
        {
            VehicleManager.Instance.GetVehicles().ForEach(myController.GetPlayerPersistentData().VehiclesData.UnlockVehicle);     
        }

        private void UnlockOutfit(ClothingAssetReference reference)
        {
            myController.GetPlayerControllerUnlocker().UnlockClothing(this, reference);
        }
        private void LockOutfit(ClothingAssetReference reference)
        {
            myController.GetPlayerControllerUnlocker().LockClothing(reference);
        }

        private void UnlockAllOutfits()
        {
            var clothes = ClothingManager.Instance.GetAllClothingReferences();
            foreach (var item in clothes)
            {
                UnlockOutfit(item);
            }
        }
        private void LockAllOutfits()
        {
            var clothes = ClothingManager.Instance.GetAllClothingReferences();
            foreach (var item in clothes)
            {
                LockOutfit(item);
            }
        }

        public int selectedPlayer = 0;
        private void NextPlayer()
        {
            PlayerController[] controllers = GameInstance.Instance.GetPlayerControllers().ToArray();

            selectedPlayer++;
            if(selectedPlayer > controllers.Length) selectedPlayer = 0;
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
            GameInstance.Instance.GetPlayerCharacters().ForEach(x => { if (x != controller) x.SetPlayerPosition(character.GetPlayerPosition()); });
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
            MelonLogger.Msg($"Before: {Physics.gravity}");
            Physics.gravity = lowGravity ? Vector3.up * -1f : Vector3.up * -19.6f;
            MelonLogger.Msg($"After: {Physics.gravity}");
        }


        private void ModUpdate()
        {
            if (firstPerson == null)
            {
                firstPerson = new() { player = this };
                firstPerson.OnEnable();
            }
        }
    }
}
