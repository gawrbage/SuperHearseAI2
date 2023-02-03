using HarmonyLib;
using System.Reflection;
using UnityEngine;
using ColossalFramework;

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

    [HarmonyPatch(typeof(HearseAI), "ReleaseVehicle")]
    public static class HearseAIReleaseVehiclePatch
    {
        static void Prefix(ushort vehicleID, ref Vehicle data)
        {
            DeathRegistry.RemoveClaimByVehicleID(vehicleID);
        }
    }

    [HarmonyPatch(typeof(HearseAI), "LoadDeadCitizens")]
    public static class HearseAILoadDeadCitizensPatch
    {
        static void Prefix(ushort vehicleID, ref Vehicle data, ushort buildingID)
        {
            //DeathRegistry.RemoveClaimByVehicleID(vehicleID);
        }
    }

    [HarmonyPatch(typeof(HearseAI), "StartTransfer")]
    public static class HearseAIStartTransferPatch
    {
        static void Prefix(ushort vehicleID, ref Vehicle data, TransferManager.TransferReason material, ref TransferManager.TransferOffer offer)
        {
            if ((data.m_flags & Vehicle.Flags.TransferToTarget) != 0)
            {
                return;
            }

            try
            {
                byte districtID = Singleton<DistrictManager>.instance.GetDistrict(Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_sourceBuilding].m_position);
                DeathClaim dc = DeathRegistry.GetClosestDeathClaim(data.GetLastFramePosition(), vehicleID, districtID);
                if (dc == null) { return; }

                TransferManager.TransferOffer betterOffer = new TransferManager.TransferOffer();

                betterOffer.Active = true;
                betterOffer.Amount = 0;
                betterOffer.Building = dc.buildingID;
                
                offer = betterOffer;
            } catch (System.Exception E)
            {
                return;
            }
        }
    }

    [HarmonyPatch(typeof(CitizenManager), "ReleaseCitizen")]
    public static class CitizenManagerReleaseCitizenPatch
    {
        static void Postfix(uint citizen)
        {
            DeathRegistry.RemoveClaimByCitizenID(citizen);
        }
    }

    [HarmonyPatch(typeof(ResidentAI))]
    public static class ResidentAISimulationStepPatch
    {
        [HarmonyPatch(nameof(ResidentAI.SimulationStep), new[] { typeof(ushort), typeof(Citizen) }, new[] { ArgumentType.Normal, ArgumentType.Ref })]
        static void Postfix(uint citizenID, ref Citizen data)
        {
            if (!data.Dead) return;

            ushort citizenBuilding = 0;
            switch (data.CurrentLocation)
            {
                case Citizen.Location.Home:
                    if (data.m_homeBuilding == 0) return;
                    citizenBuilding = data.m_homeBuilding;
                    break;
                case Citizen.Location.Visit:
                    if (data.m_visitBuilding == 0) return;
                    citizenBuilding = data.m_visitBuilding;
                    break;
                case Citizen.Location.Work:
                    if (data.m_workBuilding == 0) return;
                    citizenBuilding = data.m_workBuilding;
                    break;
                case Citizen.Location.Moving: //TODO: maybe the citizen is not allowed to die if they are moving, maybe this can be removed
                    if (data.m_vehicle == 0) return;
                    break;
                default:
                    break;
            }

            if (DeathRegistry.IsBuildingRegistered(citizenBuilding) == false)
            {
                DeathClaim dc = new DeathClaim();
                dc.location = data.CurrentLocation;
                dc.buildingID = citizenBuilding;
                dc.citizenID = citizenID;
                dc.pos = Singleton<BuildingManager>.instance.m_buildings.m_buffer[citizenBuilding].m_position;

                DeathRegistry.SubmitClaim(dc);
            }
        }
    }
}
