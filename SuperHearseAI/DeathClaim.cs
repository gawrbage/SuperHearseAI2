using UnityEngine;
using ColossalFramework;

namespace SuperHearseAI
{
    public class DeathClaim
    {
        public Citizen.Location location;
        public ushort buildingID = 0;
        public uint citizenID = 0;
        public Vector3 pos;
        public ushort vehicleID = 0;
        public bool vehicleAssigned = false;

        public byte districtID
        {
            get { return Singleton<DistrictManager>.instance.GetDistrict(pos); }
        }
    }
}
