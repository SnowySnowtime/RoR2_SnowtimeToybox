using R2API;
using RoR2;
using SnowtimeToybox;
using UnityEngine;
using RoR2.Orbs;

namespace SnowtimeToybox.Buffs
{
    public class ShortcakeTurretBuff : BuffBase<ShortcakeTurretBuff>
    {
        public override BuffDef Buff => SnowtimeToyboxMod.ShortcakeTurretBuff;

        public override void PostCreation()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += ShortcakeDamaged;
        }

        // aetherium being based again
        private void ShortcakeDamaged(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            CharacterBody pookie = victim.GetComponent<CharacterBody>();
            if(pookie != null)
            {
                if (!pookie.HasBuff(SnowtimeToyboxMod.ShortcakeTurretBuff))
                {
                    return;
                }
            }
            //Log.Debug("why did you hurt my pookie :(");

            int a = 999;
            float radius = 45f;
            bool isCrit = pookie.RollCrit();
            float damageValue = 3f * pookie.damage;
            TeamIndex teamIndex2 = pookie.teamComponent.teamIndex;
            HurtBox[] hurtBoxes = new SphereSearch
            {
                origin = damageInfo.position,
                radius = radius,
                mask = LayerIndex.entityPrecise.mask,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
            }.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamIndex2)).OrderCandidatesByDistance()
                .FilterCandidatesByDistinctHurtBoxEntities()
                .GetHurtBoxes();
            for (int m = 0; m < Mathf.Min(a, hurtBoxes.Length); m++)
            {
                SnowtimeOrbs snowtimeOrb = new SnowtimeOrbs();
                snowtimeOrb.attacker = pookie.gameObject;
                snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.ShortcakeRetaliate;
                snowtimeOrb.damageValue = damageValue;
                snowtimeOrb.isCrit = isCrit;
                snowtimeOrb.origin = damageInfo.position;
                snowtimeOrb.range = 45f;
                snowtimeOrb.teamIndex = teamIndex2;
                snowtimeOrb.target = hurtBoxes[m];
                OrbManager.instance.AddOrb(snowtimeOrb);
            }
        }
    }
}