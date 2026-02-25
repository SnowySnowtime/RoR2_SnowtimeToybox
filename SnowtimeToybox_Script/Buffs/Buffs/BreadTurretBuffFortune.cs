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
using SnowtimeToybox.FriendlyTurretChecks;

namespace SnowtimeToybox.Buffs
{
    public class BreadTurretBuff : BuffBase<BreadTurretBuff>
    {
        public override BuffDef Buff => SnowtimeToyboxMod.BreadTurretBuffFortune;
        public CharacterBody buffedSelf;

        public override void PostCreation()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddBreadTurretBuff;
        }

        // I want the damage increase to only occur if the victim has a specific buff. I dont know if this code even works.
        // I also dont know if sender is LITERALLY the character sender (looks like it should be obvious but im stupiD), or the API itself. No clue. Just gonna try this anyways.
        private void AddBreadTurretBuff(CharacterBody sender, StatHookEventArgs args)
        {
            if (!sender.HasBuff(Buff)) return;
            var damageIncrease = sender.baseDamage;

            args.luckAdd += 1f;
            if (!buffedSelf) return;
            if (!buffedSelf.HasBuff(SnowtimeToyboxMod.AcanthiTurretBuff)) return;
            args.damageTotalMult += (damageIncrease * 0.25f);
        }
    }
}