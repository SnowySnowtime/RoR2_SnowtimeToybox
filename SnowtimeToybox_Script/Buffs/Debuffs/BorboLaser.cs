using R2API;
using RoR2;
using R2API.ContentManagement;
using SnowtimeToybox;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static R2API.RecalculateStatsAPI;

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
            
            Log.Debug("Go Crazy Borbo!");
            args.armorTotalMult *= 0.2f;
            args.attackSpeedTotalMult *= 0.2f;
            args.damageTotalMult *= 0.2f;
            args.jumpPowerTotalMult *= 0f;
            args.moveSpeedTotalMult *= 0.2f;
            Log.Debug($"armor mult {args.armorTotalMult}");
            Log.Debug($"attackSpeedMultAdd {args.attackSpeedTotalMult}");
            Log.Debug($"damageMultAdd {args.damageTotalMult}");
            Log.Debug($"jumpPowerMultAdd {args.jumpPowerTotalMult}");
            Log.Debug($"moveSpeedTotalMult {args.moveSpeedTotalMult}");
        }
    }
}