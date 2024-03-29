﻿using ICities;
using CitiesHarmony.API;
using HarmonyLib;

namespace SuperHearseAI
{
    public class SuperHearseAI : IUserMod
    {
        public string Name
        {
            get { return "Super Hearse AI: Redefining DeathCare"; }
        }

        public string Description
        {
            get { return "Super. Hearses."; }
        }

        public void OnEnabled()
        {
            HarmonyHelper.DoOnHarmonyReady(() => Patcher.PatchAll());
        }

        public void OnDisabled()
        {
            if (HarmonyHelper.IsHarmonyInstalled) Patcher.UnpatchAll();
        }
    }
}
