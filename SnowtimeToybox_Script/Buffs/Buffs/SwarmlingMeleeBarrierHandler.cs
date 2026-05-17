using R2API;
using RoR2;
using SnowtimeToybox;
using SnowtimeToybox.FriendlyTurrets;
using System;
using UnityEngine;
using static R2API.RecalculateStatsAPI;

namespace SnowtimeToybox.Buffs
{
    public class SwarmlingMeleeBarrierHandler : BuffBase<SwarmlingMeleeBarrierHandler>
    {
        public override BuffDef Buff => Content.SwarmlingMeleeBarrierHandler;

        public override void PostCreation()
        {
            On.RoR2.GlobalEventManager.ProcessHitEnemy += KPBMeleeBarrierOnCritHandler;
        }

        private void KPBMeleeBarrierOnCritHandler(On.RoR2.GlobalEventManager.orig_ProcessHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (victim == null) return;
            if (damageInfo.attacker == null) return;
            CharacterBody swarmling = damageInfo.attacker?.GetComponent<CharacterBody>();
            if (swarmling == null) return;
            if (swarmling != null)
            {
                if (!swarmling.HasBuff(Buff)) return;
                if(damageInfo.damageType.damageSource != DamageSource.Primary) return;
                swarmling.AddTimedBuff(Content.SwarmlingArmorSteal, 5);
                if (!damageInfo.crit) return;
                float barrierheal = damageInfo.damage / 5f;
                swarmling.healthComponent.AddBarrier(barrierheal);
                swarmling.AddTimedBuff(Content.SwarmlingMeleeBarrierDecayDelayHandler, 2f);
            }
        }
    }
}