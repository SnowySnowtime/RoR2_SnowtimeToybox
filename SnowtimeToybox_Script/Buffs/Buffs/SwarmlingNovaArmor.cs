using R2API;
using RoR2;
using SnowtimeToybox;
using SnowtimeToybox.FriendlyTurrets;
using System;
using UnityEngine;
using static R2API.RecalculateStatsAPI;

namespace SnowtimeToybox.Buffs
{
    public class SwarmlingNovaArmor : BuffBase<SwarmlingNovaArmor>
    {
        public override BuffDef Buff => Content.SwarmlingNovaArmor;

        public override void PostCreation()
        {
            RecalculateStatsAPI.GetStatCoefficients += SwarmlingArmorHook;
        }

        private void SwarmlingArmorHook(CharacterBody sender, StatHookEventArgs args)
        {
            if (!sender.HasBuff(Buff)) return;

            args.armorAdd += 20000;
        }
    }
}