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
            if (myInputManager.IsUsingMouseKeyboard())
            {
                myInputManager.DisableGameplayCameraInput(this);
            }
            else
            {
                myInputManager.DisableUIInput(this);
                myInputManager.DisableInteratorInput(this);
            }
        }
        public void EnableInput()
        {
            if (myInputManager.IsUsingMouseKeyboard())
            {
                myInputManager.EnableGameplayCameraInput(this);
            }
            else
            {
                myInputManager.EnableUIInput(this);
                myInputManager.EnableInteratorInput(this);
            }
        }
        public void Start()
        {
            modMenuEnabled = false;
            ModsSetup();
        }
        public void Stop()
        {
            myInputManager.EnableGameplayInput(this);
            firstPerson.OnDisable();
        }
        public void LateUpdate()
        {
            if(Cursor.lockState == CursorLockMode.Confined) Cursor.lockState = CursorLockMode.None;
        }
        public void Update()
        {
            bool mod = myInputManager.IsUsingMouseKeyboard() ? Input.GetKeyDown(KeyCode.F2) : myRewiredPlayer.GetButtonDown("Horn");
            if (mod)
            {
                controller = myController;
                modMenuEnabled = !modMenuEnabled;
                MelonLogger.Msg($"ModMenu {modMenuEnabled}");
                if (myInputManager.IsUsingMouseKeyboard())
                {
                    if (modMenuEnabled) myInputManager.ShowCursor(this);
                    else myInputManager.HideCursor(this);
                }
            }
            if (modMenuEnabled)
            {
                ModMenuUpdate();
            }
            ModUpdate();
        }
        bool up => myInputManager.IsUsingMouseKeyboard() ? Input.GetKeyDown(KeyCode.UpArrow) : myRewiredPlayer.GetButtonDown("UIVertical");
        bool down => myInputManager.IsUsingMouseKeyboard() ? Input.GetKeyDown(KeyCode.DownArrow) : myRewiredPlayer.GetNegativeButtonDown("UIVertical");
        private bool canNavigate => !myInputManager.IsUsingMouseKeyboard();
        private void ModMenuUpdate()
        {
            if (!canNavigate) return;
            if (down)
            {
                selectedElement++;
                if (selectedElement >= currentPage.elements.Length) selectedElement = 0;
            }
            else if (up)
            {
                selectedElement--;
                if (selectedElement < 0) selectedElement = currentPage.elements.Length - 1;
            }
            if (myInputManager.IsUsingMouseKeyboard() ? Input.GetKeyDown(KeyCode.Return) : myRewiredPlayer.GetButtonDown("UISubmit"))
            {
                currentPage?.elements[selectedElement]?.Execute();
            }
            if (myInputManager.IsUsingMouseKeyboard() ? Input.GetKeyDown(KeyCode.Backspace) : myRewiredPlayer.GetButtonDown("UICancel"))
            {
                GoBack();
            }
        }

        private void GoBack()
        {
            if (currentPage.previousPage != null) currentPage = currentPage.previousPage;
            else
            {
                controller = myController;
                modMenuEnabled = false;
            }
        }

        public void OnCustomGUI()
        {
            if (!modMenuEnabled) return;
            GUI.skin = Core.CustomSkin;
            GUILayout.BeginVertical("hover");
            GUILayout.Box(Core.CustomLogo, "logo");
            OnGUI();
        }
        private void OnGUI()
        {
            if (!modMenuEnabled) return;
            GUILayout.Box($"{Core.link}", "hover");
            GUILayout.EndVertical();
            GUILayout.BeginVertical("hover");
            GUILayout.Box($"{myController.GetPlayerName()}'s Menu");
            GUILayout.Box($"Victim: {controller.GetPlayerName()}");
            GUILayout.EndVertical();
            GUILayout.BeginVertical("hover");
            GUILayout.Box(currentPage.name);
            for (int x = 0; x < currentPage.elements.Length; x++)
            {
                var element = currentPage.elements[x];
                var elementName = selectedElement == x ? $">>{element.name}<<" : element.name;

                if(selectedElement == x && canNavigate)
                {
                    if (GUILayout.Button(elementName, "hover"))
                    {
                        element?.Execute();
                    }
                }
                else
                {
                    if (GUILayout.Button(elementName))
                    {
                        element?.Execute();
                    }
                }
                
            }
            if (GUILayout.Button("Go Back"))
            {
                GoBack();
            }
            GUILayout.EndVertical();
        }
        public void OnUnityGUI()
        {
            if (!modMenuEnabled) return;
            GUILayout.BeginVertical("hover");
            GUILayout.Box("Viva Libre\nMod Menu", "logo");
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
