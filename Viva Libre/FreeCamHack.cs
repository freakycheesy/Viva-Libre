using HarmonyLib;
using UnityEngine;

namespace Viva_Libre
{
    [HarmonyPatch]
    public static class FreeCamHack
    {
        [HarmonyPatch(typeof(CameraFocusFree), "UpdateCamera")]
        [HarmonyPrefix]
        public static void UpdateCameraPrefix(ref CameraFocusFree __instance, GameplayCamera camera)
        {
            var r = new QuickReflection<CameraFocusFree>(__instance, Core.Flags);
            r.SetField("bLockDistanceEnabled", !Core.freeCamPlayers.ContainsKey(camera));
        }
        [HarmonyPatch(typeof(CameraFocusFree), "HandleCollision")]
        [HarmonyPrefix]
        public static bool HandleCollision(GameplayCamera camera, Vector3 targetPosition, out float distance, ref Vector3 targetCamPosition, bool bIgnoreGameObjectLayer = false)
        {
            distance = 0f;
            return !Core.freeCamPlayers.ContainsKey(camera);
        }

    }
}
