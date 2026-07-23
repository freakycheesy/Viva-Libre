using HawkNetworking;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Viva_Libre
{
    public partial class ModderPlayer
    {
        private bool _modMenuEnabled = false;
        public bool modMenuEnabled
        {
            get => _modMenuEnabled; set
            {
                _modMenuEnabled = value;
                currentPage = defaultPage;
                if (modMenuEnabled) DisableInput();
                else EnableInput();
            }
        }
        public Page defaultPage;
        public Page currentPage { get; set; }
        public PlayerController myController;
        public PlayerController controller;
        public PlayerCharacter character => controller.GetPlayerCharacter();
        public PlayerControllerInputManager myInputManager => myController.GetPlayerControllerInputManager();
        public Rewired.Player myRewiredPlayer => myInputManager.GetRewiredPlayer();
        public void DisableInput()
        {
            myInputManager.DisableGameplayInput(this, false);
        }
        public void EnableInput()
        {
            myInputManager.EnableGameplayInput(this);
        }
        public void Start()
        {
            modMenuEnabled = false;
            ModsSetup();
        }
        public void Stop()
        {
            myInputManager.EnableGameplayInput(this);
        }

        public void Update()
        {
            if (myRewiredPlayer.GetButtonDown("Horn"))
            {
                if (currentPage.previousPage != null)
                {
                    currentPage = currentPage.previousPage;
                    MelonLogger.Msg($"Reverting Page");
                }
                else
                {
                    controller = myController;
                    modMenuEnabled = !modMenuEnabled;
                    MelonLogger.Msg($"ModMenu {modMenuEnabled}");
                }
            }
            if (modMenuEnabled)
            {
                ModMenuUpdate();
            }
        }

        private void ModMenuUpdate()
        {
            if (myRewiredPlayer.GetButtonDown("Ragdoll"))
            {
                currentPage?.elements[0]?.Execute();
            }
            if (myRewiredPlayer.GetButtonDown("ActionEnterExitInteract"))
            {
                currentPage?.elements[1]?.Execute();
            }
            if (myRewiredPlayer.GetButtonDown("VehicleBoost"))
            {
                currentPage?.elements[2]?.Execute();
            }
            if (myRewiredPlayer.GetButtonDown("Jump"))
            {
                currentPage?.elements[3]?.Execute();
            }
        }

        public void OnGUI()
        {
            if (!modMenuEnabled) return;
            try
            {
                GUILayout.Box($"Mod Menu for {myController.GetPlayerName()}");
                GUILayout.Box($"Selected Player: {controller.GetPlayerName()}");
                GUILayout.Button($"Current Page: {currentPage.name}");
                if (currentPage.elements.Length > 0)
                {
                    if (GUILayout.Button($"Square: {currentPage.elements[0].name}"))
                    {
                        currentPage?.elements[0]?.Execute();
                    }
                }
                if (currentPage.elements.Length > 1)
                {
                    if (GUILayout.Button($"Triangle: {currentPage.elements[1].name}"))
                    {
                        currentPage?.elements[1]?.Execute();
                    }
                }
                if (currentPage.elements.Length > 2)
                {
                    if (GUILayout.Button($"Circle: {currentPage.elements[2].name}"))
                    {
                        currentPage?.elements[2]?.Execute();
                    }
                }
                if (currentPage.elements.Length > 3) { 
                    if (GUILayout.Button($"Cross: {currentPage.elements[3].name}"))
                    {
                        currentPage?.elements[3]?.Execute();
                    }
                }
            }
            catch
            {
                // Prob missing elements
            }
            if (GUILayout.Button("Dump Rewired Actions"))
            {
                File.WriteAllText(Path.Combine(Application.dataPath, "inputdump.txt"), JsonConvert.SerializeObject(ReInput.mapping));
                GUILayout.Box($"Dumped to {Path.Combine(Application.dataPath, "inputdump.txt")}");
            }
            if (GUILayout.Button("Dump Network Prefabs"))
            {
                var field = typeof(HawkNetworkManager).GetField("registeredNetworkBehavioursDic", System.Reflection.BindingFlags.NonPublic);
                Dictionary<Guid, AssetReference> dic = field.GetValue(HawkNetworkManager.DefaultInstance);
                File.WriteAllText(Path.Combine(Application.dataPath, "registeredprefabs.txt"), JsonConvert.SerializeObject(dic));
                GUILayout.Box($"Dumped to {Path.Combine(Application.dataPath, "registeredprefabs.txt")}");
            }
        }
    }
}
