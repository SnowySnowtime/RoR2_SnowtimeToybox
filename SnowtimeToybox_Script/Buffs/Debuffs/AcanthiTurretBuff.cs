using R2API;
using RoR2;
using SnowtimeToybox;
using UnityEngine;

namespace SnowtimeToybox.Buffs
{
    public class AcanthiVampiricDesires : BuffBase<AcanthiVampiricDesires>
    {
        public override BuffDef Buff => SnowtimeToyboxMod.AcanthiTurretBuff;

        public override void PostCreation()
        {
            On.RoR2.GlobalEventManager.ProcessHitEnemy += VampiricLifeSteal;
        }

        private void VampiricLifeSteal(On.RoR2.GlobalEventManager.orig_ProcessHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            //Log.Debug("Running Lifesteal!");
            CharacterBody acanthi = damageInfo.attacker?.GetComponent<CharacterBody>();
            if (acanthi == null) return;
            if (acanthi != null)
            {
                if (!acanthi.HasBuff(SnowtimeToyboxMod.AcanthiTurretBuff))
                {
                    return;
                }
                float lifestolen = damageInfo.damage;
                acanthi.healthComponent.Heal(lifestolen, damageInfo.procChainMask);
            }
        }
    }
}