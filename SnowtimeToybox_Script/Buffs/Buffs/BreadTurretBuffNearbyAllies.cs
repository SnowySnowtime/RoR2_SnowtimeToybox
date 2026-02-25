using R2API;
using RoR2;
using R2API.ContentManagement;
using SnowtimeToybox;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static R2API.RecalculateStatsAPI;
using EntityStates.BrotherMonster;
using EntityStates.AffixVoid;
using R2API.AddressReferencedAssets;

namespace SnowtimeToybox.Buffs
{
    public class BreadTurretBuffNearbyAllies : BuffBase<BreadTurretBuffNearbyAllies>
    {
        public override BuffDef Buff => SnowtimeToyboxMod.BreadTurretBuffNearbyAllies;

        public override void PostCreation()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddBreadTurretBuff;
        }

        private void AddBreadTurretBuff(CharacterBody sender, StatHookEventArgs args)
        {
            if (!sender.HasBuff(Buff)) return;

            args.attackSpeedMultAdd += 1.2f;
            args.barrierDecayMult += 0.4f;
        }
    }
}