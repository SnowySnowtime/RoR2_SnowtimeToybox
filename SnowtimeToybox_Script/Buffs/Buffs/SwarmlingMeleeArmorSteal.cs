using R2API;
using RoR2;
using SnowtimeToybox;
using SnowtimeToybox.FriendlyTurrets;
using System;
using UnityEngine;
using static R2API.RecalculateStatsAPI;

namespace SnowtimeToybox.Buffs
{
    public class SwarmlingArmorSteal : BuffBase<SwarmlingArmorSteal>
    {
        public override BuffDef Buff => Content.SwarmlingArmorSteal;

        public override void PostCreation()
        {
            RecalculateStatsAPI.GetStatCoefficients += SwarmlingArmorStealHook;
        }

        private void SwarmlingArmorStealHook(CharacterBody sender, StatHookEventArgs args)
        {
            if (!sender.HasBuff(Buff)) return;

            args.armorAdd += ((sender.GetBuffCount(Buff.buffIndex) / 3));
        }
    }
}