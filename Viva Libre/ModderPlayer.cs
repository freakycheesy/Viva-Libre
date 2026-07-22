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

namespace Viva_Libre
{
    public partial class ModderPlayer
    {
        private bool _modMenuEnabled = false;
        public bool modMenuEnabled { get => _modMenuEnabled; set
            {
                _modMenuEnabled = value;
                currentPage = defaultPage;
                if (modMenuEnabled) inputManager.DisableGameplayInput(this);
                else inputManager.EnableGameplayInput(this);
            } 
        }
        public Page defaultPage;
        public Page currentPage;
        public PlayerController controller;
        public PlayerCharacter character => controller.GetPlayerCharacter();
        public PlayerControllerInputManager inputManager => controller.GetPlayerControllerInputManager();
        public Rewired.Player rewiredPlayer => inputManager.GetRewiredPlayer();
        public void Start()
        {
            modMenuEnabled = false;
            ModsSetup();
        }
        public void Stop()
        {
            inputManager.EnableGameplayInput(this);
        }

        public void Update()
        {
            if (rewiredPlayer.GetButtonDown("Horn"))
            {   
                if (currentPage.previousPage != null)
                {
                    currentPage = currentPage.previousPage;
                    MelonLogger.Msg($"Reverting Page");
                }
                else
                {
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
            if (rewiredPlayer.GetButtonDown("Ragdoll"))
            {
                currentPage.elements[0].Execute();
            }
            if (rewiredPlayer.GetButtonDown("ActionEnterExitInteract"))
            {
                currentPage.elements[1].Execute();
            }
            if (rewiredPlayer.GetButtonDown("VehicleBoost"))
            {
                currentPage.elements[2].Execute();
            }
            if (rewiredPlayer.GetButtonDown("Jump"))
            {
                currentPage.elements[3].Execute();
            }
        }

        public void OnGUI()
        {
            if (!modMenuEnabled) return;
            GUILayout.Box($"Mod Menu for {controller.GetPlayerName()}");
            GUILayout.Button($"Current Page: {currentPage.name}");
            if (GUILayout.Button($"Square: {currentPage.elements[0].name}"))
            {
                currentPage.elements[0].Execute();
            }
            if (GUILayout.Button($"Triangle: {currentPage.elements[1].name}"))
            {
                currentPage.elements[1].Execute();
            }
            if (GUILayout.Button($"Circle: {currentPage.elements[2].name}"))
            {
                currentPage.elements[2].Execute();
            }
            if (GUILayout.Button($"Cross: {currentPage.elements[3].name}"))
            {
                currentPage.elements[3].Execute();
            }
            if (GUILayout.Button("Dump Rewired Actions"))
            {
                File.WriteAllText(Path.Combine(Application.dataPath, "inputdump.txt"), JsonConvert.SerializeObject(ReInput.mapping));
                GUILayout.Box($"Dumped to {Path.Combine(Application.dataPath, "inputdump.txt")}");
            }
        }
    }
}
