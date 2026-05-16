using R2API;
using RoR2;
using R2API.ContentManagement;
using SnowtimeToybox;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static R2API.RecalculateStatsAPI;
using EntityStates.BrotherMonster;
using SnowtimeToybox.FriendlyTurrets;
using EntityStates.AffixVoid;

namespace SnowtimeToybox.Buffs
{
    public class SwarmlingMeleeArmorStrip : BuffBase<SwarmlingMeleeArmorStrip>
    {
        public override BuffDef Buff => Content.SwarmlingMeleeArmorStrip;

        public override void PostCreation()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddSwarmlingTurretDebuff;
        }
        
        private void AddSwarmlingTurretDebuff(CharacterBody sender, StatHookEventArgs args)
        {
            if (!sender.HasBuff(Buff)) return;
            
            args.armorAdd += (-1f * (sender.GetBuffCount(Buff.buffIndex) / 2));
        }
    }
}