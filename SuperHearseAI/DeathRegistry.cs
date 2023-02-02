using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SuperHearseAI
{
    public static class DeathRegistry
    {
        private static List<DeathClaim> claims = new List<DeathClaim>();

        public static void RemoveClaimByBuildingID(ushort buildingID)
        {
            foreach(DeathClaim claim in claims)
            {
                if(claim.buildingID == buildingID)
                {
                    claims.Remove(claim);
                    return;
                }
            }
        }
        public static void SubmitClaim(DeathClaim dc)
        {
            claims.Add(dc);
        }
        public static DeathClaim GetClosestDeathClaim(Vector3 pos, ushort vehicleID, byte districtID)
        {
            IEnumerable<DeathClaim> results = from claim in claims
                                             where districtID == claim.districtID && claim.vehicleAssigned == false
                                             select claim;

            if (results.Count() == 0) return null;

            DeathClaim closest = results.First();

            foreach(DeathClaim dc in results)
            {
                if(Vector3.Distance(dc.pos, pos) < Vector3.Distance(closest.pos, pos))
                {
                    closest = dc;
                }
            }

            closest.vehicleAssigned = true;
            closest.vehicleID = vehicleID;

            return closest;
        }

        public static bool IsBuildingRegistered(ushort buildingID)
        {
            //This way is dumb and inefficient / TODO: probably remove later
            /*
            IEnumerable<DeathClaim> results = from claim in claims
                          where claim.buildingID == buildingID
                          select claim;

            if (results.Count() > 0) { return true; } else { return false; }*/

            foreach (DeathClaim dc in claims)
            {
                if (dc.buildingID == buildingID) return true;
            }
            return false;
        }
    }
}
