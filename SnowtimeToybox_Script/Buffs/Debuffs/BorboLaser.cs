using R2API;
using RoR2;
using R2API.ContentManagement;
using SnowtimeToybox;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static R2API.RecalculateStatsAPI;
using EntityStates.BrotherMonster;

namespace SnowtimeToybox.Buffs
{
    public class BorboTurretDebuff : BuffBase<BorboTurretDebuff>
    {
        public override BuffDef Buff => SnowtimeToyboxMod.BorboTurretDebuff;

        public override void PostCreation()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddBorboTurretDebuff;
        }
        
        private void AddBorboTurretDebuff(CharacterBody sender, StatHookEventArgs args)
        {
            if (!sender.HasBuff(Buff)) return;
            
            args.armorAdd += -120f;
            args.damageTotalMult *= 0.2f;
            args.jumpPowerTotalMult *= 0f;
            args.moveSpeedTotalMult *= 0.2f;
        }
    }
}