using MelonLoader;
using Newtonsoft.Json;
using Rewired;
using UnityEngine;

namespace Viva_Libre
{
    public partial class ModderPlayer : IDisposable
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
        private Page _currentPage;
        public Page currentPage { get => _currentPage;
            set {
                _currentPage = value;
            }
        }
        public  int selectedElement = 0;
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
        public void LateUpdate()
        {

        }
        public void Update()
        {
            if (myRewiredPlayer.GetButtonDown("Horn"))
            {
                controller = myController;
                modMenuEnabled = !modMenuEnabled;
                MelonLogger.Msg($"ModMenu {modMenuEnabled}");
            }
            if (modMenuEnabled)
            {
                ModMenuUpdate();
            }
        }
        bool up => myRewiredPlayer.GetNegativeButtonDown("UIVertical");
        bool down => myRewiredPlayer.GetButtonDown("UIVertical");
        private void ModMenuUpdate()
        {
            if (up)
            {
                selectedElement++;
                if (selectedElement >= currentPage.elements.Length) selectedElement = 0;
            }
            else if (down)
            {
                selectedElement--;
                if (selectedElement < 0) selectedElement = currentPage.elements.Length - 1;
            }
            if (myRewiredPlayer.GetButtonDown("UISubmit"))
            {
                currentPage?.elements[selectedElement]?.Execute();
            }
            if (myRewiredPlayer.GetButtonDown("UICancel"))
            {
                if(currentPage.previousPage != null) currentPage = currentPage.previousPage;
                else
                {
                    controller = myController;
                    modMenuEnabled = false;
                }
            }
        }

        public void OnGUI()
        {
            if (!modMenuEnabled) return;
            if(currentPage == null)
            {
                GUILayout.Box("FATAL ERROR: NO PAGE FOUND");
                return;
            }
            try
            {
                GUILayout.Box($"Mod Menu for {myController.GetPlayerName()}");
                GUILayout.Box($"Selected Player: {controller.GetPlayerName()}");
                GUILayout.Button($"Current Page: {currentPage.name}");
                for (int i = 0; i < currentPage.elements.Length; i++)
                {
                    if (currentPage.elements[i] == null) continue;
                    var element = currentPage.elements[i];
                    var elementName = element.name;
                    if (selectedElement == i) elementName = $">>{elementName}<<";
                    if (GUILayout.Button(elementName))
                    {
                        element?.Execute();
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
            /*
            if (GUILayout.Button("Dump Network Prefabs"))
            {
                var field = typeof(HawkNetworkManager).GetField("registeredNetworkBehavioursDic", System.Reflection.BindingFlags.NonPublic);
                Dictionary<Guid, AssetReference> dic = field.GetValue(HawkNetworkManager.DefaultInstance);
                File.WriteAllText(Path.Combine(Application.dataPath, "registeredprefabs.txt"), JsonConvert.SerializeObject(dic));
                GUILayout.Box($"Dumped to {Path.Combine(Application.dataPath, "registeredprefabs.txt")}");
            }
            */
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
