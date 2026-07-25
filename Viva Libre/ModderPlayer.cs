using MelonLoader;
using MelonLoader.TinyJSON;
using Newtonsoft.Json;
using Rewired;
using SingularityGroup.HotReload;
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
            bool mod = myController.GetPlayerControllerInputManager().IsUsingMouseKeyboard() ? Input.GetKeyDown(KeyCode.F2) : myRewiredPlayer.GetButtonDown("Horn");
            if (mod)
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
        public void OnCustomGUI()
        {
            if (!modMenuEnabled) return;
            GUI.skin = Core.CustomSkin;
            GUILayout.Box(Core.CustomLogo);
            OnGUI();
        }
        private void OnGUI()
        {
            if (!modMenuEnabled) return;
            GUILayout.Box($"Viva Libre Mod Menu - {Core.link}");
            GUILayout.Box($"Mod Menu for {myController.GetPlayerName()}");
            GUILayout.Box(currentPage.name);
            GUILayout.BeginVertical("hover");
            for (int j = 0; j < currentPage.elements.Length; j++)
            {
                var element = currentPage.elements[j];
                var elementName = element.name;
                if (selectedElement == j)
                {
                    elementName = $">>{elementName}<<";
                    if (GUILayout.Button(elementName, "hover"))
                    {
                        element?.Execute();
                    }
                }
                else if (GUILayout.Button(elementName))
                {
                    element?.Execute();
                }
            }
            GUILayout.EndVertical();
        }
        public void OnUnityGUI()
        {
            if (!modMenuEnabled) return;
            OnGUI();
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
