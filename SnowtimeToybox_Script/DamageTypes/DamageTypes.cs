using EntityStates;
using IL.RoR2.UI;
using R2API;
using RoR2.ContentManagement;
using RoR2;
using RoR2.Orbs;
using RoR2.Skills;
using SnowtimeToybox;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace SnowtimeToybox
{
    public static class SnowtimeDamageTypes
    {
        public static DamageAPI.ModdedDamageType HaloRicochetOnHit = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType BorboSuperDebuffOnHit = DamageAPI.ReserveDamageType();

        public static void GlobalEventManager_onServerDamageDealt(DamageReport report)
        {
            if (report.damageInfo.HasModdedDamageType(HaloRicochetOnHit))
            {
                SnowtimeHaloRicochetOrb.CreateHaloRicochetOrb(report.damageInfo, report.attackerTeamIndex, report.victimBody);
            }
            if (report.damageInfo.HasModdedDamageType(BorboSuperDebuffOnHit))
            {
                report.victimBody.AddTimedBuff(SnowtimeToybox.SnowtimeToyboxMod.BorboTurretDebuff, 3);
            }
        }
    }
}