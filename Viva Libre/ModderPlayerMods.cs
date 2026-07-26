using CustomItems;
using MelonLoader;
using UnityEngine;

namespace Viva_Libre
{
    public partial class ModderPlayer
    {
        // Player Mods
        Page speedMods;
        Page movementMods;
        Page characterManager;
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
        Page moneyManager;
        Page moneyBagManager;
        Page clientMods;
        // Prop Spawner
        Page propSpawner;
        // Extra Mods
        Page extraMods;

        // Default Page
        private void ModsSetup()
        {
            // Player Mods
            speedMods = new("Speed Mods", this, [new Function("Increase Speed", IncreaseSpeed), new Function("Decrease Speed", DecreaseSpeed), new Function("Increase Jump", IncreaseJump), new Function("Decrease Jump", DecreaseJump)]);
            movementMods = new("Movement Mods", this, [new Function("No Clip", NoClip), new Function("Reset", ResetMovement), speedMods]);
            characterManager = new("Character Manager", this, [new Function("Respawn", Respawn), new Function("Toggle Invinciblility", ToggleInvincible), new Function("Ragdoll", Ragdoll), new Function("UnRagdoll", UnRagdoll), new Function("Knockout", Knockout)]);
            playerMods = new("Player Mods", this, [movementMods, characterManager, new Function("Smite Player", SmitePlayer), new Function("Teleport All Character", TeleportAllCharacters)]);
            // Server Mods
            timeMods = new("Time Mods", this, [new Function("Morning", SetMorning), new Function("Midday", SetMidday), new Function("Evening", SetEvening), new Function("Midnight", SetMidnight)]);
            weatherMods = new("Weather Mods", this, [timeMods]);
            serverMods = new("Server Mods", this, [new Function("Toggle Low Gravity", LowGravity), new Function("Ragdoll All Players", RagdollAllPlayers), new Function("Respawn All Players", RespawnAllPlayers), weatherMods]);
            // Gameplay Mods
            Page slowMo = new("Slow-Mo", this, [new Function("1x speed", () => Time.timeScale = 1), new Function("2x speed", ()=> Time.timeScale = 2), new Function("1/2x speed", ()=>Time.timeScale = 0.5f), new Function("1/4x speed", ()=>Time.timeScale = 0.25f), new Function("1/5x speed", () => Time.timeScale = 0.2f), new Function("1/10x speed", () => Time.timeScale = 0.1f), new Function("1/100x speed", () => Time.timeScale = 0.01f),]);
            gameplayMods = new("Gameplay Mods", this, [playerMods, serverMods, slowMo]);
            // Client Mods
            rewardsUnlocker = new("Rewards Unlocker", this, [new Function("Unlock All Vehicles", UnlockAllVehicles), new Function("Unlock All Outfits", UnlockAllOutfits), new Function("Unlock All Achievements", UnlockAllAchievements), new Function("Unlock All Presents", UnlockAllPresents)]);
            rewardsLocker = new("Rewards Locker", this, [new Function("Lock All Vehicles", LockAllVehicles), new Function("Lock All Outfits", LockAllOutfits), new Function("Lock All Achievements", LockAllAchievements), new Function("Lock All Presents", LockAllPresents)]);
            unlockableManager = new("Unlockable Manager", this, [rewardsUnlocker, rewardsLocker]);
            moneyManager = new("Money Manager", this, [new Function("Give Money $25", () => GiveMoney(25)), new Function("Give Money $50", ()=>GiveMoney(50)), new Function("Give Money $100", ()=>GiveMoney(100)), new Function("$Give Money $1000", ()=>GiveMoney(1000)), new Function("$Give Money $500", () => GiveMoney(500)), new Function("$Give Money $10000", () => GiveMoney(10000))]);
            saveFileMods = new("Save File Mods", this, [unlockableManager, moneyManager]);
            clientMods = new("Client Mods", this, [saveFileMods, new Function("Toggle First Person Mode", ToggleFirstPerson), new Function("Toggle Free Cam Limits", ToggleFreeCamBorder)]);
            // Prop Spawner
            moneyBagManager = new("Money Bag Spawner", this, [new Function("Spawn Money Bag $25", () => SpawnMoney(25)), new Function("Spawn Money Bag $50", () => SpawnMoney(50)), new Function("Spawn Money Bag $100", () => SpawnMoney(100)), new Function("$Spawn Money Bag $1000", () => SpawnMoney(1000)), new Function("$Spawn Money Bag $500", () => SpawnMoney(500)), new Function("$Spawn Money Bag $10000", () => SpawnMoney(10000))]);
            List<Element> customItemElements = new();
            foreach(var pack in Core.CustomItemPacks)
            {
                Page packPage = new(pack.packName, this, null);
                List<Element> packElements = new();
                foreach(var item in pack.items)
                {
                    packElements.Add(new Function(item.itemName, () => SpawnItem(item)) {
                        onPreGUI = () => { GUILayout.BeginHorizontal(); },
                        onPostGUI = () => { GUILayout.Box(item.itemSprite.texture);  GUILayout.EndHorizontal(); }
                    });
                }
                packPage.elements = packElements.ToArray();
                customItemElements.Add(packPage);
            }
            Page customItems = new("Custom Items", this, customItemElements.ToArray());
            propSpawner = new("Prop Spawner", this, [moneyBagManager, customItems]);
            // Extra Mods
            extraMods = new("Extra Mods", this, [new Function("Realistic Car Crashes", ToggleRealisticCarCrashes)]);
            // Default Page
            defaultPage = new("Main", this, [gameplayMods, clientMods, propSpawner, extraMods, new Function("Swap Victim", NextPlayer)]);
            currentPage = defaultPage;
        }

