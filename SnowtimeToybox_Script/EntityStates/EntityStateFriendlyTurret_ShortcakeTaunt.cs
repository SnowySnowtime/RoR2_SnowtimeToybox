using RoR2;
using SnowtimeToybox;
using UnityEngine;
using RoR2.Orbs;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class ShortcakeTaunt : BaseState
    {
        public static GameObject effectPrefab;

        public static GameObject hitEffectPrefab;

        public static GameObject tracerEffectPrefab;

        public static float damageCoefficient = 1f;

        public static float radius = 45f;

        public static float baseDuration = 2f;

        private float duration;

        int max = 999;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            TeamIndex teamIndex2 = base.gameObject.GetComponent<TeamComponent>().teamIndex;
            HurtBox[] hurtBoxes = new SphereSearch
            {
                origin = base.characterBody.corePosition,
                radius = radius,
                mask = LayerIndex.entityPrecise.mask,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
            }.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamIndex2)).OrderCandidatesByDistance()
                .FilterCandidatesByDistinctHurtBoxEntities()
                .GetHurtBoxes();
            if(base.isAuthority)
            {
                for (int m = 0; m < Mathf.Min(max, hurtBoxes.Length); m++)
                {
                    SnowtimeOrbs snowtimeOrb = new SnowtimeOrbs();
                    snowtimeOrb.attacker = base.gameObject;
                    snowtimeOrb.damageColorIndex = DamageColorIndex.Default;
                    snowtimeOrb.damageValue = damageCoefficient;
                    snowtimeOrb.isCrit = Util.CheckRoll(critStat, base.characterBody.master);
                    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.ShortcakeTaunt;
                    snowtimeOrb.origin = base.characterBody.corePosition;
                    snowtimeOrb.procCoefficient = 0f;
                    snowtimeOrb.range = 0f;
                    snowtimeOrb.teamIndex = teamIndex2;
                    snowtimeOrb.target = hurtBoxes[m];
                    OrbManager.instance.AddOrb(snowtimeOrb);
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}