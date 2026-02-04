using R2API;
using RoR2;
using R2API.ContentManagement;
using SnowtimeToybox;
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
            bool BorboDebuffed = sender.HasBuff(Buff);
            Log.Debug("Borbo Debuff Active = " + BorboDebuffed);
            if (BorboDebuffed == true)
            {
                Log.Debug("Go Crazy Borbo!");
                args.armorTotalMult += 0f;
                args.armorAdd += -90f;
                args.attackSpeedMultAdd += 0f;
                args.damageMultAdd += 0f;
                args.jumpPowerMultAdd += 0f;
                args.moveSpeedMultAdd += 0f;
            }
        }
    }
}