        private void SpawnItem(CustomItem item)
        {
            if (character != null && item != null)
            {
                var pos = character.GetPlayerPosition() + character.GetPlayerForward();

                NetworkPrefab.SpawnNetworkPrefab(item.gameObject, pos);
            }
        }

        private void UnlockAllPresents()
        {
            foreach (var present in PresentManager.Instance.GetAllPresentGuids())
            {
                myController.GetPlayerPersistentData().MiscData.UnlockPresent(Guid.Parse(present));
            }
            myController.GetPlayerControllerUnlocker().ShowCounter(PromptCounterType.Present);
            myController.GetPlayerControllerUnlocker().SendMessage("OnPresentUnlockedChanged");
        }
        private void LockAllPresents()
        {
            myController.GetPlayerPersistentData().MiscData.LockAllPresents();
            myController.GetPlayerControllerUnlocker().ShowCounter(PromptCounterType.Present);
            myController.GetPlayerControllerUnlocker().SendMessage("OnPresentUnlockedChanged");
        }
        private void Knockout()
        {
            character.GetRagdollController().Knockout();
        }

        private void UnRagdoll()
        {
            character.GetRagdollController().SendMessage("SetIsActiveRagdoll", true);
        }

        private void Ragdoll()
        {
            character.GetRagdollController().Ragdoll();
        }

        /// <summary>
        /// https://www.youtube.com/watch?v=7s0nIxBLZio
        /// </summary>
        public bool invincible = false;

        public void ToggleInvincible()
        {
            invincible = !invincible;
            if (invincible)
            {
                character.GetRagdollController().SendMessage("SetIsActiveRagdoll", true);
                character.GetRagdollController().LockRagdollState(this);
            }
            else
            {
                character.GetRagdollController().UnlockRagdollState(this);
            }
        }

        public void SpawnMoney(int amount)
        {
            RewardManagerInstance.Instance.ServerReward(controller, RewardType.MoneyBag, amount);
        }
        public void GiveMoney(int amount)
        {
            myController.GetPlayerControllerEmployment().UpdateMoney(amount);
        }
        public void ResetMoney()
        {
            myController.GetPlayerControllerEmployment().UpdateMoney(-myController.GetPlayerControllerEmployment().GetLocalMoney());
        }

        public static bool realisticCarCrashes = false;
        private static void ToggleRealisticCarCrashes()
        {
            realisticCarCrashes = !realisticCarCrashes;
        }

        private void ToggleFreeCamBorder()
        {
            if (Core.freeCamPlayers.ContainsValue(this)) Core.freeCamPlayers.Remove(controller.GetGameplayCamera());
            else Core.freeCamPlayers.Add(controller.GetGameplayCamera(), this);
        }

        private void ToggleFirstPerson()
        {
            if (Core.firstPersonPlayers.ContainsValue(this)) Core.firstPersonPlayers.Remove(controller.GetGameplayCamera());
            else Core.firstPersonPlayers.Add(controller.GetGameplayCamera(), this);
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
            if(selectedPlayer >= controllers.Length) selectedPlayer = 0;
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
        }
    }
}
