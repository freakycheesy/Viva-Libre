using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace Viva_Libre
{
    [HarmonyPatch]
    public static class RealisticCarCrashes
    {
        [HarmonyPatch(typeof(PlayerVehicleRoadDestructable), "OnUpdatedDestructionStage")]
        [HarmonyPrefix]
        public static void OnUpdatedDestructionStage(ref PlayerVehicleRoadDestructable __instance, RoadDestructableStage stage)
        {
            if (stage != RoadDestructableStage.Fine && ModderPlayer.realisticCarCrashes)
            {
                Core.StartCoroutine(enumerator(__instance));
            }
        }

        private static IEnumerator enumerator(PlayerVehicleRoadDestructable __instance)
        {
            var r = new QuickReflection<PlayerVehicleRoadDestructable>(__instance, Core.Flags);
            var movement = (PlayerVehicleRoadMovement)r.GetField("roadMovement");
            var pos = __instance.transform.position;

            __instance.GetComponent<IOnVehicleDestroy>().OnVehicleDestroyed(movement.GetPlayerVehicleRoad());

            foreach (var destroy in __instance.GetComponentsInChildren<IOnVehicleDestroy>())
            {
                destroy.OnVehicleDestroyed(movement.GetPlayerVehicleRoad());
            }

            yield return null;

            movement.GetPlayerVehicleRoad().IterateControllersInVehicle((pc) =>
            {
                pc.GetPlayerControllerInteractor().ForceRequestExit();
                pc.GetPlayerCharacter().GetRagdollController().Knockout();
                pc.GetPlayerCharacter().GetHipRigidbody().AddExplosionForce(1500f, pos, 500f, 5f, ForceMode.Impulse);
            });
        }
    }
}
