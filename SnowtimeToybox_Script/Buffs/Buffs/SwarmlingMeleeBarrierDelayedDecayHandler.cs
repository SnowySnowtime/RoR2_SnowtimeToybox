using R2API;
using RoR2;
using SnowtimeToybox;
using SnowtimeToybox.FriendlyTurrets;
using System;
using UnityEngine;
using static R2API.RecalculateStatsAPI;

namespace SnowtimeToybox.Buffs
{
    public class SwarmlingMeleeBarrierDelayedDecayHandler : BuffBase<SwarmlingMeleeBarrierDelayedDecayHandler>
    {
        public override BuffDef Buff => Content.SwarmlingMeleeBarrierDecayDelayHandler;

        public override void PostCreation()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddSwarmlingTurretBuff;
        }

        private void AddSwarmlingTurretBuff(CharacterBody sender, StatHookEventArgs args)
        {
            if (!sender.HasBuff(Buff)) return;

            args.shouldFreezeBarrier = true;
        }
    }
}