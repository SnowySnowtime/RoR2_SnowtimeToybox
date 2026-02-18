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

            if (damageInfo.attacker == null) return;
            if (victim == null) return;
            CharacterBody pookie = victim?.GetComponent<CharacterBody>();
            if (pookie == null) return;
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
            }.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.all).OrderCandidatesByDistance()
                .FilterCandidatesByDistinctHurtBoxEntities()
                .GetHurtBoxes();
            for (int m = 0; m < Mathf.Min(a, hurtBoxes.Length); m++)
            {
                if (hurtBoxes[m] == null) return;
                if (hurtBoxes[m].teamIndex != pookie.teamComponent.teamIndex)
                {
                    //Log.Debug("Shortcake Turret Retaliation Targeting Enemy!");
                    SnowtimeOrbs snowtimeOrb = new SnowtimeOrbs();
                    snowtimeOrb.attacker = pookie.gameObject;
                    snowtimeOrb.speed = 180f;
                    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.ShortcakeRetaliate;
                    snowtimeOrb.damageValue = damageValue;
                    snowtimeOrb.isCrit = isCrit;
                    snowtimeOrb.origin = damageInfo.position;
                    snowtimeOrb.range = 45f;
                    snowtimeOrb.teamIndex = teamIndex2;
                    snowtimeOrb.target = hurtBoxes[m];
                    OrbManager.instance.AddOrb(snowtimeOrb);
                }
                else if (!hurtBoxes[m].healthComponent.gameObject.GetComponent<CharacterBody>().baseNameToken.Contains("FRIENDLYTURRET_SHORTCAKE") && hurtBoxes[m].teamIndex == pookie.teamComponent.teamIndex)
                {
                    //Log.Debug("Shortcake Turret Retaliation Targeting Ally!");
                    SnowtimeOrbs snowtimeOrb = new SnowtimeOrbs();
                    snowtimeOrb.attacker = pookie.gameObject;
                    snowtimeOrb.speed = 180f;
                    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.ShortcakeRetaliateFriendly;
                    snowtimeOrb.damageValue = damageValue * 0.16f;
                    snowtimeOrb.isCrit = false;
                    snowtimeOrb.origin = damageInfo.position;
                    snowtimeOrb.range = 45f;
                    snowtimeOrb.teamIndex = teamIndex2;
                    snowtimeOrb.target = hurtBoxes[m];
                    OrbManager.instance.AddOrb(snowtimeOrb);
                }
            }
        }
    }
}