using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace SuperHearseAI
{
    public static class Patcher
    {
        private const string HarmonyId = "gawrbage.SuperHearseAI";
        private static bool patched = false;

        public static void PatchAll()
        {
            if (patched) return;

            Debug.Log("Super Hearse AI is active");

            patched = true;

            // Apply your patches here!
            // Harmony.DEBUG = true;
            var harmony = new Harmony(HarmonyId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void UnpatchAll()
        {
            if (!patched) return;

            var harmony = new Harmony(HarmonyId);
            harmony.UnpatchAll(HarmonyId);

            patched = false;

            Debug.Log("Super Hearse AI is disabled");
        }
    }

}
