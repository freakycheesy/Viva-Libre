using MelonLoader;
using MelonLoader.Utils;
using System.Reflection;
using UnityEngine;

[assembly: MelonInfo(typeof(Viva_Libre.Core), "Viva Libre", "1.0.0", "cheesy", "https://github.com/freakycheesy/Viva-Libre.git")]
[assembly: MelonGame("RubberBandGames", "Wobbly Life")]

namespace Viva_Libre
{
    public enum UIType
    {
        Custom,
        Unity
    }
    public class Core : MelonMod
    {
        public const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        public static Core Instance { get; private set; }
        public static Dictionary<GameplayCamera, ModderPlayer> freeCamPlayers = new();
        public static Dictionary<GameplayCamera, ModderPlayer> firstPersonPlayers = new();
        public static Dictionary<PlayerController, ModderPlayer> players = new();
        public static List<CustomItemPack> CustomItemPacks { get; private set; } = new();

        public static UIType UIType { get; private set; }
        public static string link => "https://github.com/freakycheesy/Viva-Libre.git";
        public static void StartCoroutine(System.Collections.IEnumerator routine)
        {
            MelonCoroutines.Start(routine);
        }
        public override void OnInitializeMelon()
        {
            Instance = this;
            LoggerInstance.Msg("Initialized.");
            //HarmonyInstance.PatchAll();
            StartCoroutine(InitCustomItems());
            LoadVivaAssetBundle();
            GameInstance.onAssignedPlayerController += GameInstance_onAssignedPlayerController;
            GameInstance.onUnassignedPlayerController += GameInstance_onUnassignedPlayerController;
        }
        public static System.Collections.IEnumerator InitCustomItems()
        {
            var path = Path.Combine(MelonEnvironment.UserLibsDirectory, "CustomItems");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                yield break;
            }

            var itemDirectories = Directory.GetDirectories(path);

            foreach (var itemDirectory in itemDirectories)
            {
                try
                {
                    CustomItemPacks.Add(new(itemDirectory));
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error loading Custom Item Pack at \"{itemDirectory}\": {ex}");
                }
            }
        }
        public static GUISkin CustomSkin;
        public static Texture2D CustomLogo;
        private void LoadVivaAssetBundle()
        {
            string path = Path.Combine(MelonEnvironment.UserLibsDirectory, "cheesy", "viva");
            bool exists = System.IO.File.Exists(path);
            if (!exists)
            {
                UIType = UIType.Unity;
                LoggerInstance.Error($"could not find {path}, falling back to unity imgui");
            }
            else
            {
                try
                {
                    var bundle = AssetBundle.LoadFromFile(path);
                    if (bundle != null) {
                        CustomSkin = bundle.LoadAsset<GUISkin>("Viva");
                        CustomLogo = bundle.LoadAsset<Texture2D>("Logo");
                        if(CustomLogo && CustomSkin)
                        {
                            UIType = UIType.Custom;
                            LoggerInstance.MsgPastel("Skin and Logo Success! Using Custom Skin");
                        }
                        else
                        {
                            UIType = UIType.Unity;
                            LoggerInstance.Error("custom logo and custom skin not present, falling back to unity imgui");
                        }
                    }
                    else
                    {
                        UIType = UIType.Unity;
                        LoggerInstance.Error("bundle does not exist?, falling back to unity imgui");
                    }
                }
                catch (Exception e)
                {
                    UIType = UIType.Unity;
                    LoggerInstance.Msg($"ERROR: [{e}] falling back to unity imgui");
                }
            }
        }

        private void GameInstance_onUnassignedPlayerController(PlayerController playerController)
        {
            if (!playerController.IsLocal()) return;
            if (players.ContainsKey(playerController))
            {
                players[playerController].Stop();
                players[playerController].Dispose();
                players.Remove(playerController);
            }
        }

        private void GameInstance_onAssignedPlayerController(PlayerController playerController)
        {
            if (!playerController.IsLocal()) return;
            if (!players.ContainsKey(playerController))
            {
                ModderPlayer player = new()
                {
                    myController = playerController,
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
        public override void OnLateUpdate()
        {
            foreach (var item in players)
            {
                item.Value.LateUpdate();
            }
        }

        public override void OnGUI()
        {
            GUILayout.BeginHorizontal();
            foreach (var item in players)
            {
                GUILayout.BeginVertical();
                switch (UIType)
                {
                    case UIType.Custom:
                        item.Value.OnCustomGUI();
                        break;
                    case UIType.Unity:
                        item.Value.OnUnityGUI();
                        break;
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
    }
